using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌
    /// </summary>
    public class ShopBrand : EntityBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 一些说明
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
        /// <summary>
        /// 状态
        /// </summary>
        public ShopBrandStatus Status { get; set; }
    }

    /// <summary>
    /// 商铺品牌状态
    /// </summary>
    public enum ShopBrandStatus
    {
        正常 = 0,
        停用 = -1
    }
}
