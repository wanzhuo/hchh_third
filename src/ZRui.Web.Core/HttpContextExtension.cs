using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ZRui.Web
{
    public static class HttpContextExtension
    {
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        public static int GetMemberId(this HttpContext context)
        {
            return int.Parse(context.User.Identity.Name.Replace("member", ""));
        }

        public static string GetUsername(this HttpContext context)
        {
            return context.User.Identity.Name;
        }

        public static string GetTruename(this HttpContext context)
        {
            var claim = ((ClaimsIdentity)context.User.Identity).FindFirst("Truename");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
