using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    /// <summary>
    /// 商品规格及价格
    /// </summary>
    public class ShopBrandCommoditySku : EntityBase
    {
        /// <summary>
        /// 关联的商品
        /// </summary>
        [ForeignKey("CommodityId")]
        public ShopBrandCommodity Commodity { get; set; }
        /// <summary>
        /// 关联的商品的Id
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// 标识，由产品Id+属性值1Id+属性值2Id...
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 简要描述(摘要)
        /// </summary>
        public string Summary { get; set; }
    }
}
