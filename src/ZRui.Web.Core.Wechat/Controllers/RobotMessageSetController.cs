using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZRui.Web.Controllers
{
    [Authorize]
    public class RobotMessageSetController : CommunityControllerBase
    {
        private readonly ILogger _logger;
        public RobotMessageSetController(ICommunityService communityService, IOptions<MemberAPIOptions> options, MemberDbContext memberDb, ILoggerFactory loggerFactory)
        :base(communityService,options,memberDb)
        {
            _logger = loggerFactory.CreateLogger<RobotMessageSetController>();
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
