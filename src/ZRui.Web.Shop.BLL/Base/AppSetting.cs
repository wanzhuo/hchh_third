using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace ZRui.Web.BLL
{
    public class AppSetting
    {
        private static readonly object objLock = new object();
        private static AppSetting instance = null;
        private JObject jsonConfig;

        public static AppSetting GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new AppSetting();
                        string ConfigPath = Environment.CurrentDirectory + @"\appsettings.json";
                        string json = File.ReadAllText(ConfigPath, Encoding.Default);
                        instance.jsonConfig = (JObject)JsonConvert.DeserializeObject(json);
                    }
                }
            }

            return instance;
        }

        public string GetConfig(string sectionName)
        {
           // return GetInstance().Config.GetSection(name).Value;
            string value = string.Empty;
            if (sectionName.Contains(":"))
            {
                string[] sectionArray = sectionName.Split(':');
                 value = jsonConfig[sectionArray[0]][sectionArray[1]].ToString();
            }
            else
            {
                 value = jsonConfig[sectionName].ToString();
            }
            return value;
        }
    }
}
