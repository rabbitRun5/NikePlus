using Microsoft.VisualStudio.TestTools.UnitTesting;
using NikePlusApi;

namespace TestNikeConnector
{
    [TestClass]
    public class NikePlusApiUnitTestBase
    {
        protected const string Username = "rabbitrun@demons.de";
        protected const string Password = "Groovy55";

        protected static NikeConnector GetNikePlusConnector()
        {
            NikeConnector nikeConnector = new NikeConnector(Username, Password);
            nikeConnector.Connect();
            return nikeConnector;
        }
    }
}
