using NikePlusApi.JsonObjects;
using OGL_Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NikePlusApi
{
    public class NikeExporter
    {
        private readonly NikeConnector connector;

        public NikeExporter(NikeConnector nikeConnnector)
        {
            connector = nikeConnnector;

            if (!connector.Authenticated)
            {
                throw new ArgumentException("Not authenticated.");
            }
        }

        public string ExportActivityGpx(string activityId)
        {
            ActivityDetail activityDetail = connector.GetActivityDetail(activityId);

            if (activityDetail == null)
                return null;

            List<ConvertGPX.CRoute> routes = new List<ConvertGPX.CRoute>();
            ConvertGPX.CRoute route = CreateRouteOfActivityDetail(activityDetail);
            routes.Add(route);

            string filePath = Path.GetTempPath() + activityDetail.activityId;

            string gpxFileValue = null;
            if (ConvertGPX.GPSConvertToGPX(filePath, null, routes.ToArray()))
            {
                StreamReader streamReader = new StreamReader(filePath);
                gpxFileValue= streamReader.ReadToEnd();
                streamReader.Close();
            }

            return gpxFileValue;
        }

        private ConvertGPX.CRoute CreateRouteOfActivityDetail(ActivityDetail activityDetail)
        {
            List<ConvertGPX.CWaypoint> cWaypoints = GetWayPointsOfActivityDetail(activityDetail);
            ConvertGPX.CRoute route = new ConvertGPX.CRoute();
            route.Routepoints = cWaypoints.ToArray();
            route.name = activityDetail.name;

            return route;
        }

        private List<ConvertGPX.CWaypoint> GetWayPointsOfActivityDetail(ActivityDetail activityDetail)
        {
            List<ConvertGPX.CWaypoint> cWaypoints = new List<ConvertGPX.CWaypoint>();
            foreach (Waypoint wp in activityDetail.geo.waypoints)
            {
                ConvertGPX.CWaypoint cwp = new ConvertGPX.CWaypoint();
                cwp.lat = wp.lat.ToString(CultureInfo.InvariantCulture);
                cwp.lon = wp.lon.ToString(CultureInfo.InvariantCulture);
                cwp.ele = wp.ele.ToString(CultureInfo.InvariantCulture);

                cWaypoints.Add(cwp);
            }
            return cWaypoints;
        }

        //private XmlTextReader GetXmlTextReaderForRoutes()
    }
}
