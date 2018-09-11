using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZRui.Web.MemberAPIModels;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        private IHostingEnvironment hostingEnvironment;
        public MemberAPIController(ICommunityService communityService
            , MemberDbContext memberDb
            , IOptions<MemberAPIOptions> options
            , IHostingEnvironment hostingEnvironment
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<MemberAPIController>();
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: api/values
        [HttpPost]
        public async Task<APIResult> LoginForEmail([FromBody]LoginForEmailArgsModel args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Email)) throw new ArgumentNullException("Email");
                if (string.IsNullOrEmpty(args.Password)) throw new ArgumentNullException("Password");

                var member = memberDb.Members
                    .Where(m => m.Email == args.Email)
                    .Where(m => !m.IsDel)
                    .Select(m => new
                    {
                        Id = m.Id,
                        Email = m.Email,
                        Password = m.Password,
                        Truename = m.Truename
                    })
                    .FirstOrDefault();

                if (member == null) throw new ArgumentException("账号不正确");

                var password = MemberPasswordToMD5(args.Password);
                if (password != member.Password) throw new ArgumentException("密码不正确");

                var memberLogin = new MemberLogin()
                {
                    Flag = CommonUtil.CreateNoncestr(18),
                    LoginType = CookieAuthenticationDefaults.AuthenticationScheme,
                    MemberId = member.Id
                };
                memberDb.Add<MemberLogin>(memberLogin);
                await memberDb.SaveChangesAsync();

                List<Claim> claims = new List<Claim>();
                //var username = "member" + member.Id;
                //claims.Add(new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, null));
                claims.Add(new Claim(ClaimTypes.Name, memberLogin.Flag, ClaimValueTypes.String, null));
                claims.Add(new Claim("Truename", member.Truename, ClaimValueTypes.String));

                var userIdentity = new ClaimsIdentity("Form");
                userIdentity.AddClaims(claims);

                var principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, string.Empty);
                return Error(ex.Message);
            }
        }


        // GET: api/values
        [HttpPost]
        public async Task<APIResult> LoginBySms([FromBody]LoginForSmsArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Phone)) throw new ArgumentNullException("Phone");
            if (string.IsNullOrEmpty(args.Code)) throw new ArgumentNullException("Code");

            //通过用户名和用户Id获取和判断
            var memberId = memberDb.GetMemberIdByMemberPhone(args.Phone);

            if (memberId <= 0)
                throw new Exception(string.Format("手机:{0}没有绑定会员", args.Phone));

            memberDb.SetMemberSMSValiCodeTaskFinished(args.Phone, args.Code, "Login");

            var member = memberDb.Members
                    .Where(m => m.Id == memberId)
                    .Where(m => !m.IsDel)
                    .Select(m => new
                    {
                        Id = m.Id,
                        Email = m.Email,
                        Truename = m.Truename
                    })
                    .FirstOrDefault();

            var memberLogin = new MemberLogin()
            {
                Flag = CommonUtil.CreateNoncestr(18),
                LoginType = CookieAuthenticationDefaults.AuthenticationScheme,
                MemberId = member.Id
            };
            memberDb.Add<MemberLogin>(memberLogin);
            await memberDb.SaveChangesAsync();

            List<Claim> claims = new List<Claim>();
            //var username = "member" + memberId;
            //claims.Add(new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, null));
            claims.Add(new Claim(ClaimTypes.Name, memberLogin.Flag, ClaimValueTypes.String, null));
            claims.Add(new Claim("Truename", member.Truename, ClaimValueTypes.String));

            var userIdentity = new ClaimsIdentity("Form");
            userIdentity.AddClaims(claims);

            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Success();
        }
        /// <summary>
        /// 通过短信注册
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> RegisterBySms([FromBody]RegisterBySmsArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Phone)) throw new ArgumentNullException("Phone");
            if (string.IsNullOrEmpty(args.Code)) throw new ArgumentNullException("Code");
            var phone = args.Phone.Trim();

            var isBindPhone = memberDb.IsBindMemberPhone(phone);
            if (isBindPhone)
                throw new Exception("该手机已绑定使用，请重新输入手机！");

            memberDb.SetMemberSMSValiCodeTaskFinished(phone, args.Code, "Register");

            var member = new Member()
            {
                Email = $"{args.Phone}@phone",
                Password = MemberPasswordToMD5(CommonUtil.CreateIntNoncestr(8)),
                Truename = args.Phone,
                RegIP = GetIp(),
                RegTime = DateTime.Now,
                LastLoginIP = GetIp(),
                LastLoginTime = DateTime.Now,
                Status = MemberStatus.正常
            };
            memberDb.AddToMember(member);

            MemberPhone memberPhone = new MemberPhone()
            {
                Member = member,
                Phone = phone,
                State = MemberPhoneState.已绑定
            };
            memberDb.Add<MemberPhone>(memberPhone);
            await memberDb.SaveChangesAsync();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetDashBoardWidgets([FromBody]GetDashBoardWidgetsArgsModel args)
        {
            try
            {
                var model = new GetDashBoardWidgetsModel();
                model.One.Add(new DashBoadWidgetInfo()
                {
                    Title = "网站管理",
                    ContentUrl = ""
                });

                return Success(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, string.Empty);
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult ChangePassword([FromBody]ChangePasswordArgsModel args)
        {
            var memberId = GetMemberId();
            var model = memberDb.QueryMember()
                .Where(m => m.Id == memberId)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            if (model.Password != MemberPasswordToMD5(args.Password)) throw new Exception("原密码不正确");
            if (args.NewPassword != args.ConfirmPassword) throw new Exception("两次输入的密码不相等");

            model.Password = MemberPasswordToMD5(args.NewPassword);
            memberDb.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt,Cookies")]
        public APIResult<GetMemberInfoModel> GetMemberInfo()
        {
            var memberId = GetMemberId();
            var model = memberDb.QueryMember()
                .Where(m => m.Id == memberId)
                .Select(m => new GetMemberInfoModel()
                {
                    Truename = m.Truename,
                    Avatar = m.Avatar,
                    LastLoginIP = m.LastLoginIP,
                    LastLoginTime = m.LastLoginTime,
                    LoginCount = m.LoginCount
                })
                .FirstOrDefault();

            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt,Cookies")]
        public async Task<APIResult> UpdateMemberInfo([FromBody]UpdateMemberInfoArgsModel args)
        {
            var memberId = GetMemberId();
            var model = memberDb.GetSingleMember(memberId);
            if (model == null) throw new Exception("用户纪录不存在");

            model.Avatar = args.Avatar;
            model.Truename = args.Truename;

            await memberDb.SaveChangesAsync();

            return Success();
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt,Cookies")]
        public async Task<APIResult> UploadImageByBase64ForOpen([FromBody]UploadImageByBase64ArgsModel args)
        {
            var memberId = GetMemberId();
            string requestStrValue = args.Data.Substring(args.Data.IndexOf(',') + 1);//代表 图片 的base64编码数据  
            string requestFileExtension = args.Data.Split(new char[] { ';' })[0].Substring(args.Data.IndexOf('/') + 1);//获取后缀名 

            var buffer = Convert.FromBase64String(requestStrValue);

            var fileLogicPath = $"/content/members/{memberId}/image/{DateTime.Now.ToString("yyyyMMdd")}/{CommonUtil.CreateNoncestr(8)}.{requestFileExtension}";
            _logger.LogInformation(fileLogicPath);
            string localPath = hostingEnvironment.MapWebPath(fileLogicPath);
            _logger.LogInformation(localPath);
            Common.FileUtils.CreateDirectory(localPath);

            using (FileStream fs = new FileStream(localPath, FileMode.Create))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
                ////通过代码审查说，使用了using,不需要再次调用close
                //fs.Close();
            }
            return Success(new { path = fileLogicPath });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt,Cookies")]
        public APIResult<GetMemberInfoModel> GetCommunitys()
        {
            var memberId = GetMemberId();
            var model = memberDb.QueryMember()
                .Where(m => m.Id == memberId)
                .Select(m => new GetMemberInfoModel()
                {
                    Truename = m.Truename,
                    Avatar = m.Avatar,
                    LastLoginIP = m.LastLoginIP,
                    LastLoginTime = m.LastLoginTime,
                    LoginCount = m.LoginCount
                })
                .FirstOrDefault();

            return Success(model);
        }
    }
}
