using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopWechatOpenAuthorizerSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using Senparc.Weixin.Open.Containers;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.Open;
using System.IO;
using System.Collections.Generic;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopWechatOpenAuthorizerSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        public ShopWechatOpenAuthorizerSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
        }

        /// <summary>
        /// 通过店铺Id获得店铺绑定的授权用户
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<ShopWechatOpenAuthorizer> GetSingleByShopId([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");

            return Success(model);
        }
    }
}
