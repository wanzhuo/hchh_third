using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web
{
    public class MemberBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public virtual string Truename { get; set; }
    }
    public class Member : MemberBase
    {
        public string Mobile { get; set; }

        public string UnionId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 邮箱是否已经绑定
        /// </summary>
        public virtual bool EmailIsValid { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public virtual string Avatar { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string NickName { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public virtual DateTime RegTime { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public virtual string RegIP { get; set; }
        /// <summary>
        /// 最后登陆时间
        /// </summary>
        public virtual DateTime LastLoginTime { get; set; }
        /// <summary>
        /// 最后登陆IP
        /// </summary>
        public virtual string LastLoginIP { get; set; }
        /// <summary>
        /// 登陆次数
        /// </summary>
        public virtual int LoginCount { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual MemberStatus Status { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual bool IsDel { get; set; }
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum MemberStatus
    {
        正常 = 1, 停用 = 4
    }

    public class MemberQueryCondition
    {
        public string Username { get; set; }
        public MemberStatus? MemberStatus { get; set; }
    }

    public static class MemberDbContextExtention
    {
        public static Member AddToMember(this DbContext context, Member model)
        {
            if (context.MemberEmailIsUsed(model.Email))
            {
                throw new Exception("Email is Used");
            }
            context.Set<Member>().Add(model);
            return model;
        }

        public static bool MemberEmailIsUsed(this DbContext context, string email)
        {
            return context.GetMemberIdForEmail(email) > 0;
        }

        public static int GetMemberIdForEmail(this DbContext context, string email)
        {
            return context.Set<Member>()
                .Where(m => m.Email == email)
                .Where(m => !m.IsDel)
                .Select(m=>m.Id)
                .FirstOrDefault();
        }


        public static Member GetSingleMember(this DbContext context, int id)
        {
            return context.Set<Member>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static Member GetSingleMember(this DbContext context, string memberFlag)
        {
            if (!memberFlag.StartsWith("member")) throw new Exception("人员标识形如member1");
            var id = int.Parse(memberFlag.Replace("member", ""));
            return context.Set<Member>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static MemberBase GetSingleMemberBase(this DbContext context, string memberFlag)
        {
            if (!memberFlag.StartsWith("member")) throw new Exception("人员标识形如member1");
            var id = int.Parse(memberFlag.Replace("member", ""));
            return context.Set<Member>().Where(m => m.Id == id).Select(m => new MemberBase()
            {
                Id = m.Id,
                Truename = m.Truename
            }).FirstOrDefault();
        }

        public static string GetMemberTruename(this DbContext context, string memberFlag)
        {
            var memberBase = context.GetSingleMemberBase(memberFlag);
            if (memberBase == null) throw new Exception("人员记录不存在");
            return memberBase.Truename;
        }

        public static EntityEntry<Member> DeleteMember(this DbContext context, int id)
        {
            var model = context.GetSingleMember(id);
            if (model != null)
            {
                return context.Set<Member>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<Member> QueryMember(this DbContext context)
        {
            return context.Set<Member>().AsQueryable();
        }

        public static DbSet<Member> MemberDbSet(this DbContext context)
        {
            return context.Set<Member>();
        }
    }
}
