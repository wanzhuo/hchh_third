using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    public class ThirdMoneyReport
    {
        public int Id { get; set; }

        public int ShopId { get; set; }
        public string OrderNumber { get; set; }
        /// <summary>
        /// 运费/违约金
        /// </summary>
        public double Amount { get; set; }

        public DateTime AddTime { get; set; }
        /// <summary>
        /// 费用产生类型
        /// </summary>
        public ProduceType ProduceType { get; set; }
    }

    public enum ProduceType
    {
        发起订单 = 0,
        取消订单 = 2
    }
}
