using Senparc.Weixin;
using Senparc.Weixin.Open.WxaAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web.BLL
{
    public class AuthorizerHelper
    {
        /// <summary>
        /// 获取第三方平台accesstoken
        /// </summary>
        /// <returns></returns>
        public static string GetComponentAccessToken()
        {            
            ///直接从数据库读取，此处待优化
            return DbContextFactory.AuthDbContext.ComponentAuthorizer.FirstOrDefault().AccessToken;
        }

        /// <summary>
        /// 获取第三方平台accesstoken
        /// </summary>
        /// <returns></returns>
        public static ComponentAuthorizer GetComponentAuthorizer()
        {
            ///直接从数据库读取，此处待优化
            return DbContextFactory.AuthDbContext.ComponentAuthorizer.FirstOrDefault();
        }

        /// <summary>
        /// 获取授权方的accesstoken
        /// </summary>
        /// <param name="authorizerAppId"></param>
        /// <returns></returns>
        public static string GetAuthorizerAccessToken(string authorizerAppId)
        {
            ///直接从数据库读取，此处待优化
            return DbContextFactory.AuthDbContext.WechatOpenAuthorizer.FirstOrDefault(p =>!p.IsDel && p.AuthorizerAppId == authorizerAppId).AuthorizerAccessToken;
        }
        public static CategroyInfo GetCategory(string authorizerAccessToken)
        {
            string url = "http://auth.91huichihuihe.com/api/auth/GetCategory?authorizerAccessToken="+ authorizerAccessToken;
             return  Senparc.Weixin.HttpUtility.Get.GetJson<CategroyInfo>(url);
        }

        public static APIResult CreateAndBindOpen(string appId)
        {
            string url = "http://auth.91huichihuihe.com/api/auth/CreateAndBindOpen?appId=" + appId;
            return Senparc.Weixin.HttpUtility.Get.GetJson<APIResult>(url);
        }

        public static void InsertOrUpdateAuthorizer(ZRui.Web.WechatOpenAuthorizer wechatOpenAuthorizer)
        {

            var db = DbContextFactory.AuthDbContext;
           ZRui.Web.BLL.WechatOpenAuthorizer authorizer =  db.WechatOpenAuthorizer.FirstOrDefault(p =>!p.IsDel && p.AuthorizerAppId == wechatOpenAuthorizer.AuthorizerAppId);
            bool isAdd = false;
            if (authorizer == null)
            {
                isAdd = true;
                authorizer = new ZRui.Web.BLL.WechatOpenAuthorizer() ;
            }
            authorizer.AuthorizerAccessToken = wechatOpenAuthorizer.AuthorizerAccessToken;
            authorizer.AuthorizerAppId = wechatOpenAuthorizer.AuthorizerAppId;
            authorizer.AuthorizerNickname = wechatOpenAuthorizer.AuthorizerNickname;
            authorizer.AuthorizerRefreshToken = wechatOpenAuthorizer.AuthorizerRefreshToken;
            authorizer.AuthorizerUsername = wechatOpenAuthorizer.AuthorizerUsername;
            authorizer.ExpiresIn = wechatOpenAuthorizer.ExpiresIn;
            authorizer.ExpiresTime = wechatOpenAuthorizer.ExpiresTime;
            authorizer.IsDel=false;

            if (isAdd)
            {
                db.Add(authorizer);
                CreateAndBindOpen(authorizer.AuthorizerAppId);
            }
            else
            {
                db.Update(authorizer);
            }
            db.SaveChanges();
        }

           
    }
}
