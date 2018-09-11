using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using ZRui.Web.ShopCommodityComboAPIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ZRui.Web.BLL.Servers;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCommodityComboAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommodityComboAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取套餐详细内容
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetComboDetail([FromBody]GetComboDetailArgsModel args)
        {
            if (!args.CommodityId.HasValue) throw new Exception("CommodityId不能为空");

            List<ShopBrandCommodity> list = db.Query<ShopOrderComboItem>()
                .Where(m => !m.IsDel)
                .Where(m => m.Pid == args.CommodityId.Value)
                .Select(m => m.Commodity)
                .Distinct()
                .ToList();

            return Success(list);
        }


        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetComboList([FromBody]GetComboListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopId不能为空");
            if (!args.DiningWay.HasValue) throw new Exception("渠道不能为空");
            ShopBrandCommodityServer server = new ShopBrandCommodityServer(db, args.DiningWay.Value);
            return Success(server.GetComboList(args.ShopId.Value));
        }
    }
}
