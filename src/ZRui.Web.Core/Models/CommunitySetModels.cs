using System.Collections.Generic;

namespace ZRui.Web.CommunitySetModels
{
    public class AppSettingModel: CommunityArgsModel
    {
        public string CurrentCommunityFlag { get; set; }
        public string CurrentCommunityName { get;  set; }
        public string CurrentCommunityIco { get;  set; }
        public string CurrentAppFlag { get; set; }
        public string CurrentAppName { get;  set; }
        public string CurrentAppIco { get;  set; }
    }
}