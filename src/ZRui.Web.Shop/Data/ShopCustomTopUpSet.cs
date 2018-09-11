using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 自定义金额充值设置
    /// </summary>
    public class ShopCustomTopUpSet : EntityBase
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
        /// 起充金额
        /// </summary>
        public int StartAmount { get; set; }

        /// <summary>
        /// 满足金额
        /// </summary>
        public int MeetAmount { get; set; }


        /// <summary>
        /// 额外赠送（百分比）
        /// </summary>
        public double Additional { get; set; }
    }
}
