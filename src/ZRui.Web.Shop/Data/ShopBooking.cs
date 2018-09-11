using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 排队叫号
    /// </summary>
    public class ShopBooking : EntityBase
    {
        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int MemberId { get; set; }
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
        /// 人数
        /// </summary>
        public string Users { get; set; }
        /// <summary>
        /// 联系人昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 就餐时间
        /// </summary>
        public DateTime DinnerTime { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 确认失败的原因
        /// </summary>
        public string RefuseReason { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ShopBookingStatus Status { get; set; }
        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }
    }
    /// <summary>
    /// 排队叫号的状态
    /// </summary>
    public enum ShopBookingStatus
    {
        取消 = -1,
        待确认 = 0,
        确认成功 = 1,
        确认失败 = 4
    }


}
