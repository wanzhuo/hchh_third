using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌的商品类型
    /// </summary>
    public class ShopBrandCommodityCategory : EntityBase
    {
        /// <summary>
        /// 关联的商铺品牌
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// 关联的商铺品牌的Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// 商品类型名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public virtual string Flag { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Detail { get; set; }
        /// <summary>
        /// 关联的上级类型
        /// </summary>
        [ForeignKey("PId")]
        public virtual ShopBrandCommodityCategory Parent { get; set; }
        /// <summary>
        /// 关联的上级类型Id
        /// </summary>
        public virtual int? PId { get; set; }
        /// <summary>
        /// 图标，或者叫封面
        /// </summary>
        public virtual string Ico { get; set; }
        /// <summary>
        /// 排序权重
        /// </summary>
        public virtual float OrderWeight { get; set; }
        /// <summary>
        /// 标签，用于页面seo
        /// </summary>
        public virtual string Tags { get; set; }
        /// <summary>
        /// 关键字，用于页面seo
        /// </summary>
        public virtual string Keywords { get; set; }
        /// <summary>
        /// 描述，用于页面seo
        /// </summary>
        public virtual string Description { get; set; }
    }
}