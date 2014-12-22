using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NikePlusApi;
using NikePlusApi.JsonObjects;

namespace TestNikeConnector
{
    [TestClass]
    public class NikeConnectorUnitTest : NikePlusApiUnitTestBase
    {
        [TestMethod]
        public void ConnectToNikeServiceCorrectly()
        {
            NikeConnector nikeConnector = GetNikePlusConnector();

            Assert.IsNotNull(nikeConnector.AuthenticationCookie);
            Assert.IsTrue(nikeConnector.Authenticated);
        }

        [TestMethod]
        public void InvalidConnectToNikeService()
        {
            NikeConnector nikeConnector = new NikeConnector("invalid", "login");
            nikeConnector.Connect();

            Assert.IsNull(nikeConnector.AuthenticationCookie);
            Assert.IsFalse(nikeConnector.Authenticated);
        }

        [TestMethod]
        public void ShowCorrectDisplayName()
        {
            NikeConnector nikeConnector = GetNikePlusConnector();
            Assert.AreEqual(nikeConnector.DisplayName, "rabbitRun");
        }

        [TestMethod]
        public void ReceiveActivities()
        {
            NikeConnector nikeConnector = GetNikePlusConnector();

            List<Activity> activities = nikeConnector.GetActivities(0, 1);
            Assert.IsTrue(activities.Count == 1);

            activities = nikeConnector.GetActivities(0, 2);
            Assert.IsTrue(activities.Count == 2);
        }

        [TestMethod]
        public void ReceiveActivitiesDetails()
        {
            NikeConnector nikeConnector = GetNikePlusConnector();

            List<Activity> activities = nikeConnector.GetActivities(0, 1);
            ActivityDetail detail = nikeConnector.GetActivityDetail(activities[0].ActivityId);

            Assert.IsNotNull(detail);
        }

        [TestMethod]
        public void ReceiveLifeTimeActivities()
        {
            NikeConnector nikeConnector = GetNikePlusConnector();

            List<Activity> activities = nikeConnector.GetLifeTimeActivities();
            Assert.IsTrue(activities.Count > 1);
        }

        [TestMethod]
        public void TestLogging()
        {
            NikeConnector nikeConnector = new NikeConnector(Username, Password);

            object logSender = null;
            LogEventArgs logEventArgs = null;
            nikeConnector.Logging += delegate(object sender, LogEventArgs args)
            {
                logSender = sender;
                logEventArgs = args;
            };

            nikeConnector.Connect();

            Assert.IsNotNull(logSender);
            Assert.IsTrue(logEventArgs.Message != String.Empty);
        }

        [TestMethod]
        public void ReceivActivitesWithoutLoggingIn()
        {
            NikeConnector connector = new NikeConnector(Username, Password);
            List<Activity> activities = connector.GetActivities(0, 1);

            Assert.AreEqual(0, activities.Count);
        }
    }
}
