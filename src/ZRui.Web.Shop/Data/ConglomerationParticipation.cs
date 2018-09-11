using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 参与已发起的拼团表
    /// </summary>
    public class ConglomerationParticipation : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 微信头像
        /// </summary>
        public string AvatarUrl { get; set; }


        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }



        /// <summary>
        /// 已发起拼团表FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }

        /// <summary>
        /// 关联成团信息
        /// </summary>
        [ForeignKey("ConglomerationSetUpId")]
        public virtual ConglomerationSetUp ConglomerationSetUp { get; set; }

        /// <summary>
        /// 角色（1：团长 2：团员）
        /// </summary>
        public ParticipationRole Role { get; set; }

        /// <summary>
        /// 会员FK_Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 拼团订单Id
        /// </summary>
        public int ConglomerationOrderId { get; set; }

        /// <summary>
        /// 关联订单信息
        /// </summary>
        [ForeignKey("ConglomerationOrderId")]
        public virtual ConglomerationOrder ConglomerationOrder { get; set; }
        /// <summary>
        /// 活动Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }
        /// <summary>
        /// 关联活动信息
        /// </summary>
        [ForeignKey("ConglomerationActivityId")]
        public virtual ConglomerationActivity ConglomerationActivity { get; set; }

    }

    public enum ParticipationRole
    {
        团长 = 1,
        团员 = 2
    }
}
