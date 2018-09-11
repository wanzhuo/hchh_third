using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺积分记录
    /// </summary>
    public class ShopIntegralRecharge : EntityBase
    {
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 积分来源类型
        /// </summary>
        public SourceType SourceType { get; set; }

        /// <summary>
        /// 来源说明
        /// </summary>
        public string SourceRemark { get; set; }

        /// <summary>
        /// 来源订单Id
        /// </summary>
        public int SourceOrderId { get; set; }

        //积分状态（+-）
        public int CodeStatut { get; set; }

        /// <summary>
        /// 关联会员
        /// </summary>
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }
        /// <summary>
        /// 关联会员ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 积分数量
        /// </summary>
        public int Count { get; set; }


        /// <summary>
        /// 关联会员卡信息Id
        /// </summary>
        public int ShopMemberId { get; set; }


        /// <summary>
        ///   /// <summary>
        /// 关联会员卡信息
        /// </summary>
        /// </summary>
        [ForeignKey("ShopMemberId")]
        public ShopMember ShopMember { get; set; }

    }

    /// <summary>
    /// 类型
    /// </summary>
    public enum SourceType
    {
        扫码点餐订单 = 1,
        外卖订单 = 2,
        自助点餐订单 = 3,
        拼团订单 = 4,
    }

}
