using System;

namespace ZRui.Web.BLL.Utils
{
    public static class ServiceLocator
    {
        private static IServiceProvider _Instance = null;
        public static IServiceProvider Instance
        {
            get
            {
                return _Instance;
            }
            set
            {
                if (_Instance == null) _Instance = value;
            }
        }
    }
}
