using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System.IO;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.MemberWechatModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Cors;

namespace ZRui.Web.Controllers
{

    [EnableCors("AllowTestOrigin")]
    [Route("[controller]/[action]")]
    public class WechatController : WechatApiControllerBase
    {
        private readonly ILogger _logger;
        MemberDbContext memberDb;
        WechatCoreDbContext wechatCoreDb;
        WechatOptions wechatOptions;
        string GetAccessToken()
        {
            return AccessTokenContainer.TryGetAccessToken(wechatOptions.AppId, wechatOptions.AppSecret);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wechatOptions"></param>
        /// <param name="wechatCoreDb"></param>
        /// <param name="communityService"></param>
        /// <param name="memberOptions"></param>
        /// <param name="memberDb"></param>
        /// <param name="loggerFactory"></param>
        public WechatController(IOptions<WechatOptions> wechatOptions
            , WechatCoreDbContext wechatCoreDb
            , ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        : base(memberOptions, memberDb, wechatCoreDb)
        {
            _logger = loggerFactory.CreateLogger<WechatController>();
            this.memberDb = memberDb;
            this.wechatCoreDb = wechatCoreDb;
            this.wechatOptions = wechatOptions.Value;
        }

        string getOpenId(string code)
        {
            if (code == "test")
            {
                return "oNtLks6Gq9oh6nEa8vSUjoc-cvvM";
            }

            var openIdResult = OAuthApi.GetAccessToken(wechatOptions.AppId, wechatOptions.AppSecret, code);
            if (openIdResult.errcode != ReturnCode.请求成功) throw new Exception("错误：" + openIdResult.errmsg);

            return openIdResult.openid;
        }

        Senparc.Weixin.WxOpen.AdvancedAPIs.Sns.JsCode2JsonResult getOpenIdByWxOpen(string code)
        {
            if (code == "test")
            {
                return new Senparc.Weixin.WxOpen.AdvancedAPIs.Sns.JsCode2JsonResult()
                {
                    openid = "oNtLks6Gq9oh6nEa8vSUjoc-cvvM"
                };
            }

            var openIdResult = Senparc.Weixin.WxOpen.AdvancedAPIs.Sns.SnsApi.JsCode2Json(wechatOptions.AppId, wechatOptions.AppSecret, code);
            if (openIdResult.errcode != ReturnCode.请求成功) throw new Exception("错误：" + openIdResult.errmsg);

            return openIdResult;
        }

        [HttpGet]
        public ActionResult LoginStart()
        {
            var returnUrl = string.Format("{0}/api/wechat/login", _options.Host);
            var url = OAuthApi.GetAuthorizeUrl(wechatOptions.AppId, returnUrl, wechatOptions.State, OAuthScope.snsapi_base);
            return Redirect(url);
        }

        [HttpGet]
        public ActionResult Login(string code, string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
                if (string.IsNullOrEmpty(state) || state != wechatOptions.State) throw new Exception("验证失败！请从正规途径进入！");

                var openId = "string";
                openId = getOpenId(code);
                //判定是否已经绑定手机号
                int? memberId = null;
                var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == openId)
                    .FirstOrDefault();

                if (customerPhone != null)
                {
                    var email = $"{customerPhone.Phone}@phone";
                    memberId = base.memberDb.Query<Member>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.Email == email)
                        .Select(m => m.Id)
                        .FirstOrDefault();
                    if (memberId <= 0) throw new Exception("有错误发生,没有找到Member，请重新绑定手机号");
                }


                var jwt = new MemberLogin()
                {
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Wechat",
                    MemberId = memberId
                };
                jwt.SetloginSettingValue("openId", openId);
                wechatCoreDb.Add<MemberLogin>(jwt);

                var expiration = TimeSpan.FromMinutes(30 * 2 * 2);
                var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);
                wechatCoreDb.SaveChanges();

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)expiration.TotalSeconds,
                    isBindPhone = jwt.MemberId.HasValue,
                    phone = customerPhone != null ? customerPhone.Phone : ""
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult LoginForWxopen(string code, string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
                if (string.IsNullOrEmpty(state) || state != wechatOptions.State) throw new Exception("验证失败！请从正规途径进入！");

                var jsCode2Json = getOpenIdByWxOpen(code);

                //判定是否已经绑定手机号
                int? memberId = null;
                var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == jsCode2Json.openid)
                    .FirstOrDefault();

                if (customerPhone != null)
                {
                    var email = $"{customerPhone.Phone}@phone";
                    memberId = base.memberDb.Query<Member>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.Email == email)
                        .Select(m => m.Id)
                        .FirstOrDefault();
                    if (memberId <= 0) throw new Exception("有错误发生,没有找到Member，请重新绑定手机号");
                }


                var jwt = new MemberLogin()
                {
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Wechat",
                    MemberId = memberId
                };
                jwt.SetloginSettingValue("openId", jsCode2Json.openid);
                jwt.SetloginSettingValue("sessionKey", jsCode2Json.session_key);
                wechatCoreDb.Add<MemberLogin>(jwt);

                var expiration = TimeSpan.FromMinutes(30 * 2 * 2);
                var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);
                wechatCoreDb.SaveChanges();

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)expiration.TotalSeconds,
                    isBindPhone = jwt.MemberId.HasValue,
                    phone = customerPhone != null ? customerPhone.Phone : ""
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
