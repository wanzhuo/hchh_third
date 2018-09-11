using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 套餐类别
    /// </summary>
    public class ShopComboCategory : EntityBase
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
        public string Name { get; set; }
    }
}