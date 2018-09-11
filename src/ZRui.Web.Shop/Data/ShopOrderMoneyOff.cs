using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺折扣
    /// </summary>
    public class ShopOrderMoneyOff : EntityBase
    {
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
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 是否扫码点餐
        /// </summary>
        public bool IsScanCode { get; set; }
        /// <summary>
        /// 是否外卖
        /// </summary>
        public bool IsTakeout { get; set; }
        /// <summary>
        /// 是否自助点餐
        /// </summary>
        public bool IsSelfOrder { get; set; }
        /// <summary>
        /// 是否收银系统
        /// </summary>
        public bool IsCashier { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }

    public enum MoneyOffType
    {
        堂食 = 0,
        外卖 = 1,
        自助 = 2
    }

}
