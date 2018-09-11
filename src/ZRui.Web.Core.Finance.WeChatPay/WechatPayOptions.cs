using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.WechatPay
{
    public class WechatPayOptions: PayBase.IPayOption
    {
        public string Key { get; set; }
        public string OrderUrl { get; set; }
        public string OrderQueryUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string Version { get; set; }
        public string WftPublicKey { get; set; }
    }
}
