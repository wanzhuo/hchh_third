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
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.Open;
using Microsoft.Extensions.Options;
using ZRui.Web.Core.Wechat.Open;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Senparc.Weixin.Open.ComponentAPIs;
using System.Linq;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Entities;

namespace ZRui.Web
{
    public class StartupExtForCoreWechatOpen : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<StartupExtForCoreWechatOpen>();
            logger.LogInformation("StartupExtForCoreWechatOpen：进入Configure");
            var options = app.ApplicationServices.GetService<IOptions<WechatOpenOptions>>()
                .Value;
            var WeixinSettingOptions = app.ApplicationServices.GetService<IOptions<SenparcWeixinSetting>>() ;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var _contextOptions = new DbContextOptionsBuilder<WechatOpenCoreDbContext>()
                .UseMySql(configuration.GetConnectionString("WechatOpenCoreDbConnection"))
                .Options;            
         
            //RegisterService.Start(env, WeixinSettingOptions, true)
            //    .RegisterCacheRedis("localhost:6389",
            //        redisConfiguration =>
            //        {
            //            logger.LogInformation($"redisConfiguration：{redisConfiguration}");
            //            return (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            //         ? RedisObjectCacheStrategy.Instance
            //         : null;
            //        })
                
            //    .RegisterOpenComponent(options.AppId
            //    , options.AppSecret
            //    , componentAppId => //getComponentVerifyTicketFunc
            //    {
            //        return env.GetOpenTicket(componentAppId, _contextOptions);
            //    }
            //    , (componentAppId, auhtorizerId) => //getAuthorizerRefreshTokenFunc
            //    {
            //        using (var db = new WechatOpenCoreDbContext(_contextOptions))
            //        {
            //            logger.LogInformation($"获取auhtorizer{auhtorizerId}的RefreshToken");
            //            var refreshToken = db.Query<WechatOpenAuthorizer>()
            //               .Where(m => !m.IsDel)
            //               .Where(m => m.AuthorizerAppId == auhtorizerId)
            //               .Select(m => m.AuthorizerRefreshToken)
            //               .FirstOrDefault();
            //            logger.LogInformation($"获取auhtorizer{auhtorizerId}的RefreshToken为{refreshToken}");
            //            return refreshToken;
            //        }
            //    }
            //    , (componentAppId, auhtorizerId, refreshResult) =>//authorizerTokenRefreshedFunc
            //    {
            //        //logger.LogInformation($"开始更新auhtorizer{auhtorizerId}的Token");
            //        using (var db = new WechatOpenCoreDbContext(_contextOptions))
            //        {
            //            var model = db.Query<WechatOpenAuthorizer>()
            //               .Where(m => !m.IsDel)
            //               .Where(m => m.AuthorizerAppId == auhtorizerId)
            //               .FirstOrDefault();
            //            if (model != null)
            //            {
            //                model.AuthorizerAccessToken = refreshResult.authorizer_access_token;
            //                model.AuthorizerRefreshToken = refreshResult.authorizer_refresh_token;
            //                model.ExpiresIn = refreshResult.expires_in;
            //                model.ExpiresTime = DateTime.Now.AddSeconds(refreshResult.expires_in);
            //                db.SaveChanges();

            //                //logger.LogInformation($"更新auhtorizer{auhtorizerId}成功");
            //            }
            //            else
            //            {
            //                logger.LogError($"更新auhtorizer{auhtorizerId}失败：未找到数据库纪录");
            //            }
            //        }
            //    }, "【惠吃惠喝】开放平台");

            
            //logger.LogInformation("StartupExtForCoreWechatOpen：结束Configure");
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<WechatOpenOptions>(configuration.GetSection("WechatOpenOptions"));
            services.AddDbContext<WechatOpenCoreDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("WechatOpenCoreDbConnection")));
        }
    }
}
