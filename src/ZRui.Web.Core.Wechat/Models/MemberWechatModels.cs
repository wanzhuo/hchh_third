using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Wechat.MemberWechatModels
{
    public class GetLoginQRCodeUrlArgsModel
    {
        public string ClientId { get; set; }
    }

    public class LoginModel
    {
        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
