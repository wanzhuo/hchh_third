using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
   public class SetStatusArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 新状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }
    }
}
