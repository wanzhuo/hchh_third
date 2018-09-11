using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZRui.Web.Controllers
{
    public class HtmlController : Controller
    {
        //
        // GET: /Html/
        [AppendTrailingSlash]
        [Route("{flag}-i-{paramString}.html")]
        [Route("{flag}.html")]
        [Route("{folder1}/{flag}-i-{paramString}.html")]
        [Route("{folder1}/{flag}.html")]
        [Route("{folder1}/{folder2}/{flag}-i-{paramString}.html")]
        [Route("{folder1}/{folder2}/{flag}.html")]
        [HttpGet]
        public ActionResult Index(string flag, string folder1, string folder2,string paramString)
        {
            if (string.IsNullOrEmpty(flag)) flag = "index";
            ViewData.Model = paramString;
            if (string.IsNullOrEmpty(folder1) && string.IsNullOrEmpty(folder2))
            {
                return View(flag.ToLower());
            }
            else if (string.IsNullOrEmpty(folder2))
            {
                flag = folder1 + "/" + flag;
                return View(flag.ToLower());
            }
            else 
            {
                flag = folder1 + "/" + folder2 + "/" + flag;
                return View(flag.ToLower());
            }
        }
    }

    public class AppendTrailingSlashAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (request.Path.HasValue && !request.Path.Value.EndsWith(".html") && !request.Path.Value.EndsWith("/"))
            {
                filterContext.Result = new RedirectResult(request.Path + "/" + request.QueryString);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
