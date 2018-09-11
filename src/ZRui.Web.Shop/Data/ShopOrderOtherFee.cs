using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 订单其它费用
    /// </summary>
    public class ShopOrderOtherFee : EntityBase
    {
        /// <summary>
        /// 餐盒费
        /// </summary>
        public int BoxFee { get; set; }
        /// <summary>
        /// 配送费
        /// </summary>
        public int DeliveryFee { get; set; }
    }
}