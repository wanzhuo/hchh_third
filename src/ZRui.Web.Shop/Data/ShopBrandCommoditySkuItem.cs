using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 店铺的商品规格项
    /// </summary>
    public class ShopBrandCommoditySkuItem : EntityBase
    {
        /// <summary>
        /// 关联的Sku
        /// </summary>
        [ForeignKey("SkuId")]
        public ShopBrandCommoditySku Sku { get; set; }
        /// <summary>
        /// 关联的Sku的Id
        /// </summary>
        public int SkuId { get; set; }
        /// <summary>
        /// 关联的商铺品牌商品属性
        /// </summary>
        [ForeignKey("ParameterId")]
        public ShopBrandCommodityParameter Parameter { get; set; }
        /// <summary>
        /// 关联的商铺品牌商品属性的Id
        /// </summary>
        public int ParameterId { get; set; }
        /// <summary>
        /// 关联的商铺品牌商品属性值
        /// </summary>
        [ForeignKey("ParameterValueId")]
        public ShopBrandCommodityParameterValue ParameterValue { get; set; }
        /// <summary>
        /// 关联的商铺品牌商品属性值的Id
        /// </summary>
        public int ParameterValueId { get; set; }
    }
}
