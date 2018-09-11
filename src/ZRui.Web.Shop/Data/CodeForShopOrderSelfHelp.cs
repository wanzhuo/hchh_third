using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 排队叫号
    /// </summary>
    public class CodeForShopOrderSelfHelp : EntityBase
    {
        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的店铺的Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联的产品的Id
        /// </summary>
        public int CurrentNumber { get; set; }
    }
}
