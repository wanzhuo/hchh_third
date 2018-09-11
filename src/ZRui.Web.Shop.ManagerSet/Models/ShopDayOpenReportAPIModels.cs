using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    /// <summary>
    /// 店铺日营业报表模型
    /// </summary>
    public class ShopDayOpenReportAPIModels
    {
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string ShopName { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        public decimal SalesAmount { get; set; }
        /// <summary>
        /// 外卖
        /// </summary>
        public decimal TakeawayAmount { get; set; }
        /// <summary>
        /// 扫码
        /// </summary>
        public decimal ScanCodeAmount { get; set; }
        /// <summary>
        /// 自助
        /// </summary>
        public decimal SelfHelpAmount { get; set; }
        /// <summary>
        /// 拼团
        /// </summary>
        public decimal FightGroupAmount { get; set; }
        /// <summary>
        /// 满减
        /// </summary>
        public decimal FullReductionAmount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>
        /// 微信支付总金额
        /// </summary>
        public decimal WeChatAmount { get; set; }
        /// <summary>
        /// 余额消费总金额
        /// </summary>
        public decimal BalanceAmount { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ShopDayOpenReportAPIArgModels {
        public int ShopId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
