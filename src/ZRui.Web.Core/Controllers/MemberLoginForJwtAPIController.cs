using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZRui.Web.MemberLoginForJwtAPIModels;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using ZRui.Web.Core;
using Microsoft.Extensions.Options;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberLoginForJwtAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        public MemberLoginForJwtAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<MemberSetAPIController>();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoginBySms(string phone, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(phone)) throw new ArgumentNullException("phone");
                if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

                //通过用户名和用户Id获取和判断
                var memberId = memberDb.GetMemberIdByMemberPhone(phone);

                if (memberId <= 0)
                    throw new Exception(string.Format("手机:{0}没有绑定会员", phone));

                memberDb.SetMemberSMSValiCodeTaskFinished(phone, code, "Login");

                var memberLogin = new MemberLogin()
                {
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = "Jwt",
                    MemberId = memberId
                };
                memberDb.Add<MemberLogin>(memberLogin);

                var expiration = TimeSpan.FromMinutes(30 * 2 * 2);
                var encodedJwt = MemberLogin.CreateJwtToken(memberLogin, expiration);
                memberDb.SaveChanges();

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)expiration.TotalSeconds,
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
