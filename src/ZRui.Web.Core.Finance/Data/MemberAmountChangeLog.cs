using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web
{
    public partial class MemberAmountChangeLog
    {
        public virtual Int32 Id { get; set; }
        public virtual Int32 MemberId { get; set; }
        public virtual String Title { get; set; }
        public virtual String Detail { get; set; }
        public virtual Int64 OriginalAmount { get; set; }
        public virtual Int64 NowAmount { get; set; }
        public virtual Int64 Amount { get; set; }
        public virtual DateTime AddTime { get; set; }
        public virtual FinaceType Type { get; set; }
    }

    public enum FinaceType
    {
        投资回款 = 1,
        投资支出 = 2,
        借入 = 3,
        借入还款 = 4,
        充值入账 = 5,
        提现出账 = 6,
        奖励收入 = 7,
        商品购买支出 = 8,
        借入服务费 = 9,
        利息管理费支出 = 11,
        提现失败返还 = 12,
        逾期还款 = 13,
        逾期滞纳金 = 14,
        退款 = 15
    }

    public static class MemberAmountChangeLogDbContextExtention
    {
        public static MemberAmountChangeLog AddToMemberAmountChangeLog(this DbContext context, MemberAmountChangeLog model)
        {
            context.Set<MemberAmountChangeLog>().Add(model);
            return model;
        }

        public static MemberAmountChangeLog GetSingleMemberAmountChangeLog(this DbContext context, int id)
        {
            return context.Set<MemberAmountChangeLog>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<MemberAmountChangeLog> DeleteMemberAmountChangeLog(this DbContext context, int id)
        {
            var model = context.GetSingleMemberAmountChangeLog(id);
            if (model != null)
            {
                return context.Set<MemberAmountChangeLog>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberAmountChangeLog> QueryMemberAmountChangeLog(this DbContext context)
        {
            return context.Set<MemberAmountChangeLog>().AsQueryable();
        }

        public static DbSet<MemberAmountChangeLog> MemberAmountChangeLogDbSet(this DbContext context)
        {
            return context.Set<MemberAmountChangeLog>();
        }
    }
}