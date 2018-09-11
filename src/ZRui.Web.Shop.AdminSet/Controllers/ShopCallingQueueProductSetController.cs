using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;

namespace ZRui.Web.Controllers
{
    [Authorize]
    public class ShopCallingQueueProductSetController : CommunityControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCallingQueueProductSetController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
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
    }
}
