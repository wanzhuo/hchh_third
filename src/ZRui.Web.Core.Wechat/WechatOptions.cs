using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Wechat
{
    public class WechatOptions
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string State { get; set; }
        public string Token { get; set; }
        public string EncodingAESKey { get; set; }
    }
}
