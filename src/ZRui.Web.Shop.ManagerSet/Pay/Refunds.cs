using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Utils;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.SwiftpassPay;

namespace ZRui.Web.Pay
{
    public class Refunds
    {
        FinanceDbContext db;
        ShopDbContext shopDb;
        PayProxyFactory proxyFactory;
        HchhLogDbContext hchhLog;

        public Refunds(PayProxyFactory proxyFactory)
        {

            this.db = ZRui.Web.BLL.DbContextFactory.FinanceDbContext;
            this.shopDb = ZRui.Web.BLL.DbContextFactory.ShopDb;
            this.proxyFactory = proxyFactory;
            this.hchhLog = ZRui.Web.BLL.DbContextFactory.LogDbContext;


        }




        public MemberTradeForRefund RefundAction(RefundArgsModel args)
        {

            var orderid = 0;

            var Refundno = string.Empty;
            if (string.IsNullOrEmpty(args.TradeNo))
            {
                throw new ArgumentNullException("缺少必须参数");
            }
            if (args.Amount<=0) {
                throw new ArgumentNullException("退款金额必须大于0");
            }
            var memberrechanges = db.MemberTradeForRechanges.FirstOrDefault(r => r.TradeNo == args.TradeNo && r.Status == MemberTradeForRechangeStatus.成功);
            if (memberrechanges == null)
            {
                throw new ArgumentNullException("未找到支付记录,请检查数据是否正确！");
            }
            orderid = memberrechanges.OrderId;
            var memberrefundobj = db.memberTradeForRefunds.FirstOrDefault(r => r.TradeNo == args.TradeNo && r.Status == MemberTradeForRefundStatus.成功);
            if (memberrefundobj != null)
            {
                throw new ArgumentNullException($"订单{args.TradeNo}已成功退款，不能重复退款！");
            }
            //当前开通的支付通道

            ShopPayInfo currentshopPayInfo = shopDb.Query<ShopPayInfo>()
             .Where(m => !m.IsDel)
             .Where(m => m.ShopId == memberrechanges.ShopId&&m.IsEnable)
             .FirstOrDefault();

            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
              .Where(m => !m.IsDel)
              .Where(m => m.ShopId == memberrechanges.ShopId && (int)m.PayWay == memberrechanges.PayWay)
              .FirstOrDefault();
            if (currentshopPayInfo.PayWay!= shopPayInfo.PayWay) throw new Exception("退款通道不匹配,请核实。");
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好退款信息。");
            var payProxy = proxyFactory.GetProxy(shopPayInfo);

            if (memberrechanges.OrderType == OrderType.普通订单)
            {
                Refundno = "TK" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            }
            else
            {
                Refundno = "PTTK" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            }


            var model = new MemberTradeForRefund()
            {
                //AddIP = GetIp(),
                AddTime = DateTime.Now,
                //AddUser = GetUsername(),
                MemberId = memberrechanges.MemberId,
                Detail = "用户退款" + memberrechanges.TotalFee + "分",
                OutBank = "",
                PayChannel = payProxy.PayChannel,
                Status = MemberTradeForRefundStatus.退款中,
                TimeExpire = DateTime.Now,
                TimeStart = DateTime.Now,
                Title = "用户退款",
                TotalFee = memberrechanges.TotalFee,
                TradeNo = memberrechanges.TradeNo,
                MechanismTradeNo = memberrechanges.MechanismTradeNo,
                RefundTradeNo = Refundno,
                OrderType = memberrechanges.OrderType,
                OrderId = memberrechanges.OrderId
            };



            SwiftpassPayResponseHandler obj = payProxy.Refund(model) as SwiftpassPayResponseHandler;

            if (obj.Status != 0)
            {
                new RefundLog<object>(hchhLog).RefundAction("Refund"
                       , BLL.Log.PayOrRefundType.退款, memberrechanges.OrderId, memberrechanges.OrderType, model, obj, obj.ErrMsg);

                model.Status = MemberTradeForRefundStatus.失败;
                db.AddToMemberTradeForRefund(model);
                db.SaveChanges();

                return model;

            }

            if (obj.ResultCode != 0)
            {
                model.Detail = obj.ErrCode;
                #region 添加短信余额不足发短信
                if (obj.ErrCode == "REFUND_FEE_INVALID")
                {
                    var shop = shopDb.Shops.FirstOrDefault(m => m.Flag.Equals(args.ShopFlag));
                    if (!string.IsNullOrWhiteSpace(shop.Phone))
                    {
                        model.Detail = "账户余额可能存在不足的情况,导致用户无法申请退款，请查看账户余额";
                        SMSHelper.Send(shop.Phone, model.Detail);
                    }

                }
                #endregion
                model.Status = MemberTradeForRefundStatus.失败;
                db.AddToMemberTradeForRefund(model);
                db.SaveChanges();
                new RefundLog<object>(hchhLog).RefundAction("Refund"
                   , BLL.Log.PayOrRefundType.退款, memberrechanges.OrderId, memberrechanges.OrderType, model, obj, obj.ErrMsg);
                return model;
            }

            var refundresult = payProxy.GetRefundResult(model) as SwiftpassPayResponseHandler;

            string returnCode = refundresult.parameters["refund_status_0"].ToString();
            if (returnCode == "SUCCESS")
            {
                model.Status = MemberTradeForRefundStatus.成功;
            }
            else if (returnCode == "FAIL")
            {
                model.Status = MemberTradeForRefundStatus.失败;
            }
            else if (returnCode == "PROCESSING")
            {
                model.Status = MemberTradeForRefundStatus.退款中;


            }
            else
            {
                model.Detail = returnCode;
                model.Status = MemberTradeForRefundStatus.失败;


            }
            db.AddToMemberTradeForRefund(model);
            db.SaveChanges();
            return model;

        }
    }
}
