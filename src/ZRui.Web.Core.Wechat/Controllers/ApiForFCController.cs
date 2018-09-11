//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Senparc.Weixin.MP.Containers;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using ZRui.Web.Core.Wechat;

//namespace ZRui.Web.Controllers
//{

//    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
//    [Route("[controller]/[action]")]
//    public class ApiForFCController : ApiControllerBase
//    {
//        WechatTemplateSendOptions wechatTemplateSendOptions;

//        public ApiForFCController(IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions)
//        {
//            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
//        }

//        [HttpPost]
//        public APIResult UserInfo(string openId)
//        {
//            try
//            {
//                string accessToken = AccessTokenContainer.GetAccessTokenResult(wechatTemplateSendOptions.AppId).access_token;
//                var userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(accessToken, openId);
//                return Success(new
//                {
//                    userInfo.headimgurl,
//                    userInfo.nickname
//                });
//            }
//            catch (Exception e)
//            {
//                return Error(e.Message);
//            }
//        }
//    }
//}
