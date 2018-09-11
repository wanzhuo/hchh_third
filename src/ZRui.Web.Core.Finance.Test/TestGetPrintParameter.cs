using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Finance.WechatPay;
using ZRui.Web.Core.Finance.WechatPay.PayAPIModels;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.Core.Finance.Test
{
    [TestClass]
    public class TestGetPrintParameter
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var shopDb = TShopDbContxtFactory.MarkShopDb())
            {
                var order = shopDb.ShopOrders.Find(1199);
                //var parameter = DbExtention.GetPrintParameter(shopDb, order);
            }

        }
    }
}
