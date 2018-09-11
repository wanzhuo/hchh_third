using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 店铺会员卡信息
    /// </summary>
    public class ShopMemberCardInfo : EntityBase
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
        /// 特权说明
        /// </summary>
        public string PrivilegeExplain { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime? ValidityBeginTime { get; set; }


        /// <summary>
        /// 有效结束日期
        /// </summary>
        public DateTime? ValidityEndTime { get; set; }


        /// <summary>
        /// 是否永久有效
        /// </summary>
        public bool IsValidityLong { get; set; }

        /// <summary>
        /// 使用需知
        /// </summary>
        public string UsedKnow { get; set; }

        /// <summary>
        /// 卡封面
        /// </summary>
        public string CardCover { get; set; }


        /// <summary>
        /// 服务电话
        /// </summary>
        public string ServePhone { get; set; }
    }

}
