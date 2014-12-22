using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml;
using NikePlusApi.JsonObjects;

namespace NikePlusApi
{
    public class NikeConnector
    {
        #region Fields

        private  string login;
        private string password;

        private string displayName;
        private bool authenticated;

        private Cookie authenticationCookie;


        #endregion

        #region Constants

        private const string secureNikePlusLogin = @"https://secure-nikeplus.nike.com/nsl/services/user/login?app=b31990e7-8583-4251-808f-9dc67b40f5d2";
        private const string secureNikePlusActivities = @"https://secure-nikeplus.nike.com/plus/activity/running/{0}/lifetime/activities?indexStart={1}&indexEnd={2}";
        private const string secureNikePlusActivityDetail = @"https://secure-nikeplus.nike.com/plus/running/ajax/{0}";

        #endregion

        #region Properties

        public Cookie AuthenticationCookie
        {
            get { return authenticationCookie; }
        }

        public bool Authenticated
        {
            get { return authenticated; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        #endregion

        #region Constructors

        public NikeConnector(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        #endregion

        #region Delegates

        public delegate void LoggingEventHandler(object sender, LogEventArgs e);

        #endregion

        #region Events

        public event LoggingEventHandler Logging;

        #endregion

        #region Methods

        /// <summary>
        /// Establish the connection with nike plus services for further requests.
        /// </summary>
        /// <returns>true if a authentication was successful else false</returns>
        public bool Connect()
        {
            ValidateAuthentication();
            authenticated = AuthenticationCookie != null;

            return authenticated;
        }

        public List<Activity> GetLifeTimeActivities()
        {
            List<Activity> lifeTimeActivies = new List<Activity>();
            List<Activity> partActivities;

            const int increment = 10;
            uint startIndex = 0;
            uint lastIndex = increment;

            do
            {
                partActivities = GetActivities(startIndex, lastIndex);
                lifeTimeActivies.AddRange(partActivities);

                startIndex += increment;
                lastIndex += increment;


            } while (partActivities.Count > 0);


            return lifeTimeActivies;
        }

        public List<Activity> GetActivities(uint startIndex, uint lastIndex)
        {
            List<Activity> nikeActivities = new List<Activity>();
            NikeActivityRootObject activityRoot = (NikeActivityRootObject)GetJsonObjectFromNikeRequest(String.Format(secureNikePlusActivities, DisplayName, startIndex, lastIndex), typeof(NikeActivityRootObject));

            if (activityRoot != null && activityRoot.Activities != null)
            {
                foreach (ActivityContainer activityContainer in activityRoot.Activities)
                {
                    nikeActivities.Add(activityContainer.Activity);
                }
            }
            return nikeActivities;
        }

        public ActivityDetail GetActivityDetail(string id)
        {
            NikeActivityDetailRootObject activityDetailRoot = (NikeActivityDetailRootObject)GetJsonObjectFromNikeRequest(String.Format(secureNikePlusActivityDetail, id), typeof(NikeActivityDetailRootObject));

            return activityDetailRoot != null ? activityDetailRoot.activity : null;
        }

        private void ValidateAuthentication()
        {
            if (!Authenticated || AuthenticationCookie == null)
            {
                authenticationCookie = GetAuthenticationCookie(login, password);
            }
        }

        /// <summary>
        /// Creates a new <c>AuthenticationCookie</c> for nike plus services.
        /// </summary>
        /// <param name="email">A valid nikeplus login (email),</param>
        /// <param name="pwd">The assosicated password.</param>
        /// <returns>A <c>Cookie</c> used for nikeplus authentication.</returns>
        private Cookie GetAuthenticationCookie(string email, string pwd)
        {
            Cookie authCookie = null;

            string postData = String.Format("email={0}&password={1}", HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(pwd));
            byte[] bytePostData = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest loginRequest = WebRequest.CreateHttp(secureNikePlusLogin);
            loginRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            loginRequest.ContentLength = bytePostData.Length;
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Method = "POST";

            Stream requestStream = loginRequest.GetRequestStream();
            requestStream.Write(bytePostData, 0, bytePostData.Length);
            requestStream.Close();

            Log(String.Format("HttpWebRequest {0} with {1}", secureNikePlusLogin, postData));

            HttpWebResponse loginResponse = null;
            Stream responseStream = null;

            try
            {
                loginResponse = (HttpWebResponse)loginRequest.GetResponse();
                responseStream = loginResponse.GetResponseStream();

                string setCookieHeader = loginResponse.Headers["SET-COOKIE"];
                bool success = false;
                if (responseStream != null)
                {
                    StreamReader streamReader = new StreamReader(responseStream);
                    XmlDocument xmlDocument = new XmlDocument();

                    xmlDocument.LoadXml(streamReader.ReadToEnd());
                    streamReader.Close();
                    loginResponse.Close();
                    responseStream.Close();

                    XmlNode node = xmlDocument.SelectSingleNode("serviceResponse//header//success");
                    success = node != null && node.InnerText.Equals("true");

                    if (success)
                    {
                        node = xmlDocument.SelectSingleNode("serviceResponse//body//User//displayName");
                        displayName = node != null ? node.InnerText : String.Empty;
                    }
                }

                if (success && setCookieHeader != null)
                {
                    authCookie = new Cookie();
                    foreach (string splitString in setCookieHeader.Split(';'))
                    {
                        if (splitString.Contains("="))
                        {
                            string key = splitString.Split('=')[0];
                            string value = splitString.Split('=')[1];

                            if (key.ToLower().Trim() == "llcheck")
                            {
                                authCookie.Name = key;
                                authCookie.Value = value;
                            }
                            else if (key.ToLower().Trim() == "domain")
                            {
                                authCookie.Domain = value;
                            }
                            else if (key.ToLower().Trim() == "path")
                            {
                                authCookie.Path = value;
                            }
                        }
                    }

                }
            }
            catch (WebException)
            {
                if (loginResponse != null)
                    loginResponse.Close();

                if (responseStream != null)
                    responseStream.Close();
            }

            return authCookie;
        }


        private object GetJsonObjectFromNikeRequest(string url, Type jsonType)
        {
            ValidateAuthentication();

            HttpWebRequest activityRequest = WebRequest.CreateHttp(url);
            activityRequest.CookieContainer = new CookieContainer();
            activityRequest.CookieContainer.Add(authenticationCookie);
            activityRequest.Accept = "text/html, application/xhtml+xml, application/xml;q=0.9, */*;q=0.8";
            activityRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
            activityRequest.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
            activityRequest.Headers.Add("Accept-Encoding", "gzip, deflate");

            HttpWebResponse activityResponse = null;
            Stream activityResponseStream = null;
            object jsonObject = null;

            Log(String.Format("HttpWebRequest {0} ", activityRequest.RequestUri));

            try
            {
                activityResponse = (HttpWebResponse)activityRequest.GetResponse();
                activityResponseStream = activityResponse.GetResponseStream();

                if (activityResponseStream != null && activityResponse.ContentLength > 0)
                {
                    if (activityResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        activityResponseStream = new GZipStream(activityResponseStream, CompressionMode.Decompress);
                    }

                    try
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(jsonType);
                        jsonObject = jsonSerializer.ReadObject(activityResponseStream);
                    }
                    catch (SerializationException exception)
                    {
                        Log("Could not read json objects for " + url);
                        Log(exception.Message);
                    }
                }
                else
                {
                    Log("No content");
                }

            }
            catch (WebException webException)
            {
                Log(webException.Message);
            }
            finally
            {
                if (activityResponse != null)
                    activityResponse.Close();

                if (activityResponseStream != null)
                    activityResponseStream.Close();
            }

            return jsonObject;
        }

        protected virtual void Log(string message)
        {
            if (Logging != null)
            {
                LogEventArgs logEventArgs = new LogEventArgs(String.Format("[{0}]: {1}{2}", DateTime.Now, message, Environment.NewLine));
                Logging(this, logEventArgs);
            }
        }

        #endregion
    }
}