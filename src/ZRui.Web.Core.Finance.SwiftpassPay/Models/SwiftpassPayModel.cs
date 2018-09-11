using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class SwiftpassPayModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        public string ShopFlag { get; set; }

        ///// <summary>
        ///// 支付类型
        ///// </summary>
        //public int Type { get; set; }


        //public enum PayType
        //{
        //    外卖 = 0,
        //    堂食 = 1,
        //    拼团 = 2
        //}

        public enum StatusType {
            待支付 = 0,
            已支付 = 1
        }

    }
}
