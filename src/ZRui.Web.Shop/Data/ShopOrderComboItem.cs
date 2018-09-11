using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 套餐内容
    /// </summary>
    public class ShopOrderComboItem : EntityBase
    {
        /// <summary>
        /// 关联的商品
        /// </summary>
        [ForeignKey("Pid")]
        public ShopBrandCommodity ParentCommodity { get; set; }
        /// <summary>
        /// 关联的商品的Id
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 关联的商品  弃用
        /// </summary>
        [ForeignKey("CommodityId")]
        public ShopBrandCommodity Commodity { get; set; }
        /// <summary>
        /// 关联的商品的Id  弃用
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// 单品名称
        /// </summary>
        public string CommodityName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        public string Sku { get; set; }
        public int SalePrice { get; set; }

    }
}
