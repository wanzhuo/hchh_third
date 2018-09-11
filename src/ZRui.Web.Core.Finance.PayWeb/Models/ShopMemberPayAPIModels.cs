using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.WechatPay.ShopMemberPayAPIModels
{


    public class BeginRechangeArgsModel
    {
        public string ShopFlag { get; set; }
        public RechargeType RechargeType { get; set; }
        public int? TopUpId { get; set; }
        public int? Amount { get; set; }
    }


    public enum RechargeType
    {
        固定金额 = 1,
        自定义金额 = 2
    }

}
