using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Entities.Request;

using ZRui.Web.Core.Wechat.Open.CustomMessageHandler;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat.Open;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat.Open.MessageHandlers.OpenMessageHandler
{
    /// <summary>
    /// 开放平台全网发布之前需要做的验证
    /// </summary>
    public class OpenCheckMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
           https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318611&lang=zh_CN
            自动化测试的专用测试公众号的信息如下：
            （1）appid： wx570bc396a51b8ff8
            （2）Username： gh_3c884a361561
        */

        //private string testAppId = "wx570bc396a51b8ff8";

        private string componentAppId = "ComponentAppId";
        private string componentSecret = "Component_Secret";
        IHostingEnvironment hostingEnvironment;

        public OpenCheckMessageHandler(Stream inputStream, PostModel postModel,string componentAppId, string componentSecret, IHostingEnvironment hostingEnvironment, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            this.componentAppId = componentAppId;
            this.componentSecret = componentSecret;
            this.hostingEnvironment = hostingEnvironment;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            if (requestMessage.Content == "TESTCOMPONENT_MSG_TYPE_TEXT")
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = requestMessage.Content + "_callback";//固定为TESTCOMPONENT_MSG_TYPE_TEXT_callback
                return responseMessage;
            }

            if (requestMessage.Content.StartsWith("QUERY_AUTH_CODE:"))
            {
                //使用数据库去记录
                var _contextOptions = new DbContextOptionsBuilder<WechatOpenCoreDbContext>()
                    .UseMySql("Server=120.79.31.209;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;")
                    .Options;
                string openTicket = hostingEnvironment.GetOpenTicket(componentAppId, _contextOptions);
                var query_auth_code = requestMessage.Content.Replace("QUERY_AUTH_CODE:", "");
                try
                {
                    var component_access_token = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.GetComponentAccessToken(componentAppId, componentSecret, openTicket).component_access_token;
                    var oauthResult = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.QueryAuth(component_access_token, componentAppId, query_auth_code);

                    //调用客服接口
                    var content = query_auth_code + "_from_api";
                    var sendResult = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(oauthResult.authorization_info.authorizer_access_token,
                          requestMessage.FromUserName, content);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            return null;
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = requestMessage.Event + "from_callback";
            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "默认消息";
            return responseMessage;
        }
    }
}
