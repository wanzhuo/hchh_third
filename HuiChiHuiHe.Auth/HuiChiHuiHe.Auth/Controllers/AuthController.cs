using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;
using Senparc.Weixin.Open.WxaAPIs;

namespace HuiChiHuiHe.Auth.Controllers
{
    [Route("api/Auth/[action]")]
    public class AuthController : Controller
    {
        AuthDbContext db;
        HchhLogDbContext logger;
        ShopDbContext shopDb;
        public AuthController(AuthDbContext db, HchhLogDbContext logger,ShopDbContext shopDb)
        {
            this.db = db;
            this.logger = logger;
            this.shopDb = shopDb;
        }
        [HttpPost]
        public ActionResult ReceiveComponentTicket(PostModel postModel)
        {
            string postXml = new StreamReader(Request.Body).ReadToEnd();
            try
            {
              

                ComponentAuthorizer accessToken = db.ComponentAuthorizer.FirstOrDefault();
                if (accessToken != null)
                {
                    postModel.EncodingAESKey = accessToken.EncodingAESKey;
                    postModel.Token = accessToken.Token;
                    postModel.AppId = accessToken.AppId;

                    byte[] requestData = Encoding.UTF8.GetBytes(postXml);
                    Stream inputStream = new MemoryStream(requestData);

                    HchhThirdPartyMessageHandler handler = new HchhThirdPartyMessageHandler(inputStream, postModel, shopDb, logger);
                    RequestMessageComponentVerifyTicket ticket = handler.RequestMessage as RequestMessageComponentVerifyTicket;

                    if (accessToken.Ticket != ticket.ComponentVerifyTicket)
                    {
                        accessToken.Ticket = ticket.ComponentVerifyTicket;
                        accessToken.TicketTime = ticket.CreateTime;
                    }

                    if (DateTime.Now > accessToken.ExpiredTime.AddMinutes(-30))  //判断到accesstoken即将过期 马上获取新accesstoken
                    {
                        try
                        {
                            string token = ComponentContainer.TryGetComponentAccessToken(accessToken.AppId, accessToken.AppSecret, accessToken.Ticket, true);
                            accessToken.AccessToken = token;
                            accessToken.UpdatedTime = DateTime.Now;
                            accessToken.ExpiredTime = DateTime.Now.AddSeconds(7200);                         
                        }
                        catch (Exception ex)
                        {
                            logger.Add(new TaskLog() { AddTime = DateTime.Now, ExeResult = accessToken.AppId + ":" + ex.Message, TaskName = "TryGetComponentAccessToken" });
                            logger.SaveChanges();
                        }
                    }
                    db.Update(accessToken);
                    db.SaveChanges();

                    RefreshAuthorizerToken(accessToken.AppId, accessToken.AccessToken);

                    return Content(handler.ResponseMessageText);
                }
                return Content("this appid not in database");
            } catch (Exception ex)
            {
                logger.Add(new TaskLog() { AddTime= DateTime.Now, TaskName= "ReceiveComponentTicket", ExeResult = postXml + "|"+ ex.Message });
                logger.SaveChanges();
                return Content("error");
            }
        }

        private void RefreshAuthorizerToken(string componentAppId, string componentAccessToken)
        {
            foreach (var authorizer in db.WechatOpenAuthorizer.Where(p => !p.IsDel).ToList())
            {
                if (authorizer.ExpiresTime < DateTime.Now.AddMinutes(30))//判断到accesstoken即将过期 马上获取新accesstoken
                {
                    try
                    {
                        var result = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.ApiAuthorizerToken(componentAccessToken, componentAppId, authorizer.AuthorizerAppId, authorizer.AuthorizerRefreshToken);
                      
                        authorizer.AuthorizerRefreshToken = result.authorizer_refresh_token;
                        authorizer.AddTime = DateTime.Now;
                        authorizer.ExpiresIn = result.expires_in;
                        authorizer.ExpiresTime = DateTime.Now.AddSeconds(result.expires_in);
                        db.Update(authorizer);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.Add(new TaskLog() { AddTime = DateTime.Now, ExeResult = authorizer.AuthorizerAppId + ":" + ex.Message, TaskName = "RefreshAuthorizerToken" });
                        logger.SaveChanges();
                    }
                }
            }

        }

        [HttpGet]
        public CategroyInfo GetCategory(string authorizerAccessToken)
        {
            return CodeApi.GetCategory(authorizerAccessToken).category_list[0];
        }

        /// <summary>
        /// 为小程序创建开放平台并绑定
        /// </summary>
        /// <param name="appId">小程序appid</param>
        /// <param name="accessToken">小程序accesstoken</param>
        /// <returns></returns>
        [HttpGet]
        public WebAPIResult CreateAndBindOpen(string appId)
        {
            WebAPIResult result = new WebAPIResult() { Success = true };
            string openAppId = string.Empty;
            try
            {
               
                var author = db.WechatOpenAuthorizer.FirstOrDefault(p => p.AuthorizerAppId == appId && !p.IsDel);
                if (author == null)
                {
                    result.Success = false;
                    result.Message = " WechatOpenAuthorizer is null";
                    return result;
                }

                var createResult = Senparc.Weixin.Open.MpAPIs.Open.OpenApi.Create(author.AuthorizerAccessToken, appId);
                if (createResult.errcode != ReturnCode.请求成功 && createResult.errcode != ReturnCode.该公众号_小程序已经绑定了开放平台帐号)
                {
                    result.Success = false;
                    result.Message = createResult.errmsg;
                }
                else if (createResult.errcode == ReturnCode.请求成功)
                {
                    openAppId = createResult.open_appid;
                 
                }
             
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                if (ex.Message.Contains("89000"))
                {
                    result.Success = true;
                }
            }
            var bind = db.WechatOpenBind.FirstOrDefault(p => p.AppId == appId);
            if (bind == null)
            {
                bind = new WechatOpenBind()
                {                    
                    AppId = appId,
                    OpenAppId = openAppId
                };
                db.Add(bind);
            }
            bind.BindDesc = result.Message;
            bind.AddTime = DateTime.Now;
            bind.IsBind = result.Success;
            db.SaveChanges();

            return result;
        }

        [HttpGet]
        public void CreateAndBindAllOpen()
        {
            foreach (var auth in db.WechatOpenAuthorizer)
            {
                var bind = db.WechatOpenBind.FirstOrDefault(p => p.AppId == auth.AuthorizerAppId);
                if (bind == null || !bind.IsBind)
                {
                    CreateAndBindOpen(auth.AuthorizerAppId);
                }
            }

        }

        [HttpGet]
        public string Test()
        {
            return "ok";
        }

        public string SendHttp(HttpMethod method, string url, string jsonData = "")
        {
            HttpClient myHttpClient = new HttpClient();
            //设置RequestMessage
            HttpRequestMessage RequestMessage = new HttpRequestMessage();
            RequestMessage.Method = method;
            RequestMessage.RequestUri = new Uri(url);
            if (!string.IsNullOrEmpty(jsonData))
            {
                RequestMessage.Content = new StringContent(jsonData);
                RequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            HttpResponseMessage ResponseMessage = myHttpClient.SendAsync(RequestMessage).Result;
            string responseJson = ResponseMessage.Content.ReadAsStringAsync().Result;
            return responseJson;
        }

    }



}