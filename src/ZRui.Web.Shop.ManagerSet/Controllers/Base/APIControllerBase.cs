using Microsoft.AspNetCore.Mvc.Filters;
using ZRui.Web.Controllers;

namespace ZRui.Web.Core.Printer.Web.Controllers
{
    public class PrintAPIControllerBase : ApiControllerBase
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        // protected APIResult<T> Success<T>(T content, string message);
        // protected APIResult<T> Success<T>(T content);
    }
}
