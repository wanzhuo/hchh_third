using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌文章
    /// </summary>
    public class ShopBrandArticle:EntityBase
    {
        /// <summary>
        /// 关联的商铺品牌
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// 关联的商铺品牌Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// 关联的文章
        /// </summary>
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }
        /// <summary>
        /// 关联的文章Id
        /// </summary>
        public int ArticleId { get; set; }
    }
}
