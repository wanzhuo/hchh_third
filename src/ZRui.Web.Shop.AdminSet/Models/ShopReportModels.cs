using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class ShopReportModels
    {
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 搜索条件 店铺名称/编号
        /// </summary>
        public string SearchValue { get; set; }
    }

    /// <summary>
    /// 数据总计
    /// </summary>
    public class DataReport
    {
        /// <summary>
        /// 商铺数
        /// </summary>
        public int ShopCount { get; set; }
        /// <summary>
        /// 所有商铺交易总额
        /// </summary>
        public decimal ShopTransactionMoney { get; set; }
        /// <summary>
        /// 新商铺数
        /// </summary>
        public int NewShopCount { get; set; }
    }

    public class ShopReport
    {
        public string ShopName { get; set; }

        public int TransactionCount { get; set; }

        public decimal TransactionMoney { get; set; }

        public DateTime ShopCreateTime { get; set; }
    }
}
