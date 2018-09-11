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
using Microsoft.Extensions.Logging;
using System.Text;
using Senparc.Weixin.Utilities.WeixinUtility;
using Senparc.Weixin.Open.Entities;
using Newtonsoft.Json;
using ZRui.Web.Core.Wechat.Open;
using ZRui.Web.BLL;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("[controller]/[action]")]
    public class ShopWechatOpenOAuthController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        static ILogger logger;
        public ShopWechatOpenOAuthController(IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            logger = loggerFactory.CreateLogger<ShopWechatOpenOAuthController>();
        }

        /// <summary>
        /// 公众号授权页入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int shopId)
        {
            //给到一个shopId
            ViewData.Model = shopId;
            return View();
        }

        /// <summary>
        /// 发起授权页的体验URL
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GotoOAuth(int shopId)
        {
            if (shopId <= 0) throw new ArgumentNullException("shopId");
            CheckShopActor(shopId, ShopActorType.超级管理员);
            //确认当前shopId是否已经绑定了小程序
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .FirstOrDefault();
            if (model != null) throw new Exception("当前店铺绑定的小程序已经存在");
            //获取预授权码

            //var preAuthCode = ComponentContainer.TryGetPreAuthCode(wechatOpenOptions.AppId, wechatOpenOptions.AppSecret, true);\
            string componentAccessToken = ZRui.Web.BLL.AuthorizerHelper.GetComponentAccessToken();

            var log = BLL.DbContextFactory.LogDbContext;         
            var preAuthCode = ComponentApi.GetPreAuthCode(wechatOpenOptions.AppId, componentAccessToken, 0x2710).pre_auth_code;

            var callbackUrl = $"http://manager.91huichihuihe.com/ShopWechatOpenOAuth/OAuthCallback?shopId={shopId}";//成功回调地址
            var url = ComponentApi.GetComponentLoginPageUrl(wechatOpenOptions.AppId, preAuthCode, callbackUrl);
            return Redirect(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auth_code"></param>
        /// <param name="expires_in"></param>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OAuthCallback(string auth_code, int expires_in, int shopId)
        {
            logger.LogInformation($"================================调试开始====================================");
            try
            {            
                #region 查询授权信息
                var componentAppId = wechatOpenOptions.AppId;
                var authorizationCode = auth_code;              
                var accessToken = ZRui.Web.BLL.AuthorizerHelper.GetComponentAccessToken();
                var queryAuthResult = ComponentApi.QueryAuth(accessToken, componentAppId, authorizationCode);
                logger.LogInformation($"授权返回信息queryAuthResult：{queryAuthResult}");
                var authorizerAppid = queryAuthResult.authorization_info.authorizer_appid;
                var authorizationInfo = queryAuthResult.authorization_info;
                #endregion
                
                WechatOpenAuthorizer authorizer = null;
                var authorizers = db.Query<WechatOpenAuthorizer>().
                    Where(p =>p.AuthorizerAppId == authorizationInfo.authorizer_appid);
                if (authorizers.Count() > 0)
                {
                    authorizer = authorizers.FirstOrDefault(p => !p.IsDel);
                    if (authorizer != null)
                    {
                        return Content("当前店铺绑定的小程序已经存在");
                    }
                    else
                    {
                        authorizer = authorizers.OrderByDescending(p => p.Id).FirstOrDefault();
                        authorizer.IsDel = false;
                    }
                }
                else
                {
                    authorizer = new WechatOpenAuthorizer();
                    db.Add(authorizer);
                }


                authorizer.AddIp = GetIp();
                authorizer.AddTime = DateTime.Now;
                authorizer.AddUser = GetUsername();
                authorizer.AuthorizerAppId = queryAuthResult.authorization_info.authorizer_appid;
                authorizer.AuthorizerAccessToken = queryAuthResult.authorization_info.authorizer_access_token;
                authorizer.AuthorizerRefreshToken = queryAuthResult.authorization_info.authorizer_refresh_token;
                authorizer.ExpiresIn = queryAuthResult.authorization_info.expires_in;
                authorizer.ExpiresTime = DateTime.Now.AddSeconds(queryAuthResult.authorization_info.expires_in);


                GetAuthorizerInfoResult authorizerInfoResult = ComponentApi.GetAuthorizerInfo(accessToken, componentAppId, authorizerAppid, 0x2710);

                authorizer.AuthorizerNickname = authorizerInfoResult.authorizer_info.nick_name;
                //这里的Username是原始Id
                authorizer.AuthorizerUsername = authorizerInfoResult.authorizer_info.user_name;


                db.SaveChanges();



                ShopWechatOpenAuthorizer shopAuth = null;
                var shopAuths = db.Query<ShopWechatOpenAuthorizer>()               
                .Where(m => m.ShopId == shopId);
                if (shopAuths.Count() > 0)
                {
                    shopAuth = shopAuths.FirstOrDefault(p => !p.IsDel);
                    if(shopAuth==null)
                    {
                        shopAuth = shopAuths.OrderByDescending(p => p.Id).FirstOrDefault();
                    }
                    shopAuth.IsDel = false;
                   
                }
                else
                {
                    shopAuth = new ShopWechatOpenAuthorizer()
                    {
                        ShopId = shopId,
                     
                    };
                    db.Add(shopAuth);
                }
                shopAuth.WechatOpenAuthorizerId = authorizer.Id;
                db.SaveChanges();

                
                ///初始化
                //复制一份授权信息到auth数据库
               ZRui.Web.BLL.AuthorizerHelper.InsertOrUpdateAuthorizer(authorizer);
                //设置请求域以及添加跳转二维码
               var initShop = CodeApiExt.QRCodeJumpAddPublish(shopId, authorizer.AuthorizerAccessToken, hostingEnvironment);
                //创建开放平台--为了获取授权信息时含有unionid
                AuthorizerHelper.CreateAndBindOpen(authorizer.AuthorizerAppId);
             

                ViewData["QueryAuthorizationInfo"] = queryAuthResult.authorization_info;
                ViewData["GetAuthorizerInfoResult"] = authorizerInfoResult.authorizer_info;
                return View();
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ShopReceiverOAuth(string code, int state)
        {
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(wechatTemplateSendOptions.AppId,
                wechatTemplateSendOptions.AppSecret, code);
            string message = "faile";
            ShopOrderReceiver model = null;
            Senparc.Weixin.MP.AdvancedAPIs.OAuth.OAuthUserInfo userInfo = null;
            try
            {
                if (result != null && !string.IsNullOrEmpty(result.openid))
                {
                    string openId = result.openid;
                    string access_token = result.access_token;
                    userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(access_token, openId);
                    if (userInfo != null && !string.IsNullOrEmpty(userInfo.openid))
                    {
                        model = db.Query<ShopOrderReceiver>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopId == state)
                        .Where(m => m.ReceiverOpenId == openId)
                        .FirstOrDefault();
                        if (model == null)
                        {
                            bool canUsed = db.Query<ShopOrderReceiver>()
                                .Where(m => !m.IsDel)
                                .Where(m => m.ShopId == state)
                                .Where(m => m.IsUsed)
                                .Count() < 5;
                            model = new ShopOrderReceiver()
                            {
                                ShopId = state,
                                ReceiverOpenId = openId,
                                NickName = userInfo.nickname,
                                Headimgurl = userInfo.headimgurl,
                                IsDel = false,
                                IsUsed = canUsed
                            };
                            db.AddTo(model);
                            message = "success";
                        }
                        else
                        {
                            //已成功授权过
                            model.Headimgurl = userInfo.headimgurl;
                            model.NickName = userInfo.nickname;
                            message = "authored";
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                message = NickNameTryAgain(model, state, userInfo);
                logger.LogError("授权绑定失败：{0}", e.Message);
            }
            ViewData["message"] = message;
            return View();
        }


        //字符集问题，临时解决方案
        string NickNameTryAgain(ShopOrderReceiver model, int state, Senparc.Weixin.MP.AdvancedAPIs.OAuth.OAuthUserInfo userInfo)
        {
            string message = "faile";
            try
            {
                if (model == null)
                {
                    bool canUsed = db.Query<ShopOrderReceiver>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopId == state)
                        .Where(m => m.IsUsed)
                        .Count() < 5;
                    model = new ShopOrderReceiver()
                    {
                        ShopId = state,
                        ReceiverOpenId = userInfo.openid,
                        NickName = userInfo.nickname,
                        Headimgurl = userInfo.headimgurl,
                        IsDel = false,
                        IsUsed = canUsed
                    };
                    db.AddTo(model);
                    message = "success";
                }
                else
                {
                    //已成功授权过
                    model.Headimgurl = userInfo.headimgurl;
                    model.NickName = UniCodeToUTF8(userInfo.nickname);
                    message = "authored";
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

            }
            return message;
        }


        string UniCodeToUTF8(string source)
        {

            byte[] uniByte = Encoding.Unicode.GetBytes(source);
            return Encoding.UTF8.GetString(uniByte);
        }
    }
}
