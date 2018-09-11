using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZRui.Web.Controllers
{
    [Authorize]
    public class MemberSetController : CommunityControllerBase
    {
        private readonly ILogger _logger;
        public MemberSetController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        :base(communityService,options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<MemberSetController>();
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
