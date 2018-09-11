using System;
using System.Linq;
using ZRui.Web.ShopPartAPIModels;
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
    [Route("api/[controller]/[action]")]
    public class ShopPartAPIController : WechatApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        ShopDbContext db;
        public ShopPartAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            var query = db.Query<ShopPart>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.Shop.Flag == args.ShopFlag);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Detail = m.Detail,
                    Flag = m.Flag,
                    Id = m.Id,
                    ShopId = m.ShopId,
                    Title = m.Title
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");

            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;

            var query = db.Query<ShopPart>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Shop.Flag == args.ShopFlag);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Detail = m.Detail,
                    Flag = m.Flag,
                    Id = m.Id,
                    ShopId = m.ShopId,
                    Title = m.Title
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.ToList()
            });

        }
    }
}
