using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Pay
{
    public class RefundArgsModel
    {
        public int OrderId { get; set; }
        public string ShopFlag { get; set; }
        public string TradeNo { get; set; }

        public OrderType OrderType { get; set; }

        public decimal Amount { get; set; }
    }

}
