using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    public class StartupExtForCore : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MemberAPIOptions>(configuration);
            // Add application services.
            services.AddSingleton<ICommunityAppPermissionService, CommunityAppPermissionServiceForJsonFile>();
            services.AddSingleton<ICommunityService, CommunityServiceForJsonFile>();
        }
    }
}
