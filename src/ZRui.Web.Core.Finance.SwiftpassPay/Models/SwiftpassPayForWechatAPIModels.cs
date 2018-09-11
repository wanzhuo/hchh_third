using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.SwiftpassPay.SwiftpassPayWechatModels
{
    public class BeginRechangeArgsModel
    {
        public int TotalFee { get; set; }
    }

    public class GetRechangeResultArgsModel
    {
        public string TradeNo { get; set; }
    }
}
