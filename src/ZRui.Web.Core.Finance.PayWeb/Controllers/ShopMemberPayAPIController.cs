using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Utils;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Finance.WechatPay.ShopMemberPayAPIModels;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class ShopMemberPayAPIController : WechatApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        readonly ILogger _logger;
        FinanceDbContext finaceDb;
        WechatTemplateSendOptions options;
        ShopDbContext shopDb;
        PayProxyFactory proxyFactory;
        HchhLogDbContext LogDbContext;
        public ShopMemberPayAPIController(IOptions<MemberAPIOptions> memberOptions
            , IOptions<WechatTemplateSendOptions> options
            , ShopDbContext shopDb
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

            this._logger = loggerFactory.CreateLogger<SwiftpassPayForShopWechatOpenAPIController>();
            this.proxyFactory = proxyFactory;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BeginRecharge([FromBody]BeginRechangeArgsModel args)
        {
            try
            {
                if (args.ShopFlag == null) throw new ArgumentNullException("ShopFlag");
                var memberId = GetMemberId();
                Shop shop = shopDb.Query<Shop>().FirstOrDefault(m => !m.IsDel && m.Flag == args.ShopFlag);
                ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopFlag == args.ShopFlag && m.IsEnable)
                    .FirstOrDefault();
                if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");

                int rechangeAmount, presentedAmount;
                if (args.RechargeType == RechargeType.固定金额)
                {
                    if (!args.TopUpId.HasValue) throw new Exception("TopUpId不能为空");
                    var topUpSet = shopDb.Query<ShopTopUpSet>()
                        .FirstOrDefault(m => !m.IsDel && m.Id == args.TopUpId.Value && m.ShopId == shop.Id);
                    if (topUpSet == null) throw new Exception("该商铺不存在此项充值");
                    rechangeAmount = topUpSet.FixationTopUpAmount;
                    presentedAmount = topUpSet.PresentedAmount;
                }
                else
                {
                    if (!args.Amount.HasValue) throw new Exception("充值金额不能为空");
                    rechangeAmount = args.Amount.Value;
                    var customTopUpSet = shopDb.Query<ShopCustomTopUpSet>()
                        .FirstOrDefault(m => !m.IsDel && m.ShopId == shop.Id);
                    if (customTopUpSet == null) throw new Exception("该商铺不存在此项充值");
                    if (args.Amount.Value < customTopUpSet.StartAmount)
                        throw new Exception("充值金额少于起充金额");
                    if (args.Amount.Value > customTopUpSet.MeetAmount)
                        presentedAmount = (int)((args.Amount.Value - customTopUpSet.MeetAmount) * (customTopUpSet.Additional / 100.00D));
                    else
                        presentedAmount = 0;
                }
                var shopMember = BLL.Servers.ShopMemberServer.GetShopMember(shopDb, shop.Id, memberId);
                var memberRecharge = new ShopMemberRecharge()
                {
                    Amount = rechangeAmount,
                    PresentedAmount = presentedAmount,
                    ShopMemberId = shopMember.Id,
                    TransactionTime = DateTime.Now,
                    Status = ShopMemberTransactionStatus.未完成
                };
                shopDb.Add(memberRecharge);
                shopDb.SaveChanges();
                PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
                var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
                var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                var model = new MemberTradeForRechange()
                {
                    AddIP = GetIp(),
                    AddTime = DateTime.Now,
                    AddUser = GetUsername(),
                    MemberId = memberId,
                    Detail = $"用户支付{rechangeAmount}分",
                    OutBank = "",
                    PayChannel = payProxy.PayChannel,
                    Status = MemberTradeForRechangeStatus.未完成,
                    TimeExpire = DateTime.Now,
                    TimeStart = DateTime.Now,
                    Title = "用户充值",
                    ShopOrderId = memberRecharge.Id,
                    TotalFee = rechangeAmount,
                    TradeNo = tradeNo,
                    PayWay = (int)shopPayInfo.PayWay,
                    RowVersion = DateTime.Now,
                    OrderType = OrderType.充值订单,
                    ConglomerationOrderId = 0,
                    ShopId = shopPayInfo.ShopId
                };
                finaceDb.AddToMemberTradeForRechange(model);
                var openId = GetOpenId();
                var payInfo = payProxy.GetPayInfo(model, openId);
                finaceDb.SaveChanges();
                if (shopPayInfo.PayWay == PayWay.Wechat)
                {
                    new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                        , BLL.Log.PayOrRefundType.支付, rechangeAmount, OrderType.普通订单, model, payInfo);
                    return Success(new
                    {
                        payInfo,
                        TradeNo = tradeNo
                    });
                }
                else
                {
                    var infoModel = new Core.Finance.WechatPay.PayAPIModels.PayInfoModel();
                    var payinfo = JsonConvert.DeserializeObject<ZRui.Web.Core.Finance.WechatPay.PayAPIModels.PayInfo>(payInfo.ToString());
                    infoModel.payInfo = payinfo;
                    //new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                    //     , BLL.Log.PayOrRefundType.支付, args.ShopOrderId.Value, OrderType.普通订单, model, payInfo);
                    return Success(
                        infoModel
                    );
                }

            }
            catch (Exception ex)
            {
                //new PayOrRefundUtil<object>(LogDbContext).PayAction("Pay"
                //        , BLL.Log.PayOrRefundType.支付, args.ShopOrderId.Value, OrderType.普通订单, null, null, ex.Message + "【StackTrace】" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

    }
}
