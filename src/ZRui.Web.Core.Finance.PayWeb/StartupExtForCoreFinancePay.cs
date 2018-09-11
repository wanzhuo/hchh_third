using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZRui.Web.Core.Finance.SwiftpassPay;
using ZRui.Web.Core.Finance.WechatPay;

namespace ZRui.Web
{
    public class StartupExtForCoreFinancePay : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<WechatPayOptions>(configuration.GetSection("WechatPayOptions"));
            services.Configure<SwiftpassPayOptions>(configuration.GetSection("SwiftpassPayOptions"));
            services.AddSingleton<PayProxyFactory>();
        }
    }
}
