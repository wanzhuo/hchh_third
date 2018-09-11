using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 自助点餐实例表
    /// </summary>
    public class ShopOrderSelfHelp : EntityBase
    {
        /// <summary>
        /// 取餐号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 是否外带
        /// </summary>
        public bool IsTakeOut { get; set; }
    }
}
