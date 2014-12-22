using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NikePlusApi.JsonObjects
{
    [DataContract]
    public class Metrics
    {
        [DataMember(Name = "averageHeartRate")]
        public double AverageHeartRate { get; set; }

        [DataMember(Name = "minimumHeartRate")]
        public double MinimumHeartRate { get; set; }

        [DataMember(Name = "maximumHeartRate")]
        public double MaximumHeartRate { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "calories")]
        public int Calories { get; set; }

        [DataMember(Name = "fuel")]
        public int Fuel { get; set; }

        [DataMember(Name = "steps")]
        public int Steps { get; set; }

        [DataMember(Name = "distance")]
        public double Distance { get; set; }
    }

    [DataContract]
    public class Activity
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "activityId")]
        public string ActivityId { get; set; }

        [DataMember(Name = "activityType")]
        public string ActivityType { get; set; }

        [DataMember(Name = "timeZone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "timeZoneId")]
        public string TimeZoneId { get; set; }

        [DataMember(Name = "dstOffset")]
        public string DstOffset { get; set; }

        [DataMember(Name = "startTimeUtc")]
        public string StartTimeUtc { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "activeTime")]
        public int ActiveTime { get; set; }

        [DataMember(Name = "gps")]
        public bool Gps { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "heartrate")]
        public bool Heartrate { get; set; }

        [DataMember(Name = "deviceType")]
        public string DeviceType { get; set; }

        [DataMember(Name = "metrics")]
        public Metrics Metrics { get; set; }
    }

    [DataContract]
    public class ActivityContainer
    {
        [DataMember(Name = "activity")]
        public Activity Activity { get; set; }
    }

    [DataContract]
    public class NikeActivityRootObject
    {
        [DataMember(Name = "activities")]
        public List<ActivityContainer> Activities { get; set; }
    }


    public class Dataset
    {
        public string name { get; set; }
        public int duration { get; set; }
        public double distance { get; set; }
        public double heartRate { get; set; }
        public double pace { get; set; }
        public double gpsLat { get; set; }
        public double gpsLong { get; set; }
        public int time { get; set; }
    }


    public class KMSPLIT
    {
        public List<Dataset> datasets { get; set; }
    }


    public class Dataset2
    {
        public string name { get; set; }
        public int duration { get; set; }
        public double distance { get; set; }
        public double heartRate { get; set; }
        public double pace { get; set; }
        public double gpsLat { get; set; }
        public double gpsLong { get; set; }
        public int time { get; set; }
    }

    public class USERCLICK
    {
        public List<Dataset2> datasets { get; set; }
    }


    public class Dataset3
    {
        public string name { get; set; }
        public int duration { get; set; }
        public double distance { get; set; }
        public double heartRate { get; set; }
        public double pace { get; set; }
        public double gpsLat { get; set; }
        public double gpsLong { get; set; }
        public int time { get; set; }
    }

 
    public class MILESPLIT
    {
        public List<Dataset3> datasets { get; set; }
    }

    public class Snapshots
    {
        public KMSPLIT KMSPLIT { get; set; }
        public USERCLICK USERCLICK { get; set; }
        public MILESPLIT MILESPLIT { get; set; }
    }


    public class Waypoint
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public double ele { get; set; }
    }

    
    public class Geo
    {
        public string coordinate { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public double elevationLoss { get; set; }
        public double elevationGain { get; set; }
        public double elevationMin { get; set; }
        public double elevationMax { get; set; }
    }

    
    public class Tags
    {
    }

    
    public class History
    {
        public int startTimeOffset { get; set; }
        public string intervalType { get; set; }
        public int intervalMetric { get; set; }
        public string intervalUnit { get; set; }
        public string type { get; set; }
        public List<double> values { get; set; }
    }


    public class Achievement
    {
        public long achievementDate { get; set; }
        public string achievementType { get; set; }
        public string metricType { get; set; }
        public double targetValue { get; set; }
    }


    public class ActivityDetail
    {
        public string name { get; set; }
        public string activityId { get; set; }
        public string activityType { get; set; }
        public string timeZone { get; set; }
        public string timeZoneId { get; set; }
        public string dstOffset { get; set; }
        public string startTimeUtc { get; set; }
        public string status { get; set; }
        public int activeTime { get; set; }
        public bool gps { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool heartrate { get; set; }
        public string deviceType { get; set; }
        public string nextId { get; set; }
        public string prevId { get; set; }
        public int duration { get; set; }
        public int calories { get; set; }
        public int fuel { get; set; }
        public int steps { get; set; }
        public double distance { get; set; }
        public double averageHeartRate { get; set; }
        public double minimumHeartRate { get; set; }
        public double maximumHeartRate { get; set; }
        public bool isTopRoute { get; set; }
        public long syncDate { get; set; }
        public Snapshots snapshots { get; set; }
        public Geo geo { get; set; }
        public Tags tags { get; set; }
        public List<History> history { get; set; }
        public List<Achievement> achievements { get; set; }
    }

    public class NikeActivityDetailRootObject
    {
        public ActivityDetail activity { get; set; }
    }
}
