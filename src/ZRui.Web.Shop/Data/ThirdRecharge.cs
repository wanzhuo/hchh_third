using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    /// <summary>
    /// 第三方配送充值
    /// </summary>
    public class ThirdRecharge
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 达达商户ID
        /// </summary>
        public int DaDaShopId { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public RechargeType RechargeType { get; set; }
        /// <summary>
        /// 验证code
        /// </summary>
        public string VerificationCode { get; set; }

    }

    public enum RechargeType {
        运费账户 = 0,
        红包账户 = 1
    }
}
