using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{

    /// <summary>
    /// 会员设置
    /// </summary>
    public class ShopMemberSet : EntityBase
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
        /// 是否保留等级
        /// </summary>
        public bool IsSavaLevel { get; set; }

        /// <summary>
        /// 充值折扣
        /// </summary>
        public bool IsTopUpDiscount { get; set; }

        /// <summary>
        /// 消费积分
        /// </summary>
        public bool IsConsumeIntegral { get; set; }

        /// <summary>
        /// 充值积分
        /// </summary>
        public bool IsTopUpIntegral { get; set; }

        /// <summary>
        /// 消费金额
        /// </summary>
        public int ConsumeAmount { get; set; }


        /// <summary>
        /// 获取积分
        /// </summary>
        public int GetIntegral { get; set; }

        /// <summary>
        /// 是否启用自定义充值赠送
        /// </summary>
        public bool IsShowCustomTopUpSet { get; set; }


    }
}
