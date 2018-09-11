using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 第三方配送物流信息
    /// </summary>
    public class ThirdLogisticsInfo : EntityBase
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int ShopOrderId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int ShopOrderStatus { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
