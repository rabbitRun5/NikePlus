using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NikePlusApi;
using NikePlusApi.JsonObjects;
using System.Xml;

namespace TestNikeConnector
{
    [TestClass]
    public class NikeExporterUnitTest : NikePlusApiUnitTestBase
    {
        [TestMethod]
        public void ExportValidGpxData()
        {
            NikeConnector connector = GetNikePlusConnector();
            Activity activity = connector.GetActivities(0, 1)[0];

            NikeExporter exporter = new NikeExporter(connector);
            string gxpValue = exporter.ExportActivityGpx(activity.ActivityId);

            Assert.IsFalse(String.IsNullOrEmpty(gxpValue));
        }

        [TestMethod]
        public void ExportInvalidGpxData()
        {
            NikeConnector connector = GetNikePlusConnector();
            NikeExporter exporter = new NikeExporter(connector);

            string gpxValue = exporter.ExportActivityGpx("invalidId");

            Assert.IsTrue(String.IsNullOrEmpty(gpxValue));
        }
    }
}
