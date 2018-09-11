using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public static class IHostingEnvironmentExtension
    {
        public static string MapPath(this IHostingEnvironment  hostingEnvironment,string relativePath)
        {
            return System.IO.Path.Combine(hostingEnvironment.ContentRootPath, relativePath.Trim('/', '\\'));
        }

        public static string MapWebPath(this IHostingEnvironment hostingEnvironment, string relativePath)
        {
            return System.IO.Path.Combine(hostingEnvironment.WebRootPath, relativePath.Trim('/', '\\'));
        }
    }
}
