using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web.Core.Wechat
{
    public class MemberWeChatLoginTask
    {
        public virtual int Id { get; set; }
        public virtual string ClientId { get; set; }
        /// <summary>
        /// 验证码，这里表现为场景Id
        /// </summary>
        public virtual string Code { get; set; }

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
        public virtual MemberWeChatLoginTaskStatus Status { get; set; }
    }

    public enum MemberWeChatLoginTaskStatus
    {
        扫二维码进行中,
        扫二维码完成,
        登陆完成
    } 

    public class MemberWeChatLoginTaskQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class MemberWeChatLoginTaskDbContextExtention
    {
        public static MemberWeChatLoginTask AddToMemberWeChatLoginTask(this DbContext context, MemberWeChatLoginTask model)
        {
            context.Set<MemberWeChatLoginTask>().Add(model);
            return model;
        }

        public static MemberWeChatLoginTask GetSingleMemberWeChatLoginTask(this DbContext context, int id)
        {
            return context.Set<MemberWeChatLoginTask>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static MemberWeChatLoginTask GetSingleMemberWeChatLoginTask(this DbContext context, string openId)
        {
            return context.Set<MemberWeChatLoginTask>().Where(m => m.OpenId == openId).FirstOrDefault();
        }

        public static IQueryable<MemberWeChatLoginTask> QueryMemberWeChatLoginTask(this DbContext context)
        {
            return context.Set<MemberWeChatLoginTask>().AsQueryable();
        }

        public static DbSet<MemberWeChatLoginTask> MemberWeChatLoginTaskDbSet(this DbContext context)
        {
            return context.Set<MemberWeChatLoginTask>();
        }
    }
}
