using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Utils;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Finance.WechatPay.PayAPIModels;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Pay;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class SwiftpassPayForConglomerationWechatOpenAPIController : WechatApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        readonly ILogger _logger;
        FinanceDbContext db;
        WechatTemplateSendOptions options;
        ShopDbContext shopDb;
        PayProxyFactory proxyFactory;
        HchhLogDbContext LogDbContext;

        public SwiftpassPayForConglomerationWechatOpenAPIController(IOptions<MemberAPIOptions> memberOptions
            , FinanceDbContext db
            , IOptions<WechatTemplateSendOptions> options
            , ShopDbContext shopDb
            , PayProxyFactory proxyFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment) : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.db = db;
            this.options = options.Value;
            this.shopDb = shopDb;
            this.wechatCoreDb = wechatCoreDb;
            this.memberDb = memberDb;
            this._logger = loggerFactory.CreateLogger<SwiftpassPayForShopWechatOpenAPIController>();
            this.proxyFactory = proxyFactory;
            this.LogDbContext = DbContextFactory.LogDbContext;
        }

        /// <summary>
        /// 获取拼团订单支付信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BeginRechange([FromBody]BeginRechangeArgsModel args)
        {

            //if (args.TotalFee <= 0) throw new Exception("充值金额需要大于0");
            if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
            if (!args.ConglomerationOrderId.HasValue) throw new ArgumentNullException("ConglomerationOrderId");
            var memberId = GetMemberId();
            ConglomerationOrder conglomerationOrder = shopDb.GetSingle<ConglomerationOrder>(args.ConglomerationOrderId.Value);
            if (conglomerationOrder == null) throw new Exception("订单不存在");
            ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopFlag == args.ShopFlag)
                .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
                .FirstOrDefault();
            if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");
            if (shopPayInfo.PayWay == PayWay.Wechat)
            {
                throw new Exception("拼团功能暂不支持，请联系商户修改支付模式");
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
                Detail = "拼团订单支付" + conglomerationOrder.Payment.Value + "分",
                OutBank = "",
                PayChannel = payProxy.PayChannel,
                Status = MemberTradeForRechangeStatus.未完成,
                TimeExpire = DateTime.Now,
                TimeStart = DateTime.Now,
                Title = "拼团订单支付",
                TotalFee = conglomerationOrder.Payment.Value,
                ConglomerationOrderId = args.ConglomerationOrderId.Value,
                MoneyOffRuleId = 0,
                TradeNo = tradeNo,
                OrderType = OrderType.拼团订单,
                OrderId = args.ConglomerationOrderId.Value,
                ShopOrderId = 0,
                PayWay = (int)shopPayInfo.PayWay,
                ShopId = shopPayInfo.ShopId


            };
            db.AddToMemberTradeForRechange(model);
            var openId = GetOpenId();
            var payInfo = payProxy.GetPayInfo(model, openId);
            db.SaveChanges();
            if (shopPayInfo.PayWay == PayWay.Wechat)
            {
                new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                      , BLL.Log.PayOrRefundType.支付, args.ConglomerationOrderId.Value, OrderType.拼团订单, model, payInfo);
                return Success(new
                {
                    payInfo,
                    TradeNo = tradeNo
                });
            }
            else
            {
                PayInfoModel infoModel = new PayInfoModel();
                var payinfo = JsonConvert.DeserializeObject<PayInfo>(payInfo.ToString());
                infoModel.payInfo = payinfo;
                new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                    , BLL.Log.PayOrRefundType.支付, args.ConglomerationOrderId.Value, OrderType.拼团订单, model, payInfo);
                return Success(
                    infoModel
                );
            }
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
            var rechange = db.Query<MemberTradeForRechange>()
                .Where(m => m.MemberId == memberId && m.TradeNo == args.TradeNo)
                .FirstOrDefault();
            if (rechange == null) throw new Exception("指定的纪录不存在");
            _logger.LogInformation($"获取到rechange{rechange.Id},状态 {rechange.Status}");

            if (rechange.Status == MemberTradeForRechangeStatus.未完成)
            {
                var result = payProxy.GetPayResult(rechange);
                _logger.LogInformation($"获取到result{result.Xml}");
                //rechange.SetFinish(shopDb,db,options, result,_logger);
                db.SaveChanges();
            }
            return Success(rechange);
        }





        /// <summary>
        /// 更新拼团状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public APIResult UpdateConglomeration()
        {

            _logger.LogInformation("=================【更新拼团状态】UpdateConglomeration服务开始==========");
            var orders = shopDb.ConglomerationOrder.Where(m => !m.IsDel &&
            (m.Status.Equals(ShopOrderStatus.待成团) ||
            m.Status.Equals(ShopOrderStatus.待自提) ||
            m.Status.Equals(ShopOrderStatus.待配送)) &&
           (!m.ConglomerationSetUp.IsDel && m.ConglomerationSetUp.EndTime <= DateTime.Now && (m.ConglomerationSetUp.Status.Equals(ConglomerationSetUpStatus.未成团) || m.ConglomerationSetUp.Status.Equals(ConglomerationSetUpStatus.已取消))))
           .Include(m => m.ConglomerationSetUp);

            _logger.LogInformation($"需更新订单数量 :{orders.Count()}");

            foreach (var ordersItem in orders)
            {
                try
                {
                    //执行退款改变订单状态
                    var shop = shopDb.Shops.Find(ordersItem.ShopId);
                    Refunds refunds = new Refunds(proxyFactory);
                    var memberTradeForRechange = shopDb.MemberTradeForRechange.FirstOrDefault(m => m.Status.Equals(MemberTradeForRechangeStatus.成功) && m.ConglomerationOrderId.Equals(ordersItem.Id));
                    _logger.LogInformation($"支付记录ID :{memberTradeForRechange.Id}");

                    if (memberTradeForRechange == null)
                    {
                        _logger.LogInformation($"错误无法找到支付记录 订单ID :{ordersItem.Id}");
                        return Error("无法找到支付记录");
                    }
                    var isOk = refunds.RefundAction(new RefundArgsModel() { ShopFlag = shop.Flag, TradeNo = memberTradeForRechange.TradeNo });
                    _logger.LogInformation($"退款结果 isOk :{isOk.Status}");

                    if (isOk.Status == MemberTradeForRefundStatus.成功)
                    {
                        ordersItem.Status = ShopOrderStatus.已退款;
                    }
                    if (isOk.Status == MemberTradeForRefundStatus.退款中)
                    {
                        ordersItem.Status = ShopOrderStatus.退款中;

                    }
                    ordersItem.ConglomerationSetUp.Status = ConglomerationSetUpStatus.已取消;
                    ShopConglomerationActivityOptions.RemoveSetup(ordersItem.ConglomerationSetUp.Id);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"更新出错错误信息{e.Message}    订单ID :{ordersItem.Id}");
                }
            }
            shopDb.SaveChanges();
            _logger.LogInformation("=================【更新拼团状态】UpdateConglomeration服务结束==========");

            return Success("ok");
        }


    }

}
