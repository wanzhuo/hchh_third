using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
namespace ZRui.Web.Controllers
{
    public class ControllerBase : Controller
    {
        protected string GetIp()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return ip;
        }
    }
}
