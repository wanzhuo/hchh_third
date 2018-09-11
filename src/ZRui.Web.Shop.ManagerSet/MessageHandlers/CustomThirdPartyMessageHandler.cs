using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.MessageHandlers;
using System.IO;
using Senparc.Weixin.Open.Entities.Request;
using Microsoft.AspNetCore.Hosting;

namespace ZRui.Web.ShopManager.MessageHandlers
{
    public class CustomThirdPartyMessageHandler : ZRui.Web.Core.Wechat.ThirdPartyMessageHandlers.CustomThirdPartyMessageHandler
    {
        ShopDbContext db;
        public CustomThirdPartyMessageHandler(Stream inputStream
            , PostModel encryptPostModel
            , IHostingEnvironment hostingEnvironment
            , ShopDbContext db)
            : base(inputStream, encryptPostModel, hostingEnvironment)
        {
            this.db = db;
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            var authorizerAppid = requestMessage.AuthorizerAppid;
            var authorizer = db.Query<WechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.AuthorizerAppId == authorizerAppid)
                .FirstOrDefault();
            if (authorizer != null)
            {
                authorizer.IsDel = true;
                var shopAuthorizer = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.WechatOpenAuthorizerId == authorizer.Id)
                    .FirstOrDefault();
                if (shopAuthorizer != null)
                {
                    shopAuthorizer.IsDel = true;
                }
                db.SaveChanges();
            }

            //取消授权
            return base.OnUnauthorizedRequest(requestMessage);
        }
    }
}
