using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 订单的外卖信息
    /// </summary>
    public class ShopOrderTakeout : EntityBase
    {
        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("ShopOrderId")]
        public ShopOrder ShopOrder { get; set; }
        /// <summary>
        /// 关联的订单Id
        /// </summary>
        public int ShopOrderId { get; set; }
        /// <summary>
        /// 下单方式
        /// </summary>
        public TakeWay TakeWay { get; set; }
        /// <summary>
        /// 自提时间
        /// </summary>
        public DateTime PickupTime { get; set; }
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 外卖状态
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }

    }

    public enum TakeWay
    {
        自提 = 0,
        送货上门 = 1
    }

    public enum Status
    {
        待确认 = 0,
        待配送 = 10,
        配送中 = 20,
        待取消 = 30,
        已取消 = 40,
        已完成 = 50,
        
    }
}
