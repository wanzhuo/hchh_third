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
    public partial class MemberAmount
    {
        public virtual Int32 Id { get; set; }
        public virtual MemberAmountType AmountType { get; set; }
        public virtual Int32 MemberId { get; set; }
        /// <summary>
        /// 金额，单位分
        /// </summary>
        public virtual Int64 Amount { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime RowVersion { get; set; }
    }

    public enum MemberAmountType
    {
        可用现金金额 = 0,
        奖金余额 = 1,
        新手投资专享红包 = 2,
        冻结金额 = 3,
        累计代付 = 4,

        可用回款现金金额 = 6,

        待收本息金额 = 31,
        待付本息金额 = 32,


        待确认投标 = 41,
        待确认提现 = 42,
        待确认充值 = 43,


        活动奖励_收入 = 51,
        活动奖励_支出 = 52,


        净赚利息 = 101,
        净付利息 = 102,
        累计支付会员费 = 103,
        累计支付验证费 = 104,
        累计提现手续费 = 105,


        累计投资金额 = 201,
        累计借入金额 = 202,
        累计充值金额 = 203,
        累计提现金额 = 204,
        累计支付佣金 = 205,


        待收利息总额 = 301,
        待付利息总额 = 302,

        累计支付利息管理费 = 401
    }

    public enum MemberAmountLevel
    {
        半星 = 0,
        一星 = 10,
        一星半 = 15,
        二星 = 20,
        二星半 = 25,
        三星 = 30,
        三星半 = 35,
        四星 = 40,
        四星半 = 45,
        五星 = 50
    }

    public partial class MemberAmountList : List<MemberAmount>
    {
        protected DbContext db;
        protected int memberId;
        protected MemberAmountCache memberAmountCache;
        public MemberAmount GetSingle(MemberAmountType amountType)
        {
            return this.Where(m => m.AmountType == amountType).FirstOrDefault();
        }

        public long GetAvailAmount()
        {
            return GetSingle(MemberAmountType.可用现金金额).Amount + GetSingle(MemberAmountType.可用回款现金金额).Amount;
        }

        public MemberAmount Increase(MemberAmountType amountType, long amount)
        {
            var a = GetSingle(amountType);
            a.Amount += amount;
            return a;
        }

        public MemberAmount Decrease(MemberAmountType amountType, long amount)
        {
            var a = GetSingle(amountType);
            a.Amount -= amount;
            return a;
        }

        /// <summary>
        /// 增加可用金额
        /// </summary>
        /// <param name="amount1">增加的可用现金金额</param>
        /// <param name="amount2">增加的可用回款现金金额</param>
        /// <param name="logTitle">增加的标题</param>
        /// <param name="logDetail">增加的详细</param>
        /// <param name="logFinaceType">增加的财务类型</param>
        /// <returns></returns>
        public void IncreaseAvailAmount(long amount1, long amount2, string logTitle, string logDetail, FinaceType logFinaceType)
        {
            var originalAmount = GetAvailAmount();
            Increase(MemberAmountType.可用现金金额, amount1);
            Increase(MemberAmountType.可用回款现金金额, amount2);
            var nowAmount = GetAvailAmount();

            AddToMemberAmountChangeLog(new MemberAmountChangeLog()
            {
                AddTime = DateTime.Now,
                Amount = amount1 + amount2,
                MemberId = memberId,
                Detail = logDetail,
                NowAmount = nowAmount,
                OriginalAmount = originalAmount,
                Title = logTitle,
                Type = logFinaceType
            });
        }

        /// <summary>
        /// 扣可用金额
        /// </summary>
        /// <param name="amount1">扣款的可用现金金额</param>
        /// <param name="amount2">扣款的可用回款现金金额</param>
        /// <param name="logTitle">扣款的标题</param>
        /// <param name="logDetail">扣款的详细</param>
        /// <param name="logFinaceType">扣款的财务类型</param>
        /// <returns></returns>
        public void DecreaseAvailAmount(long amount1, long amount2, string logTitle, string logDetail, FinaceType logFinaceType)
        {
            var originalAmount = GetAvailAmount();
            Decrease(MemberAmountType.可用现金金额, amount1);
            Decrease(MemberAmountType.可用回款现金金额, amount2);
            var nowAmount = GetAvailAmount();

            AddToMemberAmountChangeLog(new MemberAmountChangeLog()
            {
                AddTime = DateTime.Now,
                Amount = amount1 + amount2,
                MemberId = memberId,
                Detail = logDetail,
                NowAmount = nowAmount,
                OriginalAmount = originalAmount,
                Title = logTitle,
                Type = logFinaceType
            });
        }

        protected virtual void Init()
        {
            memberAmountCache = db.Query<MemberAmountCache>()
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();

            if (memberAmountCache == null)
            {
                memberAmountCache = new MemberAmountCache()
                {
                    MemberId = memberId
                };
                db.Add<MemberAmountCache>(memberAmountCache);
            }

            var list = db.GetMemberAmounts(memberId);

            foreach (MemberAmountType item in Enum.GetValues(typeof(MemberAmountType)))
            {
                if (list.Where(m => m.AmountType == item).Count() <= 0)
                {
                    var a = new MemberAmount
                    {
                        AmountType = item,
                        Amount = 0,
                        MemberId = memberId
                    };
                    db.Set<MemberAmount>().Add(a);
                    list.Add(a);
                }
            }

            this.AddRange(list);
        }


        public MemberAmountList(DbContext db, int memberId)
        {
            this.db = db;
            this.memberId = memberId;

            Init();
        }

        public void UpdateMemberAmountCache()
        {
            memberAmountCache.AvailAmount = this.GetAvailAmount();
            memberAmountCache.TotalRecharge = this.GetSingle(MemberAmountType.累计充值金额).Amount;
        }

        public virtual void AddToMemberAmountChangeLog(MemberAmountChangeLog log)
        {
            db.AddToMemberAmountChangeLog(log);
        }
    }

    public static class MemberAmountDbContextExtention
    {
        public static MemberAmount AddToMemberAmount(this DbContext context, MemberAmount model)
        {
            context.Set<MemberAmount>().Add(model);
            return model;
        }

        public static void InitMemberAmount(this DbContext context, int memberId)
        {
            foreach (MemberAmountType item in Enum.GetValues(typeof(MemberAmountType)))
            {
                var model = new MemberAmount()
                {
                    Amount = 0,
                    AmountType = item,
                    MemberId = memberId
                };
                context.Set<MemberAmount>().Add(model);
            }
        }

        /// <summary>
        /// 减少指定类型金额
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="amountType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static MemberAmount DecreaseMemberAmount(this DbContext context, int customerId, MemberAmountType amountType, long amount)
        {
            var model = context.GetSingleOrNewMemberAmount(amountType, customerId);
            model.Amount -= amount;
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
        public static MemberAmount IncreaseMemberAmount(this DbContext context, int customerId, MemberAmountType amountType, long amount)
        {
            var model = context.GetSingleOrNewMemberAmount(amountType, customerId);
            model.Amount += amount;
            return model;
        }

        public static MemberAmount GetSingleOrNewMemberAmount(this DbContext context, MemberAmountType amountType, int memberId)
        {
            var model = context.Set<MemberAmount>().Where(m => m.AmountType == amountType && m.MemberId == memberId).FirstOrDefault();
            if (model == null)
            {
                model = new MemberAmount
                {
                    Amount = 0,
                    AmountType = amountType,
                    MemberId = memberId
                };
                context.AddToMemberAmount(model);
            }
            return model;
        }

        public static MemberAmountList GetMemberAmountList(this DbContext context, int customerId)
        {
            var list = new MemberAmountList(context, customerId);
            return list;
        }

        public static IList<MemberAmount> GetMemberAmounts(this DbContext context, int customerId)
        {
            return context.QueryMemberAmount().Where(m => m.MemberId == customerId).ToList();
        }

        public static MemberAmount GetSingleMemberAmount(this DbContext context, int id)
        {
            return context.Set<MemberAmount>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<MemberAmount> DeleteMemberAmount(this DbContext context, int id)
        {
            var model = context.GetSingleMemberAmount(id);
            if (model != null)
            {
                return context.Set<MemberAmount>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberAmount> QueryMemberAmount(this DbContext context)
        {
            return context.Set<MemberAmount>().AsQueryable();
        }

        public static DbSet<MemberAmount> MemberAmountDbSet(this DbContext context)
        {
            return context.Set<MemberAmount>();
        }

        /// <summary>
        /// 获得指定客户的土豪指数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="customerId">客户ID</param>
        /// <returns>
        /// 土豪指数<see cref="MemberAmountLevel"/>
        /// </returns>
        public static MemberAmountLevel GetMemberAmountLevel(this DbContext context, int customerId)
        {
            var p = context.Set<MemberAmount>()
                .Where(m => m.MemberId == customerId)
                .Where(m => m.AmountType == MemberAmountType.累计充值金额)
                //.Where(m => (m.AmountType == MemberAmountType.可用回款现金金额 || m.AmountType == MemberAmountType.可用现金金额 || m.AmountType == MemberAmountType.冻结金额 || m.AmountType == MemberAmountType.待收本息金额))
                .Sum(m => m.Amount);
            var w = 10000 * 100;
            var level = MemberAmountLevel.半星;
            if (p < 1 * w)
            {
                level = MemberAmountLevel.半星;
            }
            else if (p >= 3 * w && p < 5 * w)
            {
                level = MemberAmountLevel.一星半;
            }
            else if (p >= 5 * w && p < 10 * w)
            {
                level = MemberAmountLevel.二星;
            }
            else if (p >= 10 * w && p < 15 * w)
            {
                level = MemberAmountLevel.二星半;
            }
            else if (p >= 15 * w && p < 30 * w)
            {
                level = MemberAmountLevel.三星;
            }
            else if (p >= 30 * w && p < 50 * w)
            {
                level = MemberAmountLevel.三星半;
            }
            else if (p >= 50 * w && p < 100 * w)
            {
                level = MemberAmountLevel.四星;
            }
            else if (p >= 100 * w && p < 300 * w)
            {
                level = MemberAmountLevel.四星半;
            }
            else
            {
                level = MemberAmountLevel.五星;
            }

            return level;
        }

    }
}