using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopPartSetAPIModels;
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
    public class ShopPartSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        public ShopPartSetAPIController(ICommunityService communityService
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

        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopPart>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Detail = m.Detail,
                    Flag = m.Flag,
                    Id = m.Id,
                    IsDel = m.IsDel,
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
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;

            var query = db.Query<ShopPart>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Detail = m.Detail,
                    Flag = m.Flag,
                    Id = m.Id,
                    IsDel = m.IsDel,
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


        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Title)) throw new ArgumentNullException("title");
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var shopId = args.ShopId.Value;
            CheckShopActor(shopId, ShopActorType.超级管理员);

            var shop = db.GetSingle<Shop>(shopId);
            if (shop == null) throw new Exception("店铺纪录不存在");

            var model = new ShopPart()
            {
                Title = args.Title,
                Detail = args.Detail,
                Flag = System.Guid.NewGuid().ToString(),
                Shop = shop,
                IsDel = false,
                AddUser = GetUsername(),
                AddIp = GetIp(),
                AddTime = DateTime.Now
            };
            db.Add<ShopPart>(model);
            db.SaveChanges();
            return Success<int>(model.Id);
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var model = db.Query<ShopPart>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            //在获取后检查是否拥有管理权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.Title = args.Title;
            model.Detail = args.Detail;
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopPart>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);


            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 尝试初始化二维码的跳转
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult InitQRCodeJump([FromBody]InitQRCodeJumpArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var host = _options.Host.Trim('/');
            var url = $"{host}/qrcodeJump/{args.ShopId}/shopPart/";


            var getResult = CodeApiExt.QRCodeJumpGet(authorizerAccessToken);
            //说明没有对应的规则
            if (getResult.rule_list.Where(m => m.prefix == url).Count() <= 0)
            {
                //先下载验证文件
                var prefix = new Uri(url);
                var fileInfo = CodeApiExt.QRCodeJumpDownload(authorizerAccessToken);

                var filePath = hostingEnvironment.MapWebPath(Path.Combine(prefix.PathAndQuery, fileInfo.file_name));
                Common.FileUtils.CreateDirectory(filePath);
                //System.IO.File.Create(filePath);
                System.IO.File.WriteAllText(filePath, fileInfo.file_content);

                if (args.DebugUrl == null) args.DebugUrl = new List<string>();
                if (args.DebugUrl.Count <= 0) args.DebugUrl.Add($"{url}abcde");

                if (string.IsNullOrEmpty(args.OpenVersion)) args.OpenVersion = "2";
                if (string.IsNullOrEmpty(args.Path)) args.Path = "pages/order/home";

                var permitSubRule = "1";

                var result = CodeApiExt.QRCodeJumpAdd(authorizerAccessToken, url, permitSubRule, args.Path, args.OpenVersion, args.DebugUrl.ToArray(), false);
                return Success(result);
            }
            return Success();
        }

        /// <summary>
        /// 二维码跳转图片显示
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetQRCodeJumpUrl([FromBody]IdArgsModel args)
        {
            var model = db.Query<ShopPart>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.Id)
                .Select(m => new
                {
                    ShopId = m.ShopId,
                    ShopPartFlag = m.Flag
                })
                .FirstOrDefault();

            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            var host = _options.Host.Trim('/');
            var url = $"{host}/qrcodeJump/{model.ShopId}/shopPart/{model.ShopPartFlag}";
            return Success(new { url = url });
        }

        string GetAuthorizerAccessToken(int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");

            return GetAuthorizerAccessToken(model.AuthorizerAppId);
        }

        string GetAuthorizerAccessToken(string authorizerAppId)
        {
            var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken(wechatOpenOptions.AppId, authorizerAppId);
            return authorizerAccessToken;
        }
    }
}
