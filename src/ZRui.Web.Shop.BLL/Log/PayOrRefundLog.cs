using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.Log
{
    /// <summary>
    /// 支付 退款日志
    /// </summary>
    public class PayOrRefundLog
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public PayOrRefundType PayOrRefundType { get; set; }
        /// <summary>
        /// 发起参数
        /// </summary>
        public string InitiateParameter { get; set; }
        /// <summary>
        /// 接收参数
        /// </summary>
        public string NotifyParameter { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }


    }

    public enum PayOrRefundType
    {
        支付 = 1,
        退款 = 2
    }
}
