using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin;
using ZRui.Web.Models;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 提供快创API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopWechatAPIController : ShopManagerApiControllerBase
    {
        ShopDbContext db;
        readonly WechatTemplateSendOptions wechatTemplateSendOptions;
        readonly IHostingEnvironment hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopWechatAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
        }


        /// <summary>
        /// 通过Code获取微信基本信息（含Openid）
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public APIResult<GetWechatUserInfoByCodeResultModel> GetWechatUserInfoByCode(string code)
        {
            var accessTokenResult = OAuthApi.GetAccessToken(wechatTemplateSendOptions.AppId, wechatTemplateSendOptions.AppSecret, code);
            if (accessTokenResult.errcode != ReturnCode.请求成功) throw new Exception("错误：" + accessTokenResult.errmsg);
            var oauthUserInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);

            return Success(new GetWechatUserInfoByCodeResultModel()
            {
                headimgurl = oauthUserInfo.headimgurl,
                nickname = oauthUserInfo.nickname,
                openid = oauthUserInfo.openid
            });
        }

    }
}
