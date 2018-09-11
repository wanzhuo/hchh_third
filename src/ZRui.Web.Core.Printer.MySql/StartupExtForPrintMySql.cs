
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Data;

namespace ZRui.Web.Core.Printer.MySql
{
    public class StartupExtForPrintMySql : IStartupExt
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        { 
            services.AddDbContext<PrintDbContext>(options =>
              options.UseMySql(configuration.GetConnectionString("ShopDbConnection")));
            //PrinterAdmin.Configuration = configuration;
        }

    }
}
