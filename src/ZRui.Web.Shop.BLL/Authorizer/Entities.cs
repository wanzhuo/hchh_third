using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web.BLL
{
    public class ComponentAuthorizer
    {      
        [Key]
        public int Id { get; set; }    

        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string EncodingAESKey { get; set; }
        public string Token { get; set; }
        public string Ticket { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime TicketTime { get; set; }

    }


    public class WechatOpenAuthorizer
    {
        [Key]
        public int Id { get; set; }
        public bool IsDel { get; set; }

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

    public class WechatOpenBind
    {
        [Key]
        public string AppId { get; set; }
        public DateTime AddTime { get; set; }
        public string OpenAppId { get; set; }
        public bool IsBind { get; set; }
        public string BindDesc { get; set; }


    }

}
