using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.SwiftpassPay;
using ZRui.Web.Core.Finance.SwiftpassPay.SwiftpassPayWechatModels;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Models;
using static ZRui.Web.Models.SwiftpassPayModel;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class SwiftpassPayForWechatAPIController : WechatApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger _logger;
        SwiftpassPayOptions options;
        FinanceDbContext db;
        SwiftpassPayProxy payProxy;
        ShopDbContext _shopdb;


        public SwiftpassPayForWechatAPIController(IOptions<SwiftpassPayOptions> options
            , IOptions<MemberAPIOptions> memberOptions
            , SwiftpassPayProxy payProxy
            , FinanceDbContext db
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory
            , ShopDbContext shopdb
            , IHostingEnvironment hostingEnvironment) : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.options = options.Value;
            this.db = db;
            this.wechatCoreDb = wechatCoreDb;
            this.memberDb = memberDb;
            this.payProxy = payProxy;
            this._shopdb = shopdb;
            _logger = loggerFactory.CreateLogger<SwiftpassPayForWechatAPIController>();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BeginRechange([FromBody]BeginRechangeArgsModel args)
        {
            if (args.TotalFee <= 0) throw new Exception("充值金额需要大于0");
            var memberId = GetMemberId();
            var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var model = new MemberTradeForRechange()
            {
                AddIP = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                MemberId = memberId,
                Detail = "用户充值" + args.TotalFee + "分",
                OutBank = "",
                PayChannel = "宝付",
                Status = MemberTradeForRechangeStatus.未完成,
                TimeExpire = DateTime.Now,
                TimeStart = DateTime.Now,
                Title = "用户充值",
                TotalFee = args.TotalFee,
                TradeNo = tradeNo
            };
            db.AddToMemberTradeForRechange(model);
            db.SaveChanges();
            var openId = GetOpenId();

            var payInfo = payProxy.GetPayInfo(model, openId, options.DefaultAppId);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(payInfo);
            return Success(new
            {
                payInfo = obj,
                TradeNo = tradeNo
            });
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
    //    public APIResult Pay([FromBody]SwiftpassPayModel args)
    //    {
    //        var order = _shopdb.ShopOrders.FirstOrDefault(r => r.Id == args.OrderId);
    //        ShopPayInfo shopPayInfo = _shopdb.Query<ShopPayInfo>()
    //.Where(m => !m.IsDel)
    //.Where(m => m.ShopFlag == args.ShopFlag)
    //.FirstOrDefault();
    //        if (shopPayInfo == null) throw new Exception("当前商铺没有设置好支付信息。");
    //        if (order == null)
    //        {
    //            return Error("订单不存在");
    //        }

    //        int payamount = order.Amount * 100;
    //        var memberId = GetMemberId();
    //        var tradeNo = "SP" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
    //        //var tradeDate = DateTime.Now.ToString("yyyyMMddHHmmss");
    //        var model = new MemberTradeForRechange()
    //        {
    //            AddIP = GetIp(),
    //            AddTime = DateTime.Now,
    //            AddUser = GetUsername(),
    //            MemberId = memberId,
    //            Detail = $"用户支付{payamount}分",
    //            OutBank = "",
    //            PayChannel = "中信支付",
    //            Status = MemberTradeForRechangeStatus.未完成,
    //            TimeExpire = DateTime.Now,
    //            TimeStart = DateTime.Now,
    //            Title = "用户支付",
    //            TotalFee = payamount,
    //            TradeNo = tradeNo
    //        };
    //        db.AddToMemberTradeForRechange(model);
    //        db.SaveChanges();
    //        var openId = GetOpenId();

    //        var payInfo = payProxy.GetPayInfo(shopPayInfo, model, "o5pAb5B8cxRsewJBqJdsoWnrqdtQ");
    //        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(payInfo);
    //        return Success(new
    //        {
    //            payInfo = obj,
    //            TradeNo = tradeNo
    //        });
    //    }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetRechangeResult([FromBody]GetRechangeResultArgsModel args)
        {
            if (string.IsNullOrEmpty(args.TradeNo)) throw new ArgumentNullException("TradeNo");

            var memberId = GetMemberId();
            var rechange = db.Query<MemberTradeForRechange>()
                .Where(m => m.MemberId == memberId && m.TradeNo == args.TradeNo)
                .FirstOrDefault();
            if (rechange == null) throw new Exception("指定的纪录不存在");
            _logger.LogInformation($"获取到rechange{rechange.Id},状态 {rechange.Status}");

            if (rechange.Status == MemberTradeForRechangeStatus.未完成)
            {
                var result = payProxy.GetPayResult(rechange);
                _logger.LogInformation($"获取到result{result.Xml}");
                //rechange.SetFinish(db, result);
                db.SaveChanges();
            }
            return Success(rechange);
        }
    }
}
