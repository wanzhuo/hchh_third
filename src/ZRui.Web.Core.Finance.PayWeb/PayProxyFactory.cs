using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Finance.SwiftpassPay;
using ZRui.Web.Core.Finance.WechatPay;

namespace ZRui.Web
{
    public class PayProxyFactory
    {

        WechatPayOptions wechatPayOptions;
        SwiftpassPayOptions swiftpassPayOptions;

        ILoggerFactory loggerFactory;

        public PayProxyFactory(IOptions<WechatPayOptions> wechatPayOptions, IOptions<SwiftpassPayOptions> swiftpassPayOptions, ILoggerFactory loggerFactory)
        {
            this.wechatPayOptions = wechatPayOptions.Value;
            this.swiftpassPayOptions = swiftpassPayOptions.Value;
            this.loggerFactory = loggerFactory;
        }

        public PayProxyFactory(WechatPayOptions wechatPayOptions, SwiftpassPayOptions swiftpassPayOptions, ILoggerFactory loggerFactory)
        {
            this.wechatPayOptions = wechatPayOptions;
            this.swiftpassPayOptions = swiftpassPayOptions;
            this.loggerFactory = loggerFactory;
        }


        public PayProxyBase GetProxy(ShopPayInfo payInfo)
        {
            PayProxyBase rtn;
            ILogger logger;
            switch (payInfo.PayWay)
            {
                case PayWay.Wechat:
                    logger = loggerFactory.CreateLogger<WechatPayProxy>();
                    rtn = new WechatPayProxy(payInfo, wechatPayOptions, logger);
                    break;
                case PayWay.Swiftpass:
                    logger = loggerFactory.CreateLogger<SwiftpassPayProxy>();
                    rtn = new SwiftpassPayProxy(payInfo, null, logger);
                    break;
                default:
                    logger = loggerFactory.CreateLogger<WechatPayProxy>();
                    rtn = new WechatPayProxy(payInfo, wechatPayOptions, logger);
                    break;
            }
            return rtn;
        }

    }
}
