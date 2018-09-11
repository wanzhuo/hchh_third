using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web.Core.Controllers
{
    public class MyCommunitysViewComponent : ViewComponent
    {
        private readonly ILogger _logger;
        MemberDbContext memberDb;
        ICommunityService _communityService;
        public MyCommunitysViewComponent(ICommunityService communityService, MemberDbContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MyCommunitysViewComponent>();
            memberDb = context;
            _communityService = communityService;
        }

        public IViewComponentResult Invoke(string viewname)
        {
            var username = GetUsername();
            var model = _communityService.GetList(username);
            return View(viewname, model);
        }

        protected string GetUsername()
        {
            return "member" + GetMemberId();
        }

        protected int GetMemberId()
        {
            var loginFlag = User.Identity.Name;
            return this.memberDb.GetMemberIdByLoginFlag(loginFlag);
        }
    }
}
