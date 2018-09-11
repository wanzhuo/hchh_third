using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using System.Collections.Generic;
using Senparc.Weixin.Open.Containers;
using ZRui.Web.Core.Wechat;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Exceptions;
using System.IO;
using System.Text;
using Senparc.Weixin.Open.Entities.Request;
using ZRui.Web.ShopManager.MessageHandlers;
using ZRui.Web.Core.Wechat.Open.CustomMessageHandler;
using Senparc.Weixin.MP.MessageHandlers;
using Microsoft.Extensions.Logging;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    /// <summary>
    /// 用于接受微信的推送
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("[controller]/[action]")]
    public class ShopWechatOpenCoreController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        ILogger logger;
        ILoggerFactory loggerFactory;
        public ShopWechatOpenCoreController(IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<ShopWechatOpenCoreController>();
        }

        /// <summary>
        /// 微信服务器会不间断推送最新的Ticket（10分钟一次），需要在此方法中更新缓存
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult Notice(PostModel postModel)
        //{
        //    var logPath = hostingEnvironment.MapPath(string.Format("/App_Data/Open/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
        //    if (!Directory.Exists(logPath))
        //    {
        //        Directory.CreateDirectory(logPath);
        //    }
        //    try
        //    {
        //        postModel.Token = wechatOpenOptions.Token;
        //        postModel.EncodingAESKey = wechatOpenOptions.EncodingAESKey;//根据自己后台的设置保持一致
        //        postModel.AppId = wechatOpenOptions.AppId;//根据自己后台的设置保持一致


        //        string body = new StreamReader(Request.Body).ReadToEnd();
        //        byte[] requestData = Encoding.UTF8.GetBytes(body);
        //        Stream inputStream = new MemoryStream(requestData);

        //        var messageHandler = new CustomThirdPartyMessageHandler(inputStream, postModel, hostingEnvironment, db);//初始化
        //        //注意：再进行“全网发布”时使用上面的CustomThirdPartyMessageHandler，发布完成之后使用正常的自定义的MessageHandler，例如下面一行。
        //        //var messageHandler = new CommonService.CustomMessageHandler.CustomMessageHandler(Request.InputStream,
        //        //    postModel, 10);

        //        //记录RequestMessage日志（可选）
        //        //messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request.txt", DateTime.Now.Ticks)));
        //        messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.AppId)));

        //        messageHandler.Execute();//执行

        //        //记录ResponseMessage日志（可选）
        //        using (TextWriter tw = new StreamWriter(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.AppId))))
        //        {
        //            tw.WriteLine(messageHandler.ResponseMessageText);
        //            tw.Flush();
        //            tw.Close();
        //        }

        //        return Content(messageHandler.ResponseMessageText);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("推送的Ticket出错：{0}", ex.Message);
        //        throw;
        //        return Content("error：" + ex.Message);
        //    }
        //}


        /// <summary>
        /// 授权事件接收URL
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{AppId}")]
        public ActionResult Callback(Senparc.Weixin.MP.Entities.Request.PostModel postModel)
        {
            //此处的URL格式类型为：http://sdk.weixin.senparc.com/Open/Callback/$APPID$， 在RouteConfig中进行了配置，你也可以用自己的格式，只要和开放平台设置的一致。
            string oldAppid = postModel.AppId;
            //处理微信普通消息，可以直接使用公众号的MessageHandler。此处的URL也可以直接填写公众号普通的URL，如本Demo中的/Weixin访问地址。
            var logPath = hostingEnvironment.MapPath(string.Format("~/App_Data/Open/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            postModel.Token = wechatOpenOptions.Token;
            postModel.EncodingAESKey = wechatOpenOptions.EncodingAESKey; //根据自己后台的设置保持一致
            postModel.AppId = wechatOpenOptions.AppId; //根据自己后台的设置保持一致

            var maxRecordCount = 10;
            MessageHandler<CustomMessageContext> messageHandler = null;

            try
            {
                string body = new StreamReader(Request.Body).ReadToEnd();
                byte[] requestData = Encoding.UTF8.GetBytes(body);
                Stream inputStream = new MemoryStream(requestData);

                var checkPublish = false; //是否在“全网发布”阶段
                if (checkPublish)
                {
                    messageHandler = new Core.Wechat.Open.MessageHandlers.OpenMessageHandler.OpenCheckMessageHandler(inputStream, postModel, wechatOpenOptions.AppId, wechatOpenOptions.AppSecret, hostingEnvironment, 10);
                }
                else
                {
                    messageHandler = new Web.MessageHandlers.CustomMessagerHandlerForShop(inputStream, postModel, wechatOpenOptions.AppId, wechatOpenOptions.AppSecret, hostingEnvironment, db, loggerFactory, maxRecordCount);
                }

                messageHandler.RequestDocument.Save(Path.Combine(logPath,
                    string.Format("{0}_Request_{1}.txt", DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));

                messageHandler.Execute(); //执行

                if (messageHandler.ResponseDocument != null)
                {
                    var ticks = DateTime.Now.Ticks;
                    messageHandler.ResponseDocument.Save(Path.Combine(logPath,
                        string.Format("{0}_Response_{1}.txt", ticks,
                            messageHandler.RequestMessage.FromUserName)));

                    //记录加密后的日志
                    //if (messageHandler.UsingEcryptMessage)
                    //{
                    //    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath,
                    // string.Format("{0}_Response_Final_{1}.txt", ticks,
                    //     messageHandler.RequestMessage.FromUserName)));
                    //}
                }
                return new Senparc.Weixin.MP.MvcExtension.FixWeixinBugWeixinResult(messageHandler);
            }
            catch (Exception ex)
            {
                logger.LogError("接收微信信息,发生错误:{0}\n{1}", ex.Message, ex.StackTrace);
                using (
                    TextWriter tw =
                        new StreamWriter(hostingEnvironment.MapPath("~/App_Data/Open/Error_" + DateTime.Now.Ticks + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                    return Content("");
                }
            }
        }
    }
}
