using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.ShopWechatOpenApiModel
{

    public class LoginByWxAuthApiModel
    {
        public string shopFlag { get; set; }
        public string code { get; set; }
        public string encryptedData { get; set; }
        public string iv { get; set; }
    }


    public class LoginHchhApiModel
    {
        public string shopFlag { get; set; }
        public string openid { get; set; }
        public string encryptedData { get; set; }
        public string iv { get; set; }
    }

}
