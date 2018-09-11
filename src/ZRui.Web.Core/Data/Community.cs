using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
namespace ZRui.Web
{
    public interface ICommunityService
    {
        CommunityAppPermission GetSingleAppPermission(string url);
        IList<CommunityBase> GetListForBase();
        IList<CommunityBase> GetListForBase(string username);
        IList<Community> GetList(string username);
        Community GetSingle(string communityFlag);
        void AddMember(string communityFlag, string username);
        void AddManager(string communityFlag, string username);

        void RemoveMember(string communityFlag, string username);
        void RemoveManager(string communityFlag, string username);

        void SetName(string communityFlag, string name);
        void SetBase(string communityFlag, string name, string ico);

        CommunityAppSettings GetAppSettings(string communityFlag, string appFlag);
        void SetAppSettings(string communityFlag, string appFlag, CommunityAppSettings settings);
        void SetAppName(string communityFlag, string appFlag, string name);
        void SetAppBase(string communityFlag, string appFlag, string name, string ico);
        void SetAppIsDisabled(string communityFlag, string appFlag, bool isDisabled);
        void SetAppIsDefaultOpen(string communityFlag, string appFlag, bool isDefaultOpen);
        void AddApp(string communityFlag, string name, string url, string ico = "");
        void Add(string name);
        void RemoveApp(string communityFlag, string appFlag);
    }

    public class CommunityServiceForJsonFile : ICommunityService
    {
        ICommunityAppPermissionService communityAppPermissionService;
        IList<Community> _communitys;
        Dictionary<Community, string> _communitysDictionary;
        readonly string communitysPath;

        public void AddManager(string communityFlag, string username)
        {
            var community = GetSingle(communityFlag);
            if (community.Managers.Contains(username))
            {
                throw new Exception("管理员已经存在");
            }
            community.Managers.Add(username);
            Save(community);
        }

        public void AddMember(string communityFlag, string username)
        {
            var community = GetSingle(communityFlag);
            if (community.Members.Exists(m => m.Username == username))
            {
                throw new Exception("成员已经存在");
            }
            community.Members.Add(new CommunityMember()
            {
                Username = username,
                AddTime = DateTime.Now,
                Nickname = username
            });
            Save(community);
        }

        public IList<CommunityBase> GetListForBase()
        {
            return _communitys.Select(m => new CommunityBase()
            {
                Flag = m.Flag,
                Name = m.Name
            }).ToList();
        }

        public IList<CommunityBase> GetListForBase(string username)
        {
            return _communitys.Where(m => m.Members.Exists(x => x.Username == username)).Select(m => new CommunityBase()
            {
                Flag = m.Flag,
                Name = m.Name,
                Ico = m.Ico
            }).ToList();
        }
        public IList<Community> GetList(string username)
        {
            return _communitys.Where(m => m.Members.Exists(x => x.Username == username)).ToList();
        }
        public Community GetSingle(string communityFlag)
        {
            return _communitys.Where(m => m.Flag == communityFlag).FirstOrDefault();
        }

        public void RemoveManager(string communityFlag, string username)
        {
            var community = GetSingle(communityFlag);
            if (!community.Managers.Contains(username))
            {
                throw new Exception("指定的管理员不存在");
            }
            community.Managers.Remove(username);
            Save(community);
        }

        public void RemoveMember(string communityFlag, string username)
        {
            var community = GetSingle(communityFlag);
            var member = community.Members.Where(m => m.Username == username).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("指定成员不存在");
            }
            community.Members.Remove(member);
            Save(community);
        }

