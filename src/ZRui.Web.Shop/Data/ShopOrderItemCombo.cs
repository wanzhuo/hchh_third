using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 套餐订单
    /// </summary>
    public class ShopOrderItemCombo : EntityBase
    {
        public int ShopCommodityComboId { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string ComboName { get; set; }
        /// <summary>
        /// 销售价,单位是分
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 市场价,单位是分
        /// </summary>
        public int MarketPrice { get; set; }
    }
}