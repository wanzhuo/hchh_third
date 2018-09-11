using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌商品属性值
    /// </summary>
    public class ShopBrandCommodityParameterValue : EntityBase
    {
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
        /// 值
        /// </summary>
        public string Value { get; set; }
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
