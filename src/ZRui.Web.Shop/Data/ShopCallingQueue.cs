using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 排队叫号
    /// </summary>
    public class ShopCallingQueue : EntityBase
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
        /// 关联的产品，不一定存在
        /// </summary>
        [ForeignKey("ProductId")]
        public ShopCallingQueueProduct Product { get; set; }
        /// <summary>
        /// 关联的产品的Id
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否可以拼桌
        /// </summary>
        public bool CanShareTable { get; set; }
        /// <summary>
        /// 排队的号码
        /// </summary>
        public int QueueNumber { get; set; }
        /// <summary>
        /// 排队的位置，用于过号和重新排队，如果排队的号码跟排队的位置不一致，则说明已经过号
        /// 例如，过号后，则个自动增加3，在接下来的3个位置后继续叫号
        /// </summary>
        public int QueueIndex { get; set; }
        /// <summary>
        /// 确认失败的原因
        /// </summary>
        public string RefuseReason { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ShopCallingQueueStatus Status { get; set; }
        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }

        public static string GetShopOpenStatusFlag(int shopId)
        {
            return $"ShopCallingQueue_OpenStatus_{shopId}";
        }
    }

    public enum ShopCallingQueueStatus
    {
        取消 = -1,
        待确认 = 0,
        确认成功 = 1,
        确认失败 = 4
    }


}
