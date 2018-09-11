using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// StartupExtension
    /// </summary>
    public static class StartupExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection service, IConfigurationRoot configuration)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var assemblys = RuntimeHelper.GetAllAssemblies();
            foreach (var assembly in assemblys)
            {
                var types = assembly.DefinedTypes.Where(m => m.ImplementedInterfaces.Contains(typeof(IStartupExt))).ToList();
                foreach (var type in types)
                {
                    var o = Activator.CreateInstance(type) as IStartupExt;
                    o.ConfigureServices(service, configuration);
                    service.AddSingleton(typeof(IStartupExt), type);
                }
            }
            return service;
        }

        public static IApplicationBuilder Configure(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            foreach (var item in app.ApplicationServices.GetServices<IStartupExt>())
            {
                item.Configure(app, env, loggerFactory);
            }
            return app;
        }
    }
}
