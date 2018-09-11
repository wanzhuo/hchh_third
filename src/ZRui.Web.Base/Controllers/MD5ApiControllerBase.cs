using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web.Controllers
{
    public class MD5ApiControllerBase : ApiControllerBase
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var md5ArgsList = context.ActionArguments.Select(m => m.Value).OfType<MD5AuthorizeArgs>();
                foreach (var args in md5ArgsList)
                {
                    args.CheckSign();
                }
            }
            catch (Exception ex)
            {
                context.Result = Json(Error(ex.Message));
                return;
            }
        }
    }
}
