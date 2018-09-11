using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 已发起的拼团表
    /// </summary>
    public class ConglomerationSetUp : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 所需人数
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// 当前人数
        /// </summary>
        public int CurrentMemberNumber { get; set; }

        /// <summary>
        /// 拼团状态
        /// </summary>
        public ConglomerationSetUpStatus Status { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        [ForeignKey("ConglomerationActivityId")]
        public ConglomerationActivity ConglomerationActivity { get; set; }


        /// <summary>
        /// 关联拼团类型Id
        /// </summary>
        public int ConglomerationActivityTypeId { get; set; }
        /// <summary>
        /// 关联拼团类型
        /// </summary>
        [ForeignKey("ConglomerationActivityTypeId")]
        public ConglomerationActivityType ConglomerationActivityType { get; set; }

        /// <summary>
        /// 拼团结束时间
        /// </summary>

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 发起人关联的用户Id
        /// </summary>

        public int MemberId { get; set; }


        /// <summary>
        /// 是否已经发送通知
        /// </summary>
        public bool IsSendSMS { get; set; }



        public virtual ICollection<ConglomerationParticipation> ConglomerationParticipations { get; set; }

        /// <summary>
        /// 成团时间
        /// </summary>
        public DateTime? SuccessfulTime { get; set; }
    }
    /// <summary>
    /// 已发起的拼团状态
    /// </summary>
    public enum ConglomerationSetUpStatus
    {
        未支付 = 0,
        未成团 = 1,
        已经成团 = 2, //待配送
        已完成 = 3,
        已取消 = 4
    }
}
