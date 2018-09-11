using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团活动商品表（不使用）
    /// </summary>
    public class ConglomerationCommodity : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 市场价格
        /// </summary>
        public int MarketPrice { get; set; }

        /// <summary>
        /// 商品简介 （富文本）
        /// </summary>
        public string Summary { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }


        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        [ForeignKey("ConglomerationActivityId")]
        public ConglomerationActivity ConglomerationActivity { get; set; }
    }
}
