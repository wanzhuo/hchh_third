using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;

namespace ZRui.Web.Core.Finance.WechatPay
{
    /// <summary>
    /// 客户端消息返回头
    /// </summary>
    public class WechatPayResponseHandler:PayResponseBaseHandler
    {
        public WechatPayResponseHandler() { }


        public WechatPayResponseHandler(string xmlContent) : base(xmlContent) {}


        public override object PayInfo
        {
            get
            {
                return new
                {
                    prepay_id = parameters["prepay_id"] + ""
                };
            }
        }


        public object CreatePayInfo(Func<Hashtable,string> makeSing)
        {
            DateTime startTime = new DateTime(1970, 1, 1); 
            string timeStamp = (DateTime.Now - startTime).TotalSeconds.ToString();
            string package = "prepay_id=" + parameters["prepay_id"];
            Hashtable signParms = new Hashtable();
            signParms["appId"] = Appid;
            signParms["timeStamp"] = timeStamp;
            signParms["nonceStr"] = NonceStr;
            signParms["package"] = package;
            signParms["signType"] = "MD5";
            return new
            {
                timeStamp = timeStamp,
                nonceStr = NonceStr,
                package = package,
                signType = "MD5",
                paySign = makeSing(signParms)
            };
        }

    }
}
