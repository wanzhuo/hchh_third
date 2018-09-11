using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{

    /// <summary>
    /// 固定金额充值设置
    /// </summary>
    public class ShopTopUpSet : EntityBase
    {
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; }

        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 固定充值金额
        /// </summary>
        public int FixationTopUpAmount { get; set; }

        /// <summary>
        /// 赠送金额
        /// </summary>
        public int PresentedAmount { get; set; }
    }
}
