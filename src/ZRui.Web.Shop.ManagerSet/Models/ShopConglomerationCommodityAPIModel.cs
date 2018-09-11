using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZRui.Web.Models
{
    public class ShopConglomerationCommodityAPIModel
    {
    }

    /// <summary>
    /// 拼团活动商品业务实体
    /// </summary>
    public class ConglomerationCommodityModel : ModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 市场价格
        /// </summary>
        [Required]
        public int MarketPrice { get; set; }

        /// <summary>
        /// 商品简介 （富文本）
        /// </summary>
        [Required]
        public string Summary { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        [Required]
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        public ActivityModel ConglomerationActivity { get; set; }

        /// <summary>
        /// 商铺Id
        /// </summary>
        public int ShopId { get; set; }
    }
}
