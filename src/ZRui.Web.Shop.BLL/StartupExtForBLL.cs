using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;

namespace ZRui.Web
{
    public class StartupExtForBLL : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<ShopConglomerationOrderOptions>(configuration);
            services.Configure<ShopConglomerationActivityOptions>(configuration);
            services.Configure<ShopIntegralRechargeServer>(configuration);
            services.Configure<ThirdServer>(configuration);
            services.Configure<ThirdConfig>(configuration.GetSection("ThirdConfig"));
            services.AddMemoryCache();
        }
    }
}
