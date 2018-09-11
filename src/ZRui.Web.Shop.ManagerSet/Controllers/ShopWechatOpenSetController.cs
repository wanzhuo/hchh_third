using System;
using System.Linq;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using System.Collections.Generic;
using Senparc.Weixin.Open.Containers;
using ZRui.Web.Core.Wechat;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Exceptions;
using System.IO;
using System.Text;
using ZRui.Web.Core.Wechat.ThirdPartyMessageHandlers;
using Senparc.Weixin.Open.Entities.Request;
using ZRui.Web.ShopManager.ShopWechatOpenSetModels;
using Microsoft.Extensions.Logging;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("[controller]/[action]")]
    public class ShopWechatOpenSetController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        ILogger logger;
        public ShopWechatOpenSetController(IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            logger = loggerFactory.CreateLogger<ShopWechatOpenSetController>();
        }

        [HttpGet]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult> GetQRcode(GetQRCodeArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            logger.LogInformation("GetAuthorizerAccessToken Begin");
            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
            logger.LogInformation("GetAuthorizerAccessToken End");
            using (var stream = new MemoryStream())
            {
                logger.LogInformation("GetQRCodeAsync Begin");
                var result = await Senparc.Weixin.Open.WxaAPIs.CodeApi.GetQRCodeAsync(authorizerAccessToken, stream);
                logger.LogInformation("GetQRCodeAsync End");
                if (result.errcode != Senparc.Weixin.ReturnCode.请求成功)
                {
                    throw new Exception($"{result.errcode}:{result.errmsg}");
                }

                byte[] data = stream.ToArray();

                return File(data, "image/jpeg");
            }
        }

        string GetAuthorizerAccessToken(int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId,
                    AuthorizerAccessToken = m.WechatOpenAuthorizer.AuthorizerAccessToken,
                    ExpiresTime = m.WechatOpenAuthorizer.ExpiresTime,
                })
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");
            //if (model.ExpiresTime.AddMinutes(20) > DateTime.Now) return model.AuthorizerAccessToken;

            //  var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken(wechatOpenOptions.AppId, model.AuthorizerAppId);
            var authorizerAccessToken = ZRui.Web.BLL.AuthorizerHelper.GetAuthorizerAccessToken(model.AuthorizerAppId);
            return authorizerAccessToken;
        }

    }
}
