using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web
{
    public class MemberTradeForRefund
    {
        public virtual Int32 Id { get; set; }
        public virtual Int32 MemberId { get; set; }
        /// <summary>
        /// 本站流水号
        /// </summary>
        public virtual String TradeNo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public virtual String RefundTradeNo { get; set; }
        /// <summary>
        /// 机构流水号
        /// </summary>
        public virtual String MechanismTradeNo { get; set; }
        public virtual String Title { get; set; }
        public virtual String Detail { get; set; }
        public virtual Int64 TotalFee { get; set; }
        public virtual DateTime TimeStart { get; set; }
        public virtual DateTime TimeExpire { get; set; }
        public virtual String AddUser { get; set; }
        public virtual String AddIP { get; set; }
        public virtual DateTime AddTime { get; set; }
        public virtual String PayChannel { get; set; }
        public virtual MemberTradeForRefundStatus Status { get; set; }
        public virtual String OutBank { get; set; }

        public virtual OrderType OrderType { get; set; }

        public virtual int? OrderId { get; set; }
        /// <summary>
        /// 累计退款金额 
        /// </summary>
        public virtual int CumulativeRefund { get; set; }
        /// <summary>
        /// 退款方式
        /// </summary>
        public virtual RefundType RefundType { get; set; }
    }

    /// <summary>
    /// 退款状态
    /// </summary>
    public enum MemberTradeForRefundStatus
    {
        退款中 = 0,
        成功 = 1,
        失败 = 2,
        取消 = 4
    }

    public enum RefundType
    {
        全额退款 = 0,
        部分退款 = 1
    }

    public static class MemberTradeForRefundDbContextExtention
    {
        public static MemberTradeForRefund AddToMemberTradeForRefund(this DbContext context, MemberTradeForRefund model)
        {
            context.MemberTradeForRefundDbSet().Add(model);
            return model;
        }

        public static void SetMemberTradeForRefundSuccess(this DbContext db, MemberTradeForRefund model)
        {
            if (model.Status == MemberTradeForRefundStatus.退款中)
            {
                var amounts = db.GetMemberAmountList(model.MemberId);

                model.Status = MemberTradeForRefundStatus.成功;
                var totalFee = model.TotalFee;
                var amount = amounts.GetSingle(MemberAmountType.可用现金金额);
                long OriginalAmount = 0;
                if (amount != null)
                {
                    OriginalAmount = amount.Amount;
                    amount.Amount += totalFee;
                }
                else
                {
                    amount = new MemberAmount();
                    amount.Amount = totalFee;
                    amount.AmountType = MemberAmountType.可用现金金额;
                    amount.MemberId = model.MemberId;
                    db.AddToMemberAmount(amount);
                }

                amounts.Increase(MemberAmountType.累计充值金额, totalFee);

                var amountlog = new MemberAmountChangeLog();
                amountlog.AddTime = DateTime.Now;
                amountlog.Amount = totalFee;
                amountlog.MemberId = model.MemberId;
                amountlog.Detail = "在线充值-可用现余额充值";
                amountlog.NowAmount = OriginalAmount + totalFee;
                amountlog.OriginalAmount = OriginalAmount;
                amountlog.Title = "充值";
                amountlog.Type = FinaceType.充值入账;
                db.AddToMemberAmountChangeLog(amountlog);

                #region 修改金额缓存
                amounts.UpdateMemberAmountCache();
                #endregion
            }
        }





        public static void SetMemberTradeForRefundFail(this DbContext db, MemberTradeForRefund model, string errorDesc)
        {
            if (model.Status == MemberTradeForRefundStatus.退款中)
            {
                model.Detail += "，支付失败：" + errorDesc;
                model.Status = MemberTradeForRefundStatus.失败;
            }
        }


        public static MemberTradeForRefund GetSingleMemberTradeForRefund(this DbContext context, int id)
        {
            return context.Set<MemberTradeForRefund>().Find(id);
        }

        public static EntityEntry<MemberTradeForRefund> DeleteMemberTradeForRefund(this DbContext context, int id)
        {
            var model = context.Set<MemberTradeForRefund>().Find(id);
            if (model != null)
            {
                return context.Set<MemberTradeForRefund>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberTradeForRefund> QueryMemberTradeForRefund(this DbContext context)
        {
            return context.Set<MemberTradeForRefund>().AsQueryable();
        }

        public static DbSet<MemberTradeForRefund> MemberTradeForRefundDbSet(this DbContext context)
        {
            return context.Set<MemberTradeForRefund>();
        }
    }
}