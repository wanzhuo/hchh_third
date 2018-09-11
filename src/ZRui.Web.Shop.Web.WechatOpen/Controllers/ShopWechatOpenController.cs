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
using Senparc.Weixin.Open.Entities.Request;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Senparc.Weixin;
using Microsoft.Extensions.Logging;
using ZRui.Web.ShopWechatOpenApiModel;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Shop.Web.WechatOpen.Controllers
{
    /// <summary>
    /// 用于接受微信的推送
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("[controller]/[action]")]
    public class ShopWechatOpenController : WechatApiControllerBase
    {
        private readonly ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        ShopDbContext db;

        public ShopWechatOpenController(IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , WechatCoreDbContext wechatCoreDb
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment) : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            this.db = db;
            _logger = loggerFactory.CreateLogger<ShopWechatOpenController>();

        }

        Senparc.Weixin.Open.WxaAPIs.Sns.JsCode2JsonResult getJsCode2Json(string appId, string code)
        {
            if (code == "test")
            {
                return new Senparc.Weixin.Open.WxaAPIs.Sns.JsCode2JsonResult()
                {
                    openid = "oNtLks6Gq9oh6nEa8vSUjoc-cvvM"
                };
            }

            //    var accessToken =  ComponentContainer.TryGetComponentAccessToken(wechatOpenOptions.AppId, wechatOpenOptions.AppSecret);
            var accessToken = ZRui.Web.BLL.AuthorizerHelper.GetComponentAccessToken();

            _logger.LogInformation("Login时的accessToken:{0}", accessToken);

            var openIdResult = Senparc.Weixin.Open.WxaAPIs.Sns.SnsApi.JsCode2Json(appId, wechatOpenOptions.AppId, accessToken, code);
            if (openIdResult.errcode != ReturnCode.请求成功) throw new Exception("错误：" + openIdResult.errmsg);
            return openIdResult;
        }



        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="shopFlag">商铺的标识</param>
        /// <param name="code">微信code</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string shopFlag, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

                var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Shop.Flag == shopFlag)
                    .Select(m => new
                    {
                        AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                    })
                    .FirstOrDefault();


                //return Content(authorizer.AuthorizerAppId + "," + code);

                var jsCode2Json = getJsCode2Json(authorizer.AuthorizerAppId, code);

                //_logger.LogInformation("Login时，jsCode2Json:{0}", jsCode2Json);

                //判定是否已经绑定手机号
                int? memberId = null;
                var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == jsCode2Json.openid)
                    .FirstOrDefault();

                if (customerPhone != null)
                {
                    //var email = $"{customerPhone.Phone}@phone";
                    //memberId = base.memberDb.Query<Member>()
                    //    .Where(m => !m.IsDel)
                    //    .Where(m => m.Email == email)
                    //    .Select(m => m.Id)
                    //    .FirstOrDefault();

                    //获取memberId的过程，改为使用memberPhone
                    memberId = memberDb.GetMemberIdByMemberPhone(customerPhone.Phone);

                    if (memberId <= 0) throw new Exception("有错误发生,没有找到Member，请重新绑定手机号");
                }


                var jwt = new MemberLogin()
                {
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Wechat",
                    MemberId = memberId
                };
                jwt.SetloginSettingValue("shopFlag", shopFlag);
                jwt.SetloginSettingValue("openId", jsCode2Json.openid);
                jwt.SetloginSettingValue("sessionKey", jsCode2Json.session_key);
                db.Add<MemberLogin>(jwt);

                var expiration = TimeSpan.FromDays(1000);
                var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);
                db.SaveChanges();

                Member member = null;
                if (memberId.HasValue) member = memberDb.MemberDbSet().Find(memberId.Value);
                bool hasUserInfo = member != null && !string.IsNullOrEmpty(member.Avatar);

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)expiration.TotalSeconds,
                    isBindPhone = jwt.MemberId.HasValue,
                    hasUserInfo,
                    phone = customerPhone != null ? customerPhone.Phone : ""
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("登陆失败：{0}", ex.StackTrace);
                return Json(new { error = ex.Message + ex.StackTrace });
            }
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoginForNew(string shopFlag, string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

            var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.Shop.Flag == shopFlag)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();


            //return Content(authorizer.AuthorizerAppId + "," + code);

            var jsCode2Json = getJsCode2Json(authorizer.AuthorizerAppId, code);

            //_logger.LogInformation("Login时，jsCode2Json:{0}", jsCode2Json);

            //判定是否已经绑定手机号
            int? memberId = null;
            var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                .Where(m => !m.IsDel)
                .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == jsCode2Json.openid)
                .FirstOrDefault();

            var jwt = new MemberLogin()
            {
                Flag = CommonUtil.CreateNoncestr(18),
                LoginType = "Wechat",
                MemberId = memberId
            };
            jwt.SetloginSettingValue("shopFlag", shopFlag);
            jwt.SetloginSettingValue("openId", jsCode2Json.openid);
            jwt.SetloginSettingValue("sessionKey", jsCode2Json.session_key);
            db.Add<MemberLogin>(jwt);

            var expiration = TimeSpan.FromDays(1000);
            var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);
            db.SaveChanges();

            Member member = null;
            if (memberId.HasValue) member = memberDb.MemberDbSet().Find(memberId.Value);
            bool hasUserInfo = member != null && !string.IsNullOrEmpty(member.Avatar);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)expiration.TotalSeconds,
                isBindPhone = jwt.MemberId.HasValue,
                hasUserInfo,
                phone = customerPhone != null ? customerPhone.Phone : ""
            };

            return Json(response);
        }

        /// <summary>
        /// 删除登陆信息
        /// </summary>
        /// <param name="shopFlag">商铺的标识</param>
        /// <param name="code">微信code</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult DelLoginInfo()
        {
            int memberId = GetMemberId();
            string openId = GetOpenId();

            var member = memberDb.Query<Member>()
                .Where(m => m.Id == memberId)
                .FirstOrDefault();

            wechatCoreDb.Query<MemberLogin>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId || m.Flag==User.Identity.Name)
                .ToList()
                .ForEach(m => m.IsDel = true);

            member.IsDel = true;

            wechatCoreDb.SaveChanges();
            memberDb.SaveChanges();
            return Success();
        }

        [HttpPost]
        public APIResult LoginHchh([FromBody] LoginHchhApiModel apiArgs)
        {
            var jwt = db.MemberLogins.FirstOrDefault(p => !p.IsDel && p.ShopFlag == apiArgs.shopFlag && p.OpenId == apiArgs.openid);
            if (jwt == null)
            {
                throw new Exception("user is not  login");
            }

            ///根据sessionkey 解密userinfo
            var json = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeEncryptedData(jwt.SessionKey, apiArgs.encryptedData, apiArgs.iv);
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<Senparc.Weixin.WxOpen.Entities.DecodedUserInfo>(json);
            if (string.IsNullOrEmpty(args.unionId))
                throw new Exception("获取unionId失败");
            //查询数据库是否存在此member,没有则创建Member
            var member = db.Member.FirstOrDefault(p => p.UnionId == args.unionId && !p.IsDel);
            if (member == null)
            {
                member = new Member()
                {
                    Truename = "WechatUser",
                    Avatar = args.avatarUrl,
                    NickName = args.nickName,
                    RegTime = DateTime.Now,
                    Status = MemberStatus.正常,
                    UnionId = args.unionId

                };
                db.Add(member);
                db.SaveChanges();
            }
            jwt.MemberId = member.Id;
            db.SaveChanges();

            var expiration = TimeSpan.FromDays(365*10);
            var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);

            var response = new
            {
                access_token = encodedJwt
            };
            return Success(response);
        }

        [HttpGet]
        public APIResult LoginWx(string shopFlag, string code)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(shopFlag))
                throw new ArgumentNullException("args is null");

            var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.Shop.Flag == shopFlag)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();

            var jsCode2Json = getJsCode2Json(authorizer.AuthorizerAppId, code);
            if (string.IsNullOrEmpty(jsCode2Json.openid))
                throw new Exception("未能获取openId");
            var jwt = db.MemberLogins.FirstOrDefault(p => !p.IsDel && p.OpenId == jsCode2Json.openid && p.ShopFlag == shopFlag);
            if (jwt == null)
            {
                jwt = new MemberLogin()
                {
                    SessionKey = jsCode2Json.session_key,
                    ShopFlag = shopFlag,
                    OpenId = jsCode2Json.openid,
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Wechat"
                };
                db.Add(jwt);
            }
            jwt.SessionKey = jsCode2Json.session_key;

            jwt.SetloginSettingValue("shopFlag", shopFlag);
            jwt.SetloginSettingValue("openId", jsCode2Json.openid);
            jwt.SetloginSettingValue("sessionKey", jsCode2Json.session_key);

            db.SaveChanges();
            if (string.IsNullOrEmpty(jsCode2Json.openid))
            {
                return Error("获取openId失败");
            }
            string token = null;
            //若有memberId则返回token
            if (jwt.MemberId.HasValue)
            {
                var expiration = TimeSpan.FromDays(365 * 10);
                token = MemberLogin.CreateJwtToken(jwt, expiration);
            }
            var response = new
            {
                jsCode2Json.openid,
                token
            };
            return Success(response);
        }

        [HttpPost]
        public APIResult LoginByWxAuth([FromBody]LoginByWxAuthApiModel apiArgs)
        {
            if (string.IsNullOrWhiteSpace(apiArgs.code) || string.IsNullOrWhiteSpace(apiArgs.shopFlag))
                throw new ArgumentNullException("args is null");

            var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.Shop.Flag == apiArgs.shopFlag)
                .Select(m => new
                {
                    m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();

            var jsCode2Json = getJsCode2Json(authorizer.AuthorizerAppId, apiArgs.code);

            ///根据sessionkey 解密userinfo
            var json = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeEncryptedData(jsCode2Json.session_key, apiArgs.encryptedData, apiArgs.iv);
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<Senparc.Weixin.WxOpen.Entities.DecodedUserInfo>(json);

            //查询数据库是否存在此member,没有则创建Member
            var member = memberDb.Query<Member>().FirstOrDefault(p => p.UnionId == args.unionId);
            if (member == null)
            {
                member = new Member()
                {
                    Truename = "WechatUser",
                    Avatar = args.avatarUrl,
                    NickName = args.nickName,
                    RegTime = DateTime.Now,
                    Status = MemberStatus.正常,
                    UnionId = args.unionId

                };
                memberDb.Add(member);
                memberDb.SaveChanges();
            }
            var jwt = db.MemberLogins.FirstOrDefault(p => !p.IsDel && p.OpenId == jsCode2Json.openid && p.ShopFlag == apiArgs.shopFlag);
            if (jwt == null)
            {
                jwt = new MemberLogin()
                {
                    ShopFlag = apiArgs.shopFlag,
                    OpenId = jsCode2Json.openid,
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Wechat",
                    MemberId = member.Id
                };
                db.Add(jwt);
            }
            jwt.SessionKey = jsCode2Json.session_key;
            jwt.SetloginSettingValue("shopFlag", apiArgs.shopFlag);
            jwt.SetloginSettingValue("openId", jsCode2Json.openid);
            jwt.SetloginSettingValue("sessionKey", jsCode2Json.session_key);
            db.SaveChanges();

            var expiration = TimeSpan.FromDays(3650);
            var encodedJwt = MemberLogin.CreateJwtToken(jwt, expiration);
            var response = new
            {
                access_token = encodedJwt
            };
            return Success(response);
        }

    }
}
