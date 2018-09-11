using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺会员充值记录
    /// </summary>
    public class ShopMemberRecharge : EntityBase
    {
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
        /// 商铺赠送的金额
        /// </summary>
        public int PresentedAmount { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransactionTime { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        public ShopMemberTransactionStatus Status { get; set; }
    }

    public enum ShopMemberTransactionStatus
    {
        未完成 = 10,
        已完成 = 20
    }

    public enum ShopMemberTransactionType
    {
        充值 = 1
    }

}