        public void SetName(string communityFlag, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            var community = GetSingle(communityFlag);
            community.Name = name;
            Save(community);
        }
        public void SetBase(string communityFlag, string name, string ico)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            var community = GetSingle(communityFlag);
            community.Name = name;
            community.Ico = ico;
            Save(community);
        }

        public void SetAppSettings(string communityFlag, string appFlag, CommunityAppSettings settings)
        {
            var community = _communitys.Where(m => m.Flag == communityFlag).FirstOrDefault();
            if (community == null) throw new Exception("群组不存在");
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("应用不存在");
            app.AppSettings = settings;
            Save(community);
        }

        public CommunityAppSettings GetAppSettings(string communityFlag, string appFlag)
        {
            var community = _communitys.Where(m => m.Flag == communityFlag).FirstOrDefault();
            if (community == null) throw new Exception("群组不存在");
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("应用不存在");
            if (app.AppSettings == null)
            {
                app.AppSettings = new CommunityAppSettings();
            }
            return app.AppSettings;
        }

        private void Save(Community community)
        {
            var filePath = _communitysDictionary[community];
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(community, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }

        public void SetAppName(string communityFlag, string appFlag, string name)
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(appFlag)) throw new ArgumentNullException("appFlag");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var community = GetSingle(communityFlag);
            if (community.Apps.Exists(m => m.Name == name && m.Flag != appFlag)) throw new Exception("名字已经被使用");
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用不存在");
            app.Name = name;
            Save(community);
        }

        public void SetAppBase(string communityFlag, string appFlag, string name, string ico)
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(appFlag)) throw new ArgumentNullException("appFlag");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var community = GetSingle(communityFlag);
            if (community.Apps.Exists(m => m.Name == name && m.Flag != appFlag)) throw new Exception("名字已经被使用");
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用不存在");
            app.Name = name;
            app.Ico = ico;
            Save(community);
        }

        public void SetAppIsDisabled(string communityFlag, string appFlag, bool isDisabled)
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(appFlag)) throw new ArgumentNullException("appFlag");

            var community = GetSingle(communityFlag);
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用不存在");
            app.IsDisabled = isDisabled;
            Save(community);
        }

        public void SetAppIsDefaultOpen(string communityFlag, string appFlag, bool isDefaultOpen)
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(appFlag)) throw new ArgumentNullException("appFlag");

            var community = GetSingle(communityFlag);
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用不存在");
            app.IsDefaultOpen = isDefaultOpen;
            Save(community);
        }


        public void AddApp(string communityFlag, string name, string url, string ico = "")
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            var community = GetSingle(communityFlag);
            community.Apps.Add(new CommunityApp()
            {
                Name = name,
                Ico = ico,
                Url = url,
                AppSettings = new CommunityAppSettings(),
                Flag = Common.CommonUtil.CreateNoncestr(8),
                Price = 0,
                IsDisabled = false
            });
            Save(community);

        }

        public void RemoveApp(string communityFlag, string appFlag)
        {
            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(appFlag)) throw new ArgumentNullException("appFlag");

            var community = GetSingle(communityFlag);
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("指定标识的应用不存在");
            community.Apps.Remove(app);
            Save(community);
        }

        public void Add(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            var flag = DateTime.Now.ToString("yyyyMMddHHmmss") + Common.CommonUtil.CreateIntNoncestr(4);
            var community = new Community()
            {
                Managers = new List<string>(),
                Members = new List<CommunityMember>(),
                Apps = new List<CommunityApp>(),
                CreateIp = "",
                CreateTime = DateTime.Now,
                Creator = "",
                Flag = flag,
                Name = name
            };
            var dirPath = Path.Combine(communitysPath, community.Flag);
            if (Directory.Exists(dirPath)) throw new Exception("文件夹已经存在");
            Directory.CreateDirectory(dirPath);
            var settingPath = dirPath + "/communitySettings.json";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(community, Formatting.Indented);
            File.WriteAllText(settingPath, jsonString);
            load();
        }

        public CommunityServiceForJsonFile(ICommunityAppPermissionService communityAppPermissionService)
        {
            this.communityAppPermissionService = communityAppPermissionService;
            communitysPath = Directory.GetCurrentDirectory() + "/App_Data/Communitys";
            if (!Directory.Exists(communitysPath))
            {
                Directory.CreateDirectory(communitysPath);
            }
            load();
        }

        private void load()
        {
            _communitys = new List<Community>();
            _communitysDictionary = new Dictionary<Community, string>();
            var communitysDirectoryPaths = Directory.GetDirectories(communitysPath);
            foreach (var path in communitysDirectoryPaths)
            {
                var settingPath = path + "/communitySettings.json";
                if (!File.Exists(settingPath)) continue;
                using (StreamReader reader = File.OpenText(settingPath))
                {
                    var content = reader.ReadToEnd();
                    var community = Newtonsoft.Json.JsonConvert.DeserializeObject<Community>(content);
                    _communitysDictionary.Add(community, settingPath);
                    _communitys.Add(community);
                }
            }
        }

        public CommunityAppPermission GetSingleAppPermission(string url)
        {
            return communityAppPermissionService.GetSingle(url);
        }
    }
    public class CommunityBase
    {
        public string Flag { get; set; }
        public string Name { get; set; }
        public string Ico { get; set; }
    }
    public class Community : CommunityBase
    {
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateIp { get; set; }
        public List<CommunityMember> Members { get; set; }
        public List<CommunityApp> Apps { get; set; }
        public List<string> Managers { get; set; }

        public bool IsDel { get; set; }

        public bool IsManager(string username)
        {
            return Managers != null && Managers.Contains(username);
        }

        public bool IsMember(string username)
        {
            return Members != null && Members.Exists(m => m.Username == username);
        }

        public Community()
        {
            Members = new List<CommunityMember>();
            Apps = new List<CommunityApp>();
            Managers = new List<string>();
        }
    }

    public class CommunityMember
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DateTime AddTime { get; set; }
    }
    /// <summary>
    /// 群组应用
    /// </summary>
    public class CommunityApp
    {
        public string Flag { get; set; }
        public string Name { get; set; }
        public string Ico { get; set; }
        public float Price { get; set; }
        public string Url { get; set; }
        public CommunityAppSettings AppSettings { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDefaultOpen { get; set; }

        public CommunityApp()
        {
        }
    }

    public class CommunityAppSettings
    {
        public List<CommunityAppSettingItem<object>> Settings { get; set; }

        public CommunityAppSettings()
        {
            Settings = new List<CommunityAppSettingItem<object>>();
        }

        public T GetSetting<T>(string key)
        {
            var setting = Settings.Where(m => m.Key == key).FirstOrDefault();
            if (setting == null) throw new Exception(key + "未设置");
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
                Settings.Add(new CommunityAppSettingItem<object>()
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

    public class CommunityAppSettingItem<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
    }
}