using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 叫号的产品设定
    /// </summary>
    public class ShopCallingQueueProduct : EntityBase
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
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 餐桌规格
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 详细说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ShopCallingQueueProductStatus Status { get; set; }
    }
    /// <summary>
    /// 叫号的产品的状态
    /// </summary>
    public enum ShopCallingQueueProductStatus
    {
        正常,
        不开放
    }
}
