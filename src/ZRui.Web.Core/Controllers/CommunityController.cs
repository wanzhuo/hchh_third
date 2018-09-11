using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ZRui.Web.CommunityModels;

namespace ZRui.Web.Controllers
{
    [Authorize]
    public class CommunityController : CommunityControllerBase
    {
        private readonly ILogger _logger;
        public CommunityController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<CommunityAPIController>();
        }

        [Authorize]
        public ActionResult Open(string flag, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction("detail", new { flag = flag });
            }
            else
            {
                ViewData.Model = flag;
                return View();
            }
        }

        //
        // GET: /Document/
        [Authorize]
        public ActionResult Detail(string communityFlag)
        {
            var username = User.Identity.Name;
            var isManager = _options.IsRoot(username);
            if (!isManager)
            {
                var community = _communityService.GetSingle(communityFlag);
                isManager = community.IsManager(username);
            }
            ViewData.Model = new DetailModel()
            {
                CommunityFlag = communityFlag,
                IsManager = isManager
            };
            return View();
        }

        [Authorize]
        public ActionResult AppSetting(string communityFlag, string appFlag)
        {

            var username = User.Identity.Name;
            var isManager = _options.IsRoot(username);
            if (!isManager)
            {
                var community = _communityService.GetSingle(communityFlag);
                isManager = community.IsManager(username);
            }

            if (!isManager)
            {
                throw new Exception("您不是管理员");
            }

            ViewData.Model = new AppSettingModel()
            {
                AppFlag = appFlag,
                CommunityFlag = communityFlag
            };
            return View();
        }

        [Authorize]
        [Route("community/apps/{appName}/{appPage}")]
        [HttpGet]
        public ActionResult AppStart(string appName, string appPage, string communityFlag, string appFlag)
        {
            var community = _communityService.GetSingle(communityFlag);
            checkIsMember(community, GetUsername());
            //checkIsMember(communityArgsModel.CommunityFlag, GetUsername());
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("应用程序不存在");
            if (!app.Url.Equals($"/community/apps/{ appName}/{ appPage}", StringComparison.OrdinalIgnoreCase)) throw new Exception("应用程序地址错误");
            ViewData.Model = new AppStartModel()
            {
                AppFlag = appFlag,
                CommunityFlag = communityFlag,
                AppName = appName,
                AppPage = appPage
            };
            return View();
        }
    }

}
