using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ZRui.Web.CommunitySetModels;

namespace ZRui.Web.Controllers
{
    [Authorize]
    public class CommunitySetController : CommunityControllerBase
    {
        private readonly ILogger _logger;
        public CommunitySetController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<CommunitySetController>();
        }

        [Authorize]
        public ActionResult Index(string communityFlag, string appFlag)
        {
            ViewData.Model = new CommunityArgsModel()
            {
                AppFlag = appFlag,
                CommunityFlag = communityFlag
            };
            return View();
        }

        [Authorize]
        public ActionResult AppSetting(string communityFlag, string appFlag, string currentCommunityFlag, string currentAppFlag)
        {
            var username = User.Identity.Name;
            var community = _communityService.GetSingle(currentCommunityFlag);
            if (community == null) throw new Exception("指定标识的群组记录不存在");
            var app = community.Apps.Where(m => m.Flag == currentAppFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用记录不存在");

            ViewData.Model = new AppSettingModel()
            {
                AppFlag = appFlag,
                CommunityFlag = communityFlag,
                CurrentCommunityFlag = community.Flag,
                CurrentCommunityName = community.Name,
                CurrentCommunityIco = community.Ico,
                CurrentAppFlag = app.Flag,
                CurrentAppName = app.Name,
                CurrentAppIco = app.Ico
            };
            return View();
        }
    }
}
