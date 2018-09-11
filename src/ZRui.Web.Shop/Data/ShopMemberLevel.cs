using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{

    /// <summary>
    /// 商铺会员等级
    /// </summary>
    public class ShopMemberLevel : EntityBase
    {
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; }
        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public string MemberLevel { get; set; }

        /// <summary>
        /// 最高积分
        /// </summary>
        public int MaxIntegral { get; set; }

        /// <summary>
        /// 最低积分
        /// </summary>
        public int MinIntegral { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
