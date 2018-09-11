using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Core.Wechat;
using Senparc.Weixin.MP.Containers;
using Microsoft.Extensions.Options;

namespace ZRui.Web
{
    public class StartupExtForCoreWechat : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var options = app.ApplicationServices.GetService<IOptions<WechatTemplateSendOptions>>()
                .Value;
            if (!AccessTokenContainer.CheckRegistered(options.AppId))//检查是否已经注册
                AccessTokenContainer.Register(options.AppId, options.AppSecret);
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<WechatOptions>(configuration.GetSection("WechatOptions"));
            services.Configure<WechatTemplateSendOptions>(configuration.GetSection("WechatTemplateSendOptions"));
            services.AddDbContext<WechatCoreDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("WechatCoreDbConnection")));
        }
    }
}
