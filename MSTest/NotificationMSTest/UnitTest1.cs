using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZRui.Web.Sms;
using ZRui.Web.SMSImp;

namespace NotificationMSTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string moible = "13642833077";
            string content = "11Ŀǰ���ڲ���";
            ISmsHandler sms = new SMS();
          bool result = sms.SendAsync(moible, content).Result;
          

        }
    }
}
