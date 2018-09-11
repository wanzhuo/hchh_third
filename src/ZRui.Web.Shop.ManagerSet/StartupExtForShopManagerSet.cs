using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using ZRui.Web.ShopManager.MoneyOffExtension;
using ZRui.Web.OrderHandlers;
using ZRui.Web.Pay;
using ZRui.Web.Utils;
using ZRui.Web.Common;
using ZRui.Web.BLL.Third;

namespace ZRui.Web
{
    public class StartupExtForShopManagerSet : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<StartupExtForShopManagerSet>();

            RecurringJob.RemoveIfExists("moneyOffCache");
            RecurringJob.AddOrUpdate("moneyOffCache",
                  () => MoneyOffExtension.UpdateMoneyOffCache(), "5 16,19 * * *");
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<PayProxyFactory>();
            services.AddSingleton<ExThirdPartyDistributionParameter>();
        }




    }
}
