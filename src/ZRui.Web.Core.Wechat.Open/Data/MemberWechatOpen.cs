using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web.Core.Wechat
{
    public class MemberWechatOpen:EntityBase
    {
        public virtual int MemberId { get; set; }
        public virtual string UnionId { get; set; }
    }

    public static class MemberWechatOpenDbContextExtention
    {
        public static MemberWechatOpen AddToMemberWechatOpen(this DbContext context, MemberWechatOpen model)
        {
            context.Set<MemberWechatOpen>().Add(model);
            return model;
        }

        public static MemberWechatOpen GetSingleMemberWechatOpen(this DbContext context, int id)
        {
            return context.Set<MemberWechatOpen>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static MemberWechatOpen GetSingleMemberWechatOpen(this DbContext context, string unionId)
        {
            return context.Set<MemberWechatOpen>().Where(m => m.UnionId == unionId).FirstOrDefault();
        }

        public static EntityEntry<MemberWechatOpen> DeleteMemberWechatOpen(this DbContext context, int id)
        {
            var model = context.GetSingleMemberWechatOpen(id);
            if (model != null)
            {
                return context.Set<MemberWechatOpen>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberWechatOpen> QueryMemberWechatOpen(this DbContext context)
        {
            return context.Set<MemberWechatOpen>().AsQueryable();
        }

        public static DbSet<MemberWechatOpen> MemberWechatOpenDbSet(this DbContext context)
        {
            return context.Set<MemberWechatOpen>();
        }
    }
}
