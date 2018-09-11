using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺会员
    /// </summary>
    public class ShopMember : EntityBase
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
        /// 关联会员
        /// </summary>
        [ForeignKey("MemberId")]
        public Member Member { get; set; }
        /// <summary>
        /// 关联会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Credits { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 会员等级Id
        /// </summary>
        public int ShopMemberLevelId { get; set; }

        /// <summary>
        /// 关联等级
        /// </summary>
        [ForeignKey("ShopMemberLevelId")]
        public ShopMemberLevel ShopMemberLevel { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }
        /// <summary>
        /// 支付密码
        /// </summary>
        public string PayPassword { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }

}
