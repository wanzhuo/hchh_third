using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺的外卖配送信息
    /// </summary>
    public class ShopTakeOutInfo : EntityBase
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
        /// 配送范围
        /// </summary>
        public int Area { get; set; }
        /// <summary>
        /// 起送价
        /// </summary>
        public int MinAmount { get; set; }
        /// <summary>
        /// 餐盒费
        /// </summary>
        public int BoxFee { get; set; }
        /// <summary>
        /// 是否启用外卖功能
        /// </summary>
        public bool IsUseTakeOut { get; set; }
        /// <summary>
        /// 是否营业中
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 营业时间--开始
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 营业时间--结束
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 配送费
        /// </summary>
        public int DeliveryFee { get; set; }
        /// <summary>
        /// 自动接单
        /// </summary>
        public bool AutoTakeOrdre { get; set; }
        /// <summary>
        /// 自动打印
        /// </summary>
        public bool AutoPrint { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 配送方式 商家配送 = 0,达达配送 = 1
        /// </summary>
        public TakeDistributionType TakeDistributionType { get; set; }

    }

    public enum TakeDistributionType
    {
        商家配送 = 0,
        达达配送 = 1
    }
}