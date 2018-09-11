using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    /// <summary>
    /// 线下支付记录
    /// </summary>
    public class OfflinePayRecording
    {
        public int Id { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public int TotalFee { get; set; }
        /// <summary>
        /// 交易终端IP
        /// </summary>
        public string MchCreateIp { get; set; }
        /// <summary>
        /// 客户凭证
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TimeStart { get; set; }
        /// <summary>
        /// 交易过期时间
        /// </summary>
        public DateTime TimeExpire { get; set; }
        /// <summary>
        /// 操作员Id
        /// </summary>
        public int OpUserId { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string OpShopId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string OpDeviceId { get; set; }

    }
}
