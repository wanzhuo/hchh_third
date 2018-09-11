using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace ZRui.Web
{
    public partial class MemberTradeForRechange
    {
        public virtual Int32 Id { get; set; }
        public virtual Int32 MemberId { get; set; }
        public virtual int? ShopOrderId { get; set; }
        public virtual int ShopId { get; set; }

        /// <summary>
        /// 拼团订单Id
        /// </summary>
        public virtual int? ConglomerationOrderId { get; set; }
        /// <summary>
        /// 满减规则id
        /// </summary>
        public int? MoneyOffRuleId { get; set; }
        /// <summary>
        /// 本站流水号
        /// </summary>
        public virtual String TradeNo { get; set; }
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
        public virtual MemberTradeForRechangeStatus Status { get; set; }
        public virtual String OutBank { get; set; }
        [ConcurrencyCheck]
        public virtual DateTime RowVersion { get; set; }

        public virtual OrderType OrderType { get; set; }
        public virtual int OrderId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public virtual int? PayWay { get; set; }
    }

    /// <summary>
    /// 充值状态
    /// </summary>
    public enum MemberTradeForRechangeStatus
    {
        未完成 = 0,
        成功 = 1,
        失败 = 2,
        取消 = 4
    }
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        普通订单 = 1,
        拼团订单 = 2,
        充值订单 = 3
    }

    public static class MemberTradeForRechangeDbContextExtention
    {
        public static MemberTradeForRechange AddToMemberTradeForRechange(this DbContext context, MemberTradeForRechange model)
        {
            context.MemberTradeForRechangeDbSet().Add(model);
            return model;
        }

        public static void SetMemberTradeForRechangeSuccess(this DbContext db, MemberTradeForRechange model)
        {
            if (model.Status == MemberTradeForRechangeStatus.未完成)
            {
                var amounts = db.GetMemberAmountList(model.MemberId);

                model.Status = MemberTradeForRechangeStatus.成功;
                //现不使用余额
                //var totalFee = model.TotalFee;
                //var amount = amounts.GetSingle(MemberAmountType.可用现金金额);
                //long OriginalAmount = 0;
                //if (amount != null)
                //{
                //    OriginalAmount = amount.Amount;
                //    amount.Amount += totalFee;
                //}
                //else
                //{
                //    amount = new MemberAmount();
                //    amount.Amount = totalFee;
                //    amount.AmountType = MemberAmountType.可用现金金额;
                //    amount.MemberId = model.MemberId;
                //    db.AddToMemberAmount(amount);
                //}

                //amounts.Increase(MemberAmountType.累计充值金额, totalFee);

                //var amountlog = new MemberAmountChangeLog();
                //amountlog.AddTime = DateTime.Now;
                //amountlog.Amount = totalFee;
                //amountlog.MemberId = model.MemberId;
                //amountlog.Detail = "在线充值-可用现余额充值";
                //amountlog.NowAmount = OriginalAmount + totalFee;
                //amountlog.OriginalAmount = OriginalAmount;
                //amountlog.Title = "充值";
                //amountlog.Type = FinaceType.充值入账;
                //db.AddToMemberAmountChangeLog(amountlog);

                //#region 修改金额缓存
                //amounts.UpdateMemberAmountCache();
                //#endregion
            }
        }

        public static void SetMemberTradeForRechangeFail(this DbContext db, MemberTradeForRechange model, string errorDesc)
        {
            if (model.Status == MemberTradeForRechangeStatus.未完成)
            {
                model.Detail += "，支付失败：" + errorDesc;
                model.Status = MemberTradeForRechangeStatus.失败;
            }
        }


        public static MemberTradeForRechange GetSingleMemberTradeForRechange(this DbContext context, int id)
        {
            return context.Set<MemberTradeForRechange>().Find(id);
        }

        public static EntityEntry<MemberTradeForRechange> DeleteMemberTradeForRechange(this DbContext context, int id)
        {
            var model = context.Set<MemberTradeForRechange>().Find(id);
            if (model != null)
            {
                return context.Set<MemberTradeForRechange>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberTradeForRechange> QueryMemberTradeForRechange(this DbContext context)
        {
            return context.Set<MemberTradeForRechange>().AsQueryable();
        }

        public static DbSet<MemberTradeForRechange> MemberTradeForRechangeDbSet(this DbContext context)
        {
            return context.Set<MemberTradeForRechange>();
        }
    }
}