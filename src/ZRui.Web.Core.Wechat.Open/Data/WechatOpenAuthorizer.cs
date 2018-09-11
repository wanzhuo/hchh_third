using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 第三方授权方
    /// </summary>
    public class WechatOpenAuthorizer : EntityBase
    {
        /// <summary>
        /// AppId
        /// </summary>
        public string AuthorizerAppId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string AuthorizerNickname { get; set; }
        // <summary>
        /// 授权方公众号的原始Id
        /// </summary>
        public string AuthorizerUsername { get; set; }
        /// <summary>
        /// 访问Token
        /// </summary>
        public string AuthorizerAccessToken { get; set; }
        /// <summary>
        /// 刷新的Token
        /// </summary>
        public string AuthorizerRefreshToken { get; set; }
        /// <summary>
        /// 有效期,单位秒
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime ExpiresTime { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }
}
