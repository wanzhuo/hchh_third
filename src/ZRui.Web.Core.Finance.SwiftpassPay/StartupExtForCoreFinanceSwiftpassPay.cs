using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZRui.Web.Core.Finance.SwiftpassPay;

namespace ZRui.Web
{
    public class StartupExtForCoreFinanceSwiftpassProxy : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //services.Configure<SwiftpassPayOptions>(configuration.GetSection("SwiftpassPayOptions"));
            //services.Configure<FinanceDbContext>(configuration);
            // services.AddSingleton<FinanceDbContext>(); 
        }

    }
}
