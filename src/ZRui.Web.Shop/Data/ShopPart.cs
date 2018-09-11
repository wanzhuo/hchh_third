using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺的内部区域，这里指餐桌或者包间等。
    /// </summary>
    public class ShopPart : EntityBase
    {
        /// <summary>
        /// 标识，订单编号
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 标题，可以理解为房号或者桌号
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }
}