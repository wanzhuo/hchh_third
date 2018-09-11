using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class GetIntegralModel
    {

        /// <summary>
        /// 订单Id
        /// </summary>
        public int OrderId { get; set; }

        ///// <summary>
        ///// 是否拼团
        ///// </summary>
        //public bool IsConglomeration { get; set; }


        public SourceType SourceType { get; set; }
    }
}
