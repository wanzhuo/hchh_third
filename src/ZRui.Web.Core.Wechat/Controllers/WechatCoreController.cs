using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;
using ZRui.Web.Core.Wechat;
using System.IO;
using System.Text;

namespace ZRui.Web.Controllers
{
    public class WechatCoreController : Controller
    {
        private readonly ILogger _logger;
        MemberDbContext _context;
        ICommunityService _communityService;
        WechatCoreDbContext wechatCoreDb;
        WechatOptions wechatOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wechatOptions"></param>
        /// <param name="wechatCoreDb"></param>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="loggerFactory"></param>
        public WechatCoreController(IOptions<WechatOptions> wechatOptions, WechatCoreDbContext wechatCoreDb, ICommunityService communityService, IOptions<MemberAPIOptions> options, MemberDbContext context, ILoggerFactory loggerFactory)
        {
            this.wechatCoreDb = wechatCoreDb;

            _logger = loggerFactory.CreateLogger<CommunityAPIController>();
            _communityService = communityService;
            _context = context;
            this.wechatOptions = wechatOptions.Value;
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        [Route("wechatCore")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, wechatOptions.Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, wechatOptions.Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        [Route("wechatCore")]
        public ActionResult Post(PostModel postModel)
        {
            try
            {
                _logger.LogTrace(string.Format("post:进入！"));
                if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, wechatOptions.Token))
                {
                    _logger.LogTrace(string.Format("post:参数错误！"));
                    return Content("参数错误！");
                }
                _logger.LogTrace(string.Format("post:Check End！"));

                postModel.Token = wechatOptions.Token;//根据自己后台的设置保持一致
                postModel.EncodingAESKey = wechatOptions.EncodingAESKey;//根据自己后台的设置保持一致
                postModel.AppId = wechatOptions.AppId;//根据自己后台的设置保持一致

                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                var maxRecordCount = 10;

                //XDocument xdoc = null;
                //using (XmlReader reader = XmlReader.Create(Request.Body))
                //{
                //    xdoc = XDocument.Load(reader);
                //}

                ////自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                //_logger.LogTrace(string.Format("post:new Handler"));

                //var messageHandler = new WechatCoreMessageHandler(wechatCoreDb, wechatOptions.AppId, wechatOptions.AppSecret, xdoc, postModel, _logger, maxRecordCount);


                string body = new StreamReader(Request.Body).ReadToEnd();
                byte[] requestData = Encoding.UTF8.GetBytes(body);
                Stream inputStream = new MemoryStream(requestData);
                var messageHandler = new WechatCoreMessageHandler(wechatCoreDb, wechatOptions.AppId, wechatOptions.AppSecret, inputStream, postModel, _logger, maxRecordCount);


                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;

                //执行微信处理过程
                messageHandler.Execute();
                _logger.LogTrace($"post:finish messageHandler.Execute");

                if (messageHandler.ResponseDocument != null)
                {
                    _logger.LogTrace($"post:{messageHandler.ResponseDocument.ToString()}");
                }

                //return new WeixinResult(messageHandler);
                //return Content("success");

                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(10001), ex, "weixin post error");
                var aa = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    aa += " | " + ex.Message;
                }
                _logger.LogError(new EventId(10002), ex, "weixin post error" + aa + ex.StackTrace);
                return Content("success");
            }
        }
    }

}
