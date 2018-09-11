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
    public class TestBeginRechange
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var shopDb = TShopDbContxtFactory.MarkShopDb())
            {
                using (var db = TShopDbContxtFactory.MarkFinanceDb())
                {
                    BeginRechangeArgsModel args = new BeginRechangeArgsModel();
                    if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
                    if (!args.ShopOrderId.HasValue) throw new ArgumentNullException("ShopOrderId");
                    var memberId = 113;
                    ShopOrder shopOrder = shopDb.GetSingle<ShopOrder>(args.ShopOrderId.Value);
                    if (shopOrder == null) throw new Exception("订单不存在");
                    ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
                        .FirstOrDefault();
                    if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");
                    PayProxyBase payProxy = FactoryUtil.GetPayProxyFactory().GetProxy(shopPayInfo);
                    var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
                    var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var model = new MemberTradeForRechange()
                    {
                        AddTime = DateTime.Now,
                        MemberId = memberId,
                        Detail = "用户支付" + shopOrder.Payment.Value + "分",
                        OutBank = "",
                        PayChannel = payProxy.PayChannel,
                        Status = MemberTradeForRechangeStatus.未完成,
                        TimeExpire = DateTime.Now,
                        TimeStart = DateTime.Now,
                        Title = "用户支付",
                        TotalFee = shopOrder.Payment.Value,
                        ShopOrderId = args.ShopOrderId.Value,
                        MoneyOffRuleId = shopOrder.MoneyOffRuleId,
                        TradeNo = tradeNo,
                        PayWay = (int)shopPayInfo.PayWay,
                        //ShopType = shopOrder
                        RowVersion = DateTime.Now
                    };
                    db.AddToMemberTradeForRechange(model);
                    var payInfo = payProxy.GetPayInfo(model, "");
                }
            }

        }
    }
}
