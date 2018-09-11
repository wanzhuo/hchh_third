using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 支付记录
    /// </summary>
    public class MemberPayRecording : EntityBase
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 支付金额（分）
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 支付商品类型
        /// </summary>
       // public int PayType { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string AddIP { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CarryOutTime { get; set; }
    }
}
