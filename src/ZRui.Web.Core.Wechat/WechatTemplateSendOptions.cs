using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Wechat
{
    public class WechatTemplateSendOptions
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string ServiceTemplateId { get; set; }
        public string TakeOutTemplateId { get; set; }
        public string SendUrl { get; set; }
    }
}
