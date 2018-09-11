using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺的外卖配送信息
    /// </summary>
    public class ShopSelfHelpInfo : EntityBase
    {
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
        /// 是否有餐盒费
        /// </summary>
        public bool HasBoxFee { get; set; }
        /// <summary>
        /// 餐盒费
        /// </summary>
        public int BoxFee { get; set; }
        /// <summary>
        /// 是否支持外带
        /// </summary>
        public bool HasTakeOut { get; set; }
    }
}