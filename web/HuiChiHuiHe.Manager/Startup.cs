using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using NLog.Web;
using ZRui.Web.Core.Wechat;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using ZRui.Web.Sms;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace ZRui.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            // Add framework services.
            services.AddOptions();

            services.Configure<Sms.SmsOptions>(Configuration.GetSection("SmsOptions"));
          
            services.AddCors(options =>
            {
                options.AddPolicy("AllowTestOrigin", builder =>
                {
                    //builder.WithOrigins("http://localhost:8081");
                    //builder.AllowCredentials();
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    builder.WithOrigins("http://localhost:8080");
                });
            });

            services.AddMvc().AddJsonOptions(options=> {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowTestOrigin"));
            });
            var secretKey = "STkzn6iROYF8YqE840An";
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "wechat",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "user",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = "/Member/Login/";
                })
                .AddJwtBearer("jwt", o =>
                {
                    o.TokenValidationParameters = tokenValidationParameters;
                });


            services.AddHangfire(m =>
            {
                m.UseStorage(new Hangfire.MemoryStorage.MemoryStorage());
            });

            //下面是Swagger相关
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Version = "v1",
                        Title = "ZRui.Web",
                    }
                );
                //c.OperationFilter<ApplySummariesOperationFilter>();
                var xmls = Directory.GetFiles(PlatformServices.Default.Application.ApplicationBasePath, "ZRui.Web.*.xml");
                foreach (var item in xmls)
                {
                    c.IncludeXmlComments(item);
                }

                c.CustomSchemaIds(m =>
                {
                    return m.ToString();
                });
            });
            services.AddAutoMapper();
            services.ConfigureServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseResponseCompression();
            app.UseCors("AllowTestOrigin");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog("nlog.config");
            //app.AddNLogWeb();
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("启动");

            app.UseHangfireServer();
            app.UseHangfireDashboard("/aaaaabbbbb", new DashboardOptions
            {
                Authorization = new[] { new DefaultDashboardAuthorizationFilter() }
            });
           
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            app.Configure(env, loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //下面是Swagger相关
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
        }
    }

    public class DefaultDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }

}
