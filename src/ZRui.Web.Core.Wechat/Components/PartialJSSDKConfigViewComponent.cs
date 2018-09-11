using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.Core.Controllers
{
    public class PartialJSSDKConfigViewComponent : ViewComponent
    {
        private readonly ILogger _logger;
        ICommunityService _communityService;
        WechatOptions wechatOptions;
        public PartialJSSDKConfigViewComponent(IOptions<WechatOptions> wechatOptions, ICommunityService communityService, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PartialJSSDKConfigViewComponent>();
            _communityService = communityService;
            this.wechatOptions = wechatOptions.Value;
        }

        public IViewComponentResult Invoke(string url, string viewName)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = "default";
            try
            {
                var appId = wechatOptions.AppId;
                var appSecret = wechatOptions.AppSecret;

                //获取时间戳
                var timestamp = JSSDKHelper.GetTimestamp();
                //获取随机码
                var nonceStr = Common.CommonUtil.CreateNoncestr();
                string ticket = JsApiTicketContainer.TryGetJsApiTicket(appId, appSecret);
                if (string.IsNullOrEmpty(url))
                {
                    url = $"{Request.Scheme}://{Request.Host}{Request.Path}";
                    if (Request.QueryString.HasValue)
                    {
                        url += $"?{Request.QueryString.Value}";
                    }
                }

                //获取签名
                var signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, url);

                return View(viewName, new PartialJSSDKConfigViewModel()
                {
                    AppId = appId,
                    NonceStr = nonceStr,
                    Signature = signature,
                    Timestamp = timestamp,
                    Url = url,
                    Error = ticket
                });
            }
            catch (Exception ex)
            {
                return View(viewName, new PartialJSSDKConfigViewModel()
                {
                    Error = ex.Message
                });
            }

        }
    }

    public class PartialJSSDKConfigViewModel
    {
        public string AppId { get; set; }
        public string NonceStr { get; set; }
        public string Signature { get; set; }
        public string Timestamp { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
    }
}
