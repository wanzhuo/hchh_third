using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web.Core.Wechat
{
    public class MemberWeChatBindTask
    {
        public virtual int Id { get; set; }
        public virtual string Code { get; set; }
        public virtual int MemberId { get; set; }

        /// <summary>
        /// 使用此Code的微信OpenId
        /// </summary>
        /// <returns></returns>
        public virtual string OpenId { get; set; }

        /// <summary>
        /// 添加时间，同时也是判断过期的时间，例如如果超过10分钟，则过期
        /// </summary>
        /// <returns></returns>
        public virtual DateTime AddTime { get; set; }
        public virtual string AddIp { get; set; }
        public virtual MemberWeChatBindTaskStatus Status { get; set; }
    }

    public enum MemberWeChatBindTaskStatus
    {
        未使用,
        已使用
    } 

    public class MemberWeChatBindTaskQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class MemberWeChatBindTaskDbContextExtention
    {
        public static MemberWeChatBindTask AddToMemberWeChatBindTask(this DbContext context, MemberWeChatBindTask model)
        {
            context.Set<MemberWeChatBindTask>().Add(model);
            return model;
        }

        public static MemberWeChatBindTask GetSingleMemberWeChatBindTask(this DbContext context, int id)
        {
            return context.Set<MemberWeChatBindTask>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static MemberWeChatBindTask GetSingleMemberWeChatBindTask(this DbContext context, string openId)
        {
            return context.Set<MemberWeChatBindTask>().Where(m => m.OpenId == openId).FirstOrDefault();
        }

        public static IQueryable<MemberWeChatBindTask> QueryMemberWeChatBindTask(this DbContext context)
        {
            return context.Set<MemberWeChatBindTask>().AsQueryable();
        }

        public static DbSet<MemberWeChatBindTask> MemberWeChatBindTaskDbSet(this DbContext context)
        {
            return context.Set<MemberWeChatBindTask>();
        }
    }
}
