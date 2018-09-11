using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace ZRui.Web
{
    public interface ICommunityAppPermissionService
    {
        CommunityAppPermission GetSingle(string url);
    }

    public class CommunityAppPermissionServiceForJsonFile : ICommunityAppPermissionService
    {
        private readonly ILogger _logger;
        IList<CommunityAppPermission> _communityAppPermissions;
        Dictionary<CommunityAppPermission, string> _communityAppPermissionsDictionary;
        readonly string communitysPath;

        public CommunityAppPermission GetSingle(string url)
        {
            return _communityAppPermissions.Where(m => m.Url == url.ToLower()).FirstOrDefault();
        }

        private void Save(CommunityAppPermission communityAppPermission)
        {
            var filePath = _communityAppPermissionsDictionary[communityAppPermission];
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(communityAppPermission, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }
        
        public CommunityAppPermissionServiceForJsonFile(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CommunityAppPermissionServiceForJsonFile>();
            communitysPath = Directory.GetCurrentDirectory() + "/App_Data/CommunityAppPermissions";
            _logger.LogInformation("初始权限目录：{0}", communitysPath);
            if (!Directory.Exists(communitysPath))
            {
                Directory.CreateDirectory(communitysPath);
            }
            load();
        }

        private void load()
        {
            _communityAppPermissions = new List<CommunityAppPermission>();
            _communityAppPermissionsDictionary = new Dictionary<CommunityAppPermission, string>();
            var communitysDirectoryPaths = Directory.GetDirectories(communitysPath);
            foreach (var path in communitysDirectoryPaths)
            {
                _logger.LogInformation("加载应用权限：{0}", communitysDirectoryPaths);
                var settingPath = path + "/permissions.json";
                if (!File.Exists(settingPath)) continue;
                using (StreamReader reader = File.OpenText(settingPath))
                {
                    var content = reader.ReadToEnd();
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<CommunityAppPermission>(content);
                    _communityAppPermissionsDictionary.Add(model, settingPath);
                    _communityAppPermissions.Add(model);
                }
            }
        }


    }
    /// <summary>
    /// 群组应用
    /// </summary>
    public class CommunityAppPermission
    {
        public string Url { get; set; }
        public List<string> Sources { get; set; }

        public CommunityAppPermission()
        {
            Sources = new List<string>();
        }
    }
}