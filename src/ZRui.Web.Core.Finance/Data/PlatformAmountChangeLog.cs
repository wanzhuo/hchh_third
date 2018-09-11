using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web
{
    public partial class PlatformAmountChangeLog
    {
        public virtual Int32 Id { get; set; }
        public virtual String Title { get; set; }
        public virtual String Detail { get; set; }
        public virtual Int64 OriginalAmount { get; set; }
        public virtual Int64 NowAmount { get; set; }
        public virtual Int64 Amount { get; set; }
        public virtual DateTime AddTime { get; set; }
        public virtual PlatformFinaceType Type { get; set; }
    }

    public static class PlatformAmountChangeLogDbContextExtention
    {
        public static PlatformAmountChangeLog AddToPlatformAmountChangeLog(this DbContext context, PlatformAmountChangeLog model)
        {
            context.Set<PlatformAmountChangeLog>().Add(model);
            return model;
        }

        public static PlatformAmountChangeLog GetSinglePlatformAmountChangeLog(this DbContext context, int id)
        {
            return context.Set<PlatformAmountChangeLog>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<PlatformAmountChangeLog> DeletePlatformAmountChangeLog(this DbContext context, int id)
        {
            var model = context.GetSinglePlatformAmountChangeLog(id);
            if (model != null)
            {
                return context.Set<PlatformAmountChangeLog>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<PlatformAmountChangeLog> QueryPlatformAmountChangeLog(this DbContext context)
        {
            return context.Set<PlatformAmountChangeLog>().AsQueryable();
        }

        public static DbSet<PlatformAmountChangeLog> PlatformAmountChangeLogDbSet(this DbContext context)
        {
            return context.Set<PlatformAmountChangeLog>();
        }
    }
}