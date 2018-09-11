using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ZRui.Web
{
    public class MD5Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Detail { get; set; }
        public List<string> Resource { get; set; }
        public MD5ClientSettings Settings { get; set; }
    }

    public class MD5ClientSettings
    {
        public List<MD5ClientSettingItem<object>> Settings { get; set; }

        public MD5ClientSettings()
        {
            Settings = new List<MD5ClientSettingItem<object>>();
        }

        public T GetSetting<T>(string key)
        {
            var setting = Settings.Where(m => m.Key == key).FirstOrDefault();
            var v1 = setting.Value as JContainer;
            if (v1 != null)
            {
                return (T)v1.ToObject(typeof(T));
            }
            else
            {
                return (T)setting.Value;
            }
        }

        public bool HasKey(string key)
        {
            return Settings.Exists(m => m.Key == key);
        }

        public void SetSetting<T>(string key, T value)
        {
            var setting = Settings.Where(m => m.Key == key).FirstOrDefault();
            if (setting == null)
            {
                Settings.Add(new MD5ClientSettingItem<object>()
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                setting.Value = value;
            }
        }
    }

    public class MD5ClientSettingItem<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
    }
}
