using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Log;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;
using ZRui.Web.BLL.Utils;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Finance.SwiftpassPay;
using ZRui.Web.Core.Finance.WechatPay.PayAPIModels;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Pay;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class SwiftpassPayForShopWechatOpenAPIController : WechatApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        readonly ILogger _logger;
        FinanceDbContext finaceDb;
        WechatTemplateSendOptions options;
        ShopDbContext shopDb;
        Core.Printer.Data.PrintDbContext printDbContext;
        PayProxyFactory proxyFactory;
        HchhLogDbContext LogDbContext;
        ThirdConfig thirdConfig;
        public SwiftpassPayForShopWechatOpenAPIController(IOptions<MemberAPIOptions> memberOptions
            //, FinanceDbContext db
            , IOptions<ThirdConfig> poptions
            , IOptions<WechatTemplateSendOptions> options
            , ShopDbContext shopDb
            , Core.Printer.Data.PrintDbContext printDbContext
            , PayProxyFactory proxyFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment) : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.finaceDb = DbContextFactory.FinanceDbContext;
            this.options = options.Value;
            this.shopDb = shopDb;
            this.wechatCoreDb = wechatCoreDb;
            this.memberDb = memberDb;
            this.LogDbContext = DbContextFactory.LogDbContext;
            this.printDbContext = printDbContext;
            this.thirdConfig = poptions.Value;
            this._logger = loggerFactory.CreateLogger<SwiftpassPayForShopWechatOpenAPIController>();
            this.proxyFactory = proxyFactory;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BeginRechange([FromBody]BeginRechangeArgsModel args)
        {
            try
            {
                if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
                if (!args.ShopOrderId.HasValue) throw new ArgumentNullException("ShopOrderId");
                var memberId = GetMemberId();
                ShopOrder shopOrder = shopDb.GetSingle<ShopOrder>(args.ShopOrderId.Value);
                if (shopOrder == null) throw new Exception("订单不存在");
                ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
                    .FirstOrDefault();
                if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");


                //排除扫码 自助点餐
                var checkThirdResult = CheckThird(shopOrder, shopOrder.ShopId);
                if (!checkThirdResult.Success)
                {
                    throw new Exception(checkThirdResult.Message);
                }

                PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
                var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
                var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                var model = new MemberTradeForRechange()
                {
                    AddIP = GetIp(),
                    AddTime = DateTime.Now,
                    AddUser = GetUsername(),
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
                    RowVersion = DateTime.Now,
                    OrderType = OrderType.普通订单,
                    OrderId = args.ShopOrderId.Value,
                    ConglomerationOrderId = 0,
                    ShopId = shopPayInfo.ShopId
                };
                finaceDb.AddToMemberTradeForRechange(model);
                // var openId = "oYBBo5PUP3TF37pUNiVvc7j5gJ6k"; //"oYBBo5PUP3TF37pUNiVvc7j5gJ6k";//
                var openId = GetOpenId(); //"oYBBo5PUP3TF37pUNiVvc7j5gJ6k";//
                var payInfo = payProxy.GetPayInfo(model, openId);
                finaceDb.SaveChanges();
                if (shopPayInfo.PayWay == PayWay.Wechat)
                {
                    new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                        , BLL.Log.PayOrRefundType.支付, args.ShopOrderId.Value, OrderType.普通订单, model, payInfo);
                    return Success(new
                    {
                        payInfo,
                        TradeNo = tradeNo
                    });
                }
                else
                {
                    PayInfoModel infoModel = new PayInfoModel();
                    var payinfo = JsonConvert.DeserializeObject<ZRui.Web.Core.Finance.WechatPay.PayAPIModels.PayInfo>(payInfo.ToString());
                    infoModel.payInfo = payinfo;
                    new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                         , BLL.Log.PayOrRefundType.支付, args.ShopOrderId.Value, OrderType.普通订单, model, payInfo);
                    return Success(
                        infoModel
                    );
                }

            }
            catch (Exception ex)
            {
                new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                        , BLL.Log.PayOrRefundType.支付, args.ShopOrderId.Value, OrderType.普通订单, null, null, ex.Message + "【StackTrace】" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
            //if (args.TotalFee <= 0) throw new Exception("充值金额需要大于0");

        }
        [HttpPost]
        public APIResult PayApp([FromBody]BeginRechangeArgsModel args)
        {
            ShopOrder shopOrder = shopDb.GetSingle<ShopOrder>(args.ShopOrderId.Value);
            if (shopOrder == null) throw new Exception("订单不存在");
            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
                .FirstOrDefault();
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");


            //排除扫码 自助点餐
            var checkThirdResult = CheckThird(shopOrder, shopOrder.ShopId);
            if (!checkThirdResult.Success)
            {
                throw new Exception(checkThirdResult.Message);
            }

            PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
            var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var model = new MemberTradeForRechange()
            {
                AddIP = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                MemberId = 0,
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
                RowVersion = DateTime.Now,
                OrderType = OrderType.普通订单,
                OrderId = args.ShopOrderId.Value,
                ConglomerationOrderId = 0,
                ShopId = shopPayInfo.ShopId
            };
            finaceDb.AddToMemberTradeForRechange(model);
            var payInfo = payProxy.GetPayAppInfo(model, "2018");
            finaceDb.SaveChanges();
            return Success();
        }

        [HttpPost]
        public APIResult OfflinePay([FromBody] OfflinePayModel model)
        {
            return Success();
        }
        [HttpGet]
        public APIResult TestOfx()
        {
          
            var OpenOfx = BaiduMapUtil.GetBdToGd(113.76031219944932, 23.012808108215836);
            var thirddshopaddmodel = new ThirdShopAddOrderModel()
            {
                ShopId = 11,
                origin_id = DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5),//
                                                  //shop_no = "11047059",//测试
                shop_no = "9896-126130",
                cargo_type = -1,
                cargo_price = 10,
                city_code = "0769",
                is_prepay = 0,
                origin_mark = "HCHH",
                origin_mark_no = DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5),
                receiver_lng = 113.7538969583704,
                receiver_lat = 23.00648851131533,
                receiver_phone = "15812808736",
                receiver_address = "百安中心",
                receiver_name = "万",
                callback = thirdConfig.CallBackUrl,
            };
            var result = DbExtention.ThirdAmountFinish(shopDb, thirdConfig, new BLL.Third.CThirdShopRechargeQueryModel() { ShopId = 11, category = 1 }).Result;
            var preresult = new ThirdServer(shopDb, thirdConfig).PreThirdAddOrder(thirddshopaddmodel);

            return Success();
        }

        private APIResult CheckThird(ShopOrder shopOrder, int ShopId)
        {
            try
            {
                if (shopOrder.ShopPartId.HasValue || shopOrder.ShopOrderSelfHelpId.HasValue)
                {
                    return Success();
                }
                var shopordertakeoutinfo = shopDb.ShopOrderTakeouts.FirstOrDefault(r => r.ShopOrderId == shopOrder.Id && !r.IsDel);
                if (shopordertakeoutinfo.TakeWay != TakeWay.送货上门)
                {
                    return Success();
                }

                var shoptakeoutinfo = shopDb.ShopTakeOutInfo.FirstOrDefault(r => r.ShopId == ShopId && !r.IsDel);
                if (shoptakeoutinfo.TakeDistributionType == TakeDistributionType.商家配送)
                {
                    return Success();
                }

                var thirdshop = shopDb.ThirdShop.FirstOrDefault(r => r.ShopId == ShopId);
                if (thirdshop == null || thirdshop.Status != Data.ThirdShop.ShopStatus.门店激活)
                {
                    return Error("配送门店状态不正常");
                }
                var converResult = BaiduMapUtil.GetBdToGd(shopordertakeoutinfo.Longitude.Value, shopordertakeoutinfo.Latitude.Value);
                var thirddshopaddmodel = new ThirdShopAddOrderModel()
                {
                    ShopId = shopOrder.ShopId,
                    origin_id = shopOrder.OrderNumber,//DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5),//
                                                      //shop_no = "11047059",//测试
                    shop_no = thirdshop.OriginShopId,
                    cargo_type = -1,
                    cargo_price = shopOrder.Amount,
                    city_code = "0769",
                    is_prepay = 0,
                    origin_mark = "HCHH",
                    origin_mark_no = shopOrder.OrderNumber,
                    receiver_lng = converResult.x,
                    receiver_lat = converResult.y,
                    receiver_phone = shopordertakeoutinfo.Phone,
                    receiver_address = shopordertakeoutinfo.Address,
                    receiver_name = shopordertakeoutinfo.Name,
                    callback = thirdConfig.CallBackUrl,
                };
                var result = DbExtention.ThirdAmountFinish(shopDb, thirdConfig, new BLL.Third.CThirdShopRechargeQueryModel() { ShopId = shopOrder.ShopId, category = 1 }).Result;
                var preresult = new ThirdServer(shopDb, thirdConfig).PreThirdAddOrder(thirddshopaddmodel);
                if (result.result.deliverBalance < preresult.result.fee)
                {
                    return Error("商家配送帐号余额不足");
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"出现错误：{ex.Message}{ex.StackTrace}");
            }


            return Success();

        }

        public async void TestThird([FromBody]ThirdModel model)
        {
            //var order = shopDb.ShopOrders.FirstOrDefault(r => r.Id == model.OrderId);
            //await InitiateThird(order, model.ShopId);
            // await DbExtention.ThirdOrderFinish(shopDb,thirdConfig,model.OrderId, _logger, ExSource.支付成功);
        }

        [HttpPost]
        [Authorize]
        public APIResult BeginRefund(RefundArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag) || string.IsNullOrEmpty(args.TradeNo))
            {
                throw new ArgumentNullException("缺少必须参数");
            }
            var memberrechanges = finaceDb.MemberTradeForRechanges.FirstOrDefault(r => r.TradeNo == args.TradeNo && r.Status == MemberTradeForRechangeStatus.成功);
            if (memberrechanges == null)
            {
                return Error("未找到支付记录,请检查数据是否正确！");
            }
            var memberrefundobj = finaceDb.memberTradeForRefunds.FirstOrDefault(r => r.TradeNo == args.TradeNo && r.Status == MemberTradeForRefundStatus.成功);
            if (memberrefundobj != null)
            {
                throw new ArgumentNullException($"订单{args.TradeNo}已成功退款，不能重复退款！");
            }
            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
              .Where(m => !m.IsDel)
              .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
              .FirstOrDefault();
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好退款信息。");
            PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);

            var Refundno = "TK" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);

            var model = new MemberTradeForRefund()
            {
                AddIP = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
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
                RefundTradeNo = Refundno


            };
            finaceDb.AddToMemberTradeForRefund(model);
            var obj = payProxy.Refund(model);
            return Success(obj);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetRechangeResult([FromBody]GetRechangeResultArgsModel args)
        {
            if (string.IsNullOrEmpty(args.TradeNo)) throw new ArgumentNullException("TradeNo");
            if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
            var memberId = GetMemberId();
            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopFlag == args.ShopFlag)
                .FirstOrDefault();
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");
            PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
            var rechange = finaceDb.Query<MemberTradeForRechange>()
                .Where(m => m.MemberId == memberId && m.TradeNo == args.TradeNo)
                .FirstOrDefault();
            if (rechange == null) throw new Exception("指定的纪录不存在");
            _logger.LogInformation($"获取到rechange{rechange.Id},状态 {rechange.Status}");

            if (rechange.Status == MemberTradeForRechangeStatus.未完成)
            {
                var result = payProxy.GetPayResult(rechange);
                _logger.LogInformation($"获取到result{result.Xml}");
                //rechange.SetFinish(shopDb,db,options, result,_logger);
                finaceDb.SaveChanges();
            }
            return Success(rechange);
        }

        [HttpPost]
        [Authorize]
        public APIResult GetRefundResult([FromBody]RefundArgsModel args)
        {
            if (string.IsNullOrEmpty(args.TradeNo)) throw new ArgumentNullException("TradeNo");
            if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
            var memberId = GetMemberId();
            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopFlag == args.ShopFlag)
                .FirstOrDefault();
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");
            PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
            var refund = finaceDb.Query<MemberTradeForRefund>()
                .Where(m => m.MemberId == memberId && m.TradeNo == args.TradeNo)
                .FirstOrDefault();
            if (refund == null) throw new Exception("指定的纪录不存在");
            _logger.LogInformation($"获取到rechange{refund.Id},状态 {refund.Status}");

            if (refund.Status == MemberTradeForRefundStatus.退款中)
            {
                var result = payProxy.GetRefundResult(refund);
                _logger.LogInformation($"获取到result{result.Xml}");
                //rechange.SetFinish(shopDb,db,options, result,_logger);
                finaceDb.SaveChanges();
            }
            return Success(refund);
        }
        [HttpPost]
        public void TestRefund([FromBody]RefundArgsModel args)
        {
            //  this.LogDbContext.Add(new TaskLog() { TaskName = "TestRefund", ExeResult = "" });
            //this.LogDbContext.SaveChanges();
            Refunds refund = new Refunds(proxyFactory);
            refund.RefundAction(new ZRui.Web.Core.Finance.WechatPay.PayAPIModels.RefundArgsModel()
            {
                ShopFlag = args.ShopFlag,
                TradeNo = args.TradeNo,

            });
        }

        [HttpPost]
        public void TestRefundQuery([FromBody]RefundArgsModel args)
        {
            //  this.LogDbContext.Add(new TaskLog() { TaskName = "TestRefund", ExeResult = "" });
            //this.LogDbContext.SaveChanges();
            Refunds refund = new Refunds(proxyFactory);
            refund.RefundAction(new ZRui.Web.Core.Finance.WechatPay.PayAPIModels.RefundArgsModel()
            {
                ShopFlag = args.ShopFlag,
                TradeNo = args.TradeNo,

            });
        }

        [HttpGet]
        public void RefundQuery()
        {
            //  shoporderid is not null and shoporderid> 0 and `Status`= 1 and PayWay = 1
            foreach (var refund in finaceDb.memberTradeForRefunds.Where(p => p.Status == MemberTradeForRefundStatus.退款中))
            {


                var memberrechanges = finaceDb.MemberTradeForRechanges.FirstOrDefault(r => r.TradeNo == refund.TradeNo && r.Status == MemberTradeForRechangeStatus.成功);
                if (memberrechanges == null || memberrechanges.ShopId == 0)
                {
                    // throw new ArgumentNullException("未找到支付记录,请检查数据是否正确！");
                    continue;
                }

                ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
              .Where(m => !m.IsDel)
              .Where(m => m.ShopId == memberrechanges.ShopId && (int)m.PayWay == memberrechanges.PayWay)
              .FirstOrDefault();
                if (shopPayInfo == null)
                {
                    continue;
                } //throw new Exception("当前商铺没有设置好退款信息。");

                var payProxy = proxyFactory.GetProxy(shopPayInfo);

                var result = payProxy.GetRefundResult(refund) as SwiftpassPayResponseHandler;
                if (result.Status != 0)
                {
                    refund.Detail = result.Message;
                    refund.Status = MemberTradeForRefundStatus.失败;
                }
                else
                {
                    string returnCode = result.parameters["refund_status_0"].ToString();
                    if (returnCode == "SUCCESS")
                    {
                        refund.Status = MemberTradeForRefundStatus.成功;
                        refund.TimeExpire = DateTime.Now;
                    }
                    else if (returnCode == "FAIL")
                    {
                        refund.Detail = returnCode;
                        refund.Status = MemberTradeForRefundStatus.失败;
                    }
                    else if (result.parameters["refund_status_0"].ToString() != "PROCESSING")
                    {
                        refund.Detail = returnCode;
                        refund.Status = MemberTradeForRefundStatus.失败;
                    }
                }
                //update bussine order status

                if (refund.Status == MemberTradeForRefundStatus.成功)
                {
                    if (memberrechanges.OrderType == OrderType.普通订单)
                    {
                        var order = shopDb.ShopOrders.FirstOrDefault(p => p.Id == memberrechanges.OrderId);
                        if (order != null)
                        {
                            order.Status = ShopOrderStatus.已退款;
                        }
                    }
                    else if (memberrechanges.OrderType == OrderType.拼团订单)
                    {
                        var order = shopDb.ConglomerationOrder.FirstOrDefault(p => p.Id == memberrechanges.OrderId);
                        if (order != null)
                        {
                            order.Status = ShopOrderStatus.已退款;
                        }
                    }

                }

                shopDb.SaveChanges();
                finaceDb.SaveChanges();
            }
        }


        /// <summary>
        /// 余额消费
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BalanceConsume([FromBody]BalanceConsumeArgsModel args)
        {
            int memberId = GetMemberId();
            ShopOrder shopOrder = shopDb.GetSingle<ShopOrder>(args.OrderId);
            if (shopOrder == null) throw new Exception("订单不存在");
            var shopMemberServer = new ShopMemberServer(shopDb, shopOrder.ShopId, memberId);
            //if (!shopMemberServer.CheckPassword(args.Password)) throw new Exception("支付密码不正确");
            if (!shopMemberServer.BalanceConsume(shopOrder)) throw new Exception("当前余额不足");
            var checkThirdResult = CheckThird(shopOrder, shopOrder.ShopId);
            if (!checkThirdResult.Success)
            {
                throw new Exception(checkThirdResult.Message);
            }
            shopDb.SaveChanges();
            Utils.ShopOrderUtil.SetShopOrderFinishInBackgroup(shopOrder.Id);
            return Success();

        }
    }
}
