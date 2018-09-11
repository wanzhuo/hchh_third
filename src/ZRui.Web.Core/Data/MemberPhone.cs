using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    /// <summary>
    /// 会员绑定手机实体类
    /// </summary>
    public class MemberPhone : EntityBase
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 会员实体类
        /// </summary>
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public MemberPhoneState State { get; set; }
    }

    public enum MemberPhoneState
    {
        已绑定 = 1, 已解绑 = 2
    }

    public static class MemberPhoneDbContextExtention
    {
        public static bool IsBindMemberPhone(this DbContext db, string phone)
        {
            return db.Query<MemberPhone>()
                .Where(m => !m.IsDel)
                .Where(m => m.Phone == phone)
                .Where(m => m.State == MemberPhoneState.已绑定).Count() > 0;
        }

        public static int GetMemberIdByMemberPhone(this DbContext db, string phone)
        {
            return db.Query<MemberPhone>()
                .Where(m => !m.IsDel)
                .Where(m => m.Phone == phone)
                .Where(m => m.State == MemberPhoneState.已绑定)
                .Select(m => m.MemberId)
                .FirstOrDefault();
        }
    }
}
