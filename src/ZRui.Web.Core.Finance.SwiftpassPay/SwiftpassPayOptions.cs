using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.SwiftpassPay
{
    public class SwiftpassPayOptions
    {
        public string DefaultAppId { get; set; }
        public string MchId { get; set; }
        public string Key { get; set; }
        public string ReqUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string Version { get; set; }
        public string MchPrivateKey { get; set; }
        public string WftPublicKey { get; set; }
    }
}
