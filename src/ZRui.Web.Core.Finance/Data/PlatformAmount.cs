using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web
{
    /// <summary>
    /// 客户金额
    /// </summary>
    public partial class PlatformAmount
    {
        public virtual Int32 Id { get; set; }
        public virtual PlatformAmountType AmountType { get; set; }
        public virtual Int64 Amount { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime RowVersion { get; set; }
    }
    public enum PlatformAmountType
    {
        可用现金金额 = 0
    }

    public enum PlatformFinaceType
    {
        商品购买收入 = 0,
        逾期代付支出 = 1,
        逾期代付回款 = 2,
        利息管理费收入 = 10,
        借入服务费 = 11,
        提现服务费 = 12,
        逾期滞纳金
    }

    public static class PlatformAmountDbContextExtention
    {
        public static PlatformAmount AddToPlatformAmount(this DbContext context, PlatformAmount model)
        {
            context.Set<PlatformAmount>().Add(model);
            return model;
        }

        /// <summary>
        /// 减少指定类型金额
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="amountType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static PlatformAmount DecreasePlatformAmount(this DbContext context, PlatformAmountType amountType, long amount, string logTitle, string logDetail, PlatformFinaceType logFinaceType)
        {
            var model = context.GetSingleOrNewPlatformAmount(amountType);
            var originalAmount = model.Amount;
            model.Amount -= amount;
            var nowAmount = model.Amount;

            context.AddToPlatformAmountChangeLog(new PlatformAmountChangeLog()
            {
                AddTime = DateTime.Now,
                Amount = amount,
                Detail = logDetail,
                NowAmount = nowAmount,
                OriginalAmount = originalAmount,
                Title = logTitle,
                Type = logFinaceType
            });


            return model;
        }
        /// <summary>
        /// 增加指定类型金额
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="amountType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static PlatformAmount IncreasePlatformAmount(this DbContext context, PlatformAmountType amountType, long amount, string logTitle, string logDetail, PlatformFinaceType logFinaceType)
        {
            var model = context.GetSingleOrNewPlatformAmount(amountType);
            var originalAmount = model.Amount;
            model.Amount += amount;
            var nowAmount = model.Amount;

            context.AddToPlatformAmountChangeLog(new PlatformAmountChangeLog()
            {
                AddTime = DateTime.Now,
                Amount = amount,
                Detail = logDetail,
                NowAmount = nowAmount,
                OriginalAmount = originalAmount,
                Title = logTitle,
                Type = logFinaceType
            });

            return model;
        }

        public static PlatformAmount GetSingleOrNewPlatformAmount(this DbContext context, PlatformAmountType amountType)
        {
            var model = context.Set<PlatformAmount>().Where(m => m.AmountType == amountType).FirstOrDefault();
            if (model == null)
            {
                model = new PlatformAmount
                {
                    Amount = 0,
                    AmountType = amountType
                };
                context.AddToPlatformAmount(model);
            }
            return model;
        }

        public static List<PlatformAmount> GetPlatformAmountList(this DbContext context, int customerId)
        {
            var list = context.Set<PlatformAmount>()
                .Select(m => new PlatformAmount
                {
                    Amount = m.Amount,
                    AmountType = m.AmountType
                })
                .ToList();

            foreach (PlatformAmountType item in Enum.GetValues(typeof(PlatformAmountType)))
            {
                if (list.Where(m => m.AmountType == item).Count() <= 0)
                {
                    list.Add(new PlatformAmount
                    {
                        AmountType = item,
                        Amount = 0
                    });
                }
            }

            return list;
        }

        public static PlatformAmount GetSinglePlatformAmount(this DbContext context, int id)
        {
            return context.Set<PlatformAmount>().Find(id);
        }

        public static EntityEntry<PlatformAmount> DeletePlatformAmount(this DbContext context, int id)
        {
            var model = context.Set<PlatformAmount>().Find(id);
            if (model != null)
            {
                return context.Set<PlatformAmount>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<PlatformAmount> QueryPlatformAmount(this DbContext context)
        {
            return context.Set<PlatformAmount>().AsQueryable();
        }

        public static DbSet<PlatformAmount> PlatformAmountDbSet(this DbContext context)
        {
            return context.Set<PlatformAmount>();
        }

    }
}