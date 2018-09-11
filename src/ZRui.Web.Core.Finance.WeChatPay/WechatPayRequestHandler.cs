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
    public class WechatPayRequestHandler : PayRequestBaseHandler
    {
        public WechatPayRequestHandler(IPayOption options) : base(options)
        {
        }
    }
}
