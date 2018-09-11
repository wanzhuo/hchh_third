using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZRui.Web.Sms;

namespace ZRui.Web.Survey
{
    public class StartupExtForSmsHttp : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<Sms.HttpSmsOptions>(configuration.GetSection("HttpSmsOptions"));
            services.AddSingleton<ISmsHandler, Sms.HttpSmsHandler>();
        }
    }
}
