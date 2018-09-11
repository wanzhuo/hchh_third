using System.Collections.Generic;

namespace ZRui.Web.CommunityModels
{

    public class DetailModel
    {
        public string CommunityFlag { get; set; }
        public bool IsManager { get; set; }
    }

    public class AppSettingModel
    {
        public string AppFlag { get; set; }
        public string CommunityFlag { get; set; }
    }

    public class AppStartModel:CommunityArgsModel
    {
        public string AppName { get; set; }
        public string AppPage { get; set; }
    }

}