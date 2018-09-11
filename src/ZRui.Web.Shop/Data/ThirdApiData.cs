using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    /// <summary>
    /// 达达配送API数据记录
    /// </summary>
    public class ThirdApiData
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
        /// 达达配送发起订单ID
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public string JsonData { get; set; }
        /// <summary>
        /// 达达返回结果数据
        /// </summary>
        public string ResultData { get; set; }

        public DateTime AddTime { get; set; }
    }
}
