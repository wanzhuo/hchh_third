using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 固定套餐内容
    /// </summary>
    public class ShopBrandFixComboItem : EntityBase
    {
        /// <summary>
        /// 关联套餐
        /// </summary>
        [ForeignKey("ComboId")]
        public ShopBrandCombo ShopBrandCombo { get; set; }
        /// <summary>
        /// 关联套餐Id
        /// </summary>
        public int ComboId { get; set; }
        public string CommodityName { get; set; }
        public int Count { get; set; }
        public string Sku { get; set; }
        public decimal SalePrice { get; set; }
    }
}