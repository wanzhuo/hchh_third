using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.Test
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurtaionServices()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = @"D:\ProjHchh\web\HuiChiHuiHe\appsettings.json", ReloadOnChange = true })
            .Build();
        }


    }
}
