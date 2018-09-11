using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZRui.Web.MemberModels;

namespace ZRui.Web.Controllers
{

    [Authorize]
    public class MemberController : CommunityControllerBase
    {
        public MemberController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        : base(communityService, options, memberDb)
        { }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            ViewData.Model = new LoginModel()
            {
                ReturnUrl = returnUrl,
                MemberAPIOptions = _options
            };
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        public IActionResult Index()
        {
            ViewData.Model = new IndexModel()
            {
                MemberAPIOptions = _options
            };
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        public IActionResult MyInfo()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult PartialMyCommunitys(string viewname)
        {
            var username = GetUsername();
            var model = _communityService.GetList(username);
            return View(viewname, model);
        }
    }
}
