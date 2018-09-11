using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商圈的商店
    /// </summary>
    public class CommercialDistrictShop : EntityBase
    {
        /// <summary>
        /// 关联的商圈
        /// </summary>
        [ForeignKey("CommercialDistrictId")]
        public virtual CommercialDistrict CommercialDistrict { get; set; }
        /// <summary>
        /// 关联的商圈Id
        /// </summary>
        public virtual int CommercialDistrictId { get; set; }
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }
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
