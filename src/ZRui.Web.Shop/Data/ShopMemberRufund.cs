using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺会员退款记录
    /// </summary>
    public class ShopMemberRufund : EntityBase
    {
        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("ShopOrderId")]
        public ShopOrder ShopOrder { get; set; }
        /// <summary>
        /// 关联订单ID
        /// </summary>
        public int ShopOrderId { get; set; }
        /// <summary>
        /// 关联商铺会员
        /// </summary>
        [ForeignKey("ShopMemberId")]
        public ShopMember ShopMember { get; set; }
        /// <summary>
        /// 关联会员ID
        /// </summary>
        public int ShopMemberId { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionTime { get; set; }
    }
}
