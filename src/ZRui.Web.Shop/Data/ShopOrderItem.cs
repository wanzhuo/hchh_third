using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺订单项
    /// </summary>
    public class ShopOrderItem : EntityBase
    {
        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("ShopOrderId")]
        public ShopOrder ShopOrder { get; set; }
        /// <summary>
        /// 关联的订单的Id
        /// </summary>
        public int ShopOrderId { get; set; }
        /// <summary>
        /// 关联的库存
        /// </summary>
        [ForeignKey("CommodityStockId")]
        public ShopCommodityStock CommodityStock { get; set; }
        /// <summary>
        /// 关联的库存的Id
        /// </summary>
        public int CommodityStockId { get; set; }
        /// <summary>
        /// 商铺名字
        /// </summary>
        public string CommodityName { get; set; }
        /// <summary>
        /// 规格摘要
        /// </summary>
        public string SkuSummary { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 成本价,单位是分
        /// </summary>
        public int CostPrice { get; set; }
        /// <summary>
        /// 销售价,单位是分
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public int PrimePrice { get; set; }
        /// <summary>
        /// 市场价,单位是分  弃用
        /// </summary>
        public int MarketPrice { get; set; }
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
