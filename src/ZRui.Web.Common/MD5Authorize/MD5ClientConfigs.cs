using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web
{
    public class MD5AuthorizeConfigs
    {
        public static MD5AuthorizeConfigs Instance = new MD5AuthorizeConfigs();
        private string directoryPath;
        private List<MD5Client> clientApps;
        public MD5AuthorizeConfigs()
        {
            clientApps = new List<MD5Client>();
            directoryPath = Directory.GetCurrentDirectory() + "/App_Data/MD5ClientConfigs/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            Load();
        }
        public MD5Client GetSingle(string appId)
        {
            return clientApps.FirstOrDefault(m => m.ClientId == appId);
        }

        public void Load()
        {
            List<MD5Client> _clientApps = new List<MD5Client>();
            var filePaths = Directory.GetFiles(directoryPath);
            foreach (var path in filePaths)
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    var content = reader.ReadToEnd();
                    var community = Newtonsoft.Json.JsonConvert.DeserializeObject<MD5Client>(content);
                    _clientApps.Add(community);
                }
            }
            clientApps = _clientApps;
        }
    }
}
