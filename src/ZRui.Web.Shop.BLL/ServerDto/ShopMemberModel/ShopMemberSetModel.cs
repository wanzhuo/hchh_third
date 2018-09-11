using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
    /// <summary>
    ///会员设置
    /// </summary>
    public class ShopMemberSetModel : CreateModelBaseB
    {

        /// <summary>
        /// 关联的商铺
        /// </summary>
        public Shop Shop { get; set; }
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
        public int ConsumeAmount { get { return (int)(ConsumeAmountM * 100); } }

        public decimal ConsumeAmountM { get; set; }


        /// <summary>
        /// 获取积分
        /// </summary>
        public int GetIntegral { get; set; }


        /// <summary>
        /// 是否启用自定义充值赠送
        /// </summary>
        public bool IsShowCustomTopUpSet { get; set; }


        /// <summary>
        /// 是否显示充值设置
        /// </summary>
        public bool IsShowTopUpSet { get; set; }
    }
}
