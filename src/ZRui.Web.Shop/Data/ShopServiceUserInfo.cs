using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 客服信息(可核销拼团订单人员)
    /// </summary>
    public class ShopServiceUserInfo : EntityBase
    {
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }


        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; }
        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }


        /// <summary>
        /// Openid
        /// </summary>
        public string Openid { get; set; }



        /// <summary>
        /// Nickname
        /// </summary>
        public string Nickname { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }



        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }


        /// <summary>
        /// Unionid
        /// </summary>
        public string Unionid { get; set; }
    }

   
}
