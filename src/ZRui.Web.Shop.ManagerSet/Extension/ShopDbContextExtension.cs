using Senparc.Weixin.Open.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web.Extension
{
    public static class ShopDbContextExtension
    {
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static string GetAuthorizerAccessToken(this ShopDbContext db,string appId, int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    m.WechatOpenAuthorizer.AuthorizerAppId,
                    m.WechatOpenAuthorizer.AuthorizerAccessToken,
                    m.WechatOpenAuthorizer.ExpiresTime,
                })
                .FirstOrDefault();

            if (model == null) throw new Exception("指定的授权纪录不存在");
            var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken(appId, model.AuthorizerAppId);
            return authorizerAccessToken;
        }
    }
}
