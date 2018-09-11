using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace ZRui.Web
{
    public static class IdentityExtension
    {
        public static string Truename(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Truename");
            return (claim != null) ? claim.Value : "无名";
        }
        public static string Role(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Role");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
