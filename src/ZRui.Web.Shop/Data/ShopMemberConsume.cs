using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺会员消费记录
    /// </summary>
    public class ShopMemberConsume : EntityBase
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
        /// 交易时间
        /// </summary>
        public DateTime TransactionTime { get; set; }
    }
}
