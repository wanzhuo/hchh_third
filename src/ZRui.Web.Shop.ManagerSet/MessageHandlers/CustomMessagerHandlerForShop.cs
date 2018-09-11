using Microsoft.AspNetCore.Hosting;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZRui.Web.Core.Wechat.Open.CustomMessageHandler;
using Senparc.Weixin.MP.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.WxaAPIs;

namespace ZRui.Web.MessageHandlers
{
    public class CustomMessagerHandlerForShop : CustomMessageHandler
    {
        ShopDbContext db;
        ILogger logger;
        public CustomMessagerHandlerForShop(Stream inputStream
            , PostModel encryptPostModel
            , string appId
            , string appSecret
            , IHostingEnvironment hostingEnvironment
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , int maxRecordCount)
            : base(inputStream, encryptPostModel, appId, appSecret, hostingEnvironment, maxRecordCount)
        {
            this.db = db;
            this.logger = loggerFactory.CreateLogger<CustomMessagerHandlerForShop>();
        }


        public override IResponseMessageBase OnEvent_WeAppAuditSuccessRequest(RequestMessageEvent_WeAppAuditSuccess requestMessage)
        {
            var shopAuthorizer =
                db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.WechatOpenAuthorizer.AuthorizerUsername == requestMessage.ToUserName)
                .FirstOrDefault();
            if (shopAuthorizer != null)
            {
                //logger.LogInformation("OnEvent_WeAppAuditSuccessRequest:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(requestMessage));
                shopAuthorizer.CurrentAuditStatus = 0;//审核状态，其中0为审核成功，1为审核失败，2为审核中

                #region 审核成功后自动发布
                var authorizerAccessToken = GetAuthorizerAccessToken(shopAuthorizer.ShopId);
                var result = CodeApi.ReleaseAsync(authorizerAccessToken);
                result.Wait();
                //如果请求成功，则将请求的结果写入到数据库中
                if (result.Result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopId == shopAuthorizer.ShopId)
                        .FirstOrDefault();
                    authorizer.IsRelease = true;
                    logger.LogInformation("自动发布成功，信息:{0}", result.Result.errmsg);
                }
                else
                {
                    logger.LogInformation("自动发布失败，信息：{0}",result.Result.errmsg);

                }
                #endregion
                db.SaveChanges();
            }
               
            return base.OnEvent_WeAppAuditSuccessRequest(requestMessage);
        }

        public override IResponseMessageBase OnEvent_WeAppAuditFailRequest(RequestMessageEvent_WeAppAuditFail requestMessage)
        {
            this.logger.LogInformation("OnEvent_WeAppAuditFailRequest");
            var shopAuthorizer =
                db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.WechatOpenAuthorizer.AuthorizerUsername == requestMessage.ToUserName)
                .FirstOrDefault();
            if (shopAuthorizer != null)
            {
                shopAuthorizer.CurrentAuditStatus = 1;//审核状态，其中0为审核成功，1为审核失败，2为审核中
                shopAuthorizer.CurrentAuditFailReason = requestMessage.Reason;
                db.SaveChanges();
            }
            return base.OnEvent_WeAppAuditFailRequest(requestMessage);
        }

        string GetAuthorizerAccessToken(int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId,
                    AuthorizerAccessToken = m.WechatOpenAuthorizer.AuthorizerAccessToken,
                    ExpiresTime = m.WechatOpenAuthorizer.ExpiresTime,
                })
                .FirstOrDefault();

            if (model == null) throw new Exception("指定的纪录不存在");
            //if (model.ExpiresTime.AddMinutes(20) > DateTime.Now) return model.AuthorizerAccessToken;

            return GetAuthorizerAccessToken(model.AuthorizerAppId);
        }

        string GetAuthorizerAccessToken(string authorizerAppId)
        {
            // var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken("wx0d9f22803e745596", authorizerAppId);

            var authorizerAccessToken = ZRui.Web.BLL.AuthorizerHelper.GetAuthorizerAccessToken(authorizerAppId);
            return authorizerAccessToken;
        }

    }
}
