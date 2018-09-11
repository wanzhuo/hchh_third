using System;
using System.Linq;
using ZRui.Web.ShopOrderMoneyOffAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.BLL.ShopDbContextExtension;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopOrderMoneyOffAPIController : WechatApiControllerBase
    {
        static object lockAddObject = new object();
        ShopDbContext db;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopOrderMoneyOffAPIController(
            IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }

        /// <summary>
        /// 获取我的订单满减
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetRule([FromBody]GetRuleArgsModel args)
        {
            if (string.IsNullOrEmpty(args.shopFlag)) throw new Exception("shopFlag不能为空");
            if (!args.Amount.HasValue) throw new Exception("ShopOrderId不能为空");
            if (!args.moneyOffType.HasValue) args.moneyOffType = MoneyOffType.堂食;

            var memberId = GetMemberId();

            var rule = db.GetMoneyOffRule(args.shopFlag, args.Amount.Value, args.moneyOffType.Value);

            if (rule == null) return Success();

            return Success(new 
            {
                rule.Id,
                Discount = rule.Discount / 100d,
                FullAmount = rule.FullAmount / 100d
            });

        }


        /// <summary>
        /// 获取商店订单满减
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetShopRule([FromBody]GetShopRuleArgsModel args)
        {
            if (string.IsNullOrEmpty(args.shopFlag)) throw new Exception("shopFlag不能为空");
            if (!args.moneyOffType.HasValue) args.moneyOffType = MoneyOffType.堂食;

            var shop = db.Set<Shop>()
                .Where(m=>!m.IsDel)
                .Where(m => m.Flag == args.shopFlag)
                .FirstOrDefault();

            if (shop == null) throw new Exception("商铺不存在");

            var cache = db.GetMoneyOffRuleCache(shop.Id, args.moneyOffType.Value);

            if (cache == null) return Success();

            var list = db.Query<ShopOrderMoneyOffRule>()
                .Where(m => !m.IsDel)
                .Where(m => m.MoneyOffId == cache.MoneyOffId)
                .Select(m => new GetShopRule()
                {
                    Discount = m.Discount / 100m,
                    FullAmount = m.FullAmount / 100m
                })
                .ToList();

            return Success(list);

        }

    }
}
