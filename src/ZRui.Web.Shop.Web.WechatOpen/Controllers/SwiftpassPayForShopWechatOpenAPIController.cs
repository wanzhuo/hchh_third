//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using ZRui.Web.Common;
//using ZRui.Web.Core.Finance.SwiftpassPay;
//using ZRui.Web.Core.Finance.SwiftpassPay.SwiftpassPayShopWechatOpenModels;
//using ZRui.Web.Core.Wechat;

//namespace ZRui.Web.Controllers
//{
//    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
//    [Route("api/[controller]/[action]")]
//    public class SwiftpassPayForShopWechatOpenAPIController : WechatApiControllerBase
//    {
//        readonly IHostingEnvironment hostingEnvironment;
//        private readonly ILogger _logger;
//        SwiftpassPayOptions options;
//        FinanceDbContext financeDb;
//        ShopDbContext db;
//        SwiftpassPayProxy payProxy;

//        public SwiftpassPayForShopWechatOpenAPIController(IOptions<SwiftpassPayOptions> options
//            , IOptions<MemberAPIOptions> memberOptions
//            , SwiftpassPayProxy payProxy
//            , ShopDbContext db
//            , FinanceDbContext financeDb
//            , WechatCoreDbContext wechatCoreDb
//            , MemberDbContext memberDb
//            , ILoggerFactory loggerFactory
//            , IHostingEnvironment hostingEnvironment) : base(memberOptions, memberDb, wechatCoreDb)
//        {
//            this.hostingEnvironment = hostingEnvironment;
//            this.options = options.Value;
//            this.financeDb = financeDb;
//            this.db = db;
//            this.payProxy = payProxy;
//            _logger = loggerFactory.CreateLogger<SwiftpassPayForWechatAPIController>();
//        }

//        [HttpPost]
//        [Authorize(AuthenticationSchemes = "jwt")]
//        public APIResult BeginRechange([FromBody]BeginRechangeArgsModel args)
//        {
//            if (args.TotalFee <= 0) throw new Exception("充值金额需要大于0");
//            var memberId = GetMemberId();
//            var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
//            var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
//            var model = new MemberTradeForRechange()
//            {
//                AddIP = GetIp(),
//                AddTime = DateTime.Now,
//                AddUser = GetUsername(),
//                MemberId = memberId,
//                Detail = "用户充值" + args.TotalFee + "分",
//                OutBank = "",
//                PayChannel = "Swiftpass",
//                Status = MemberTradeForRechangeStatus.未完成,
//                TimeExpire = DateTime.Now,
//                TimeStart = DateTime.Now,
//                Title = "充值",
//                TotalFee = args.TotalFee,
//                TradeNo = tradeNo
//            };
//            financeDb.AddToMemberTradeForRechange(model);
//            financeDb.SaveChanges();
//            var openId = GetOpenId();

//            //这里是调用ShopDB
//            var appId = db.Query<ShopWechatOpenAuthorizer>()
//                .Where(m => !m.IsDel)
//                .Where(m => m.Shop.Flag == args.ShopFlag)
//                .Select(m => m.WechatOpenAuthorizer.AuthorizerAppId)
//                .FirstOrDefault();
//            if (string.IsNullOrEmpty(appId)) throw new Exception("指定店铺的AppId不存在");

//            var payInfo = payProxy.GetPayInfo(model, openId, appId);
//            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(payInfo);
//            return Success(new
//            {
//                payInfo = obj,
//                TradeNo = tradeNo
//            });
//        }
//    }
//}
