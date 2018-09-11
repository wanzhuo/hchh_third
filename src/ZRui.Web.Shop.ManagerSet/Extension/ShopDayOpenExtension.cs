using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Models;

namespace ZRui.Web.Extension
{
    public static class ShopDayOpenExtension
    {
        public static ShopDayOpenReportAPIModels ExShopDayOpenReportSumAmount(this ShopDbContext db, ShopDayOpenReportAPIArgModels args, Member member, Shop shop)
        {
            var MoneyOffRuleSum = 0M;
            // var MoneyOtherFeeldSum = 0M;
            //外卖
            var TakeawayAmount = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.IsTakeOut && r.Status != ShopOrderStatus.已退款)
                .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime).Select(r => r.Payment).Sum();
            var TakeawayList = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.IsTakeOut && r.Status != ShopOrderStatus.已退款)
               .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime);
            foreach (var item in TakeawayList)
            {
                if (item.MoneyOffRuleId != null)
                {
                    var MoneyOffRule = db.ShopOrderMoneyOffRules.FirstOrDefault(r => r.Id == item.MoneyOffRuleId.Value);
                    if (MoneyOffRule != null)
                    {
                        MoneyOffRuleSum += MoneyOffRule.Discount;//计算减了多少金额
                    }
                }

                //if (item.OtherFeeId != null)
                //{
                //    var OtherFee = db.ShopOrderOtherFees.FirstOrDefault(r => r.Id == item.OtherFeeId.Value && !r.IsDel);
                //    if (OtherFee != null)
                //    {
                //        MoneyOtherFeeldSum += (OtherFee.BoxFee + OtherFee.DeliveryFee);//计算每单对应的配送费和餐盒费
                //    }

                //}
            }


            //扫码
            var ScanCodeAmount = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.ShopPartId != null && !r.IsTakeOut && r.Status != ShopOrderStatus.已退款)
                .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime).Select(r => r.Payment).Sum();
            var ScanCodeList = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.ShopPartId != null && !r.IsTakeOut && r.Status != ShopOrderStatus.已退款)
    .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime);
            foreach (var item in ScanCodeList)
            {
                if (item.MoneyOffRuleId != null)
                {
                    var MoneyOffRule = db.ShopOrderMoneyOffRules.FirstOrDefault(r => r.Id == item.MoneyOffRuleId.Value);
                    if (MoneyOffRule != null)
                    {
                        MoneyOffRuleSum += MoneyOffRule.Discount;
                    }
                }

            }
            //自助
            var SelfHelpAmount = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.ShopOrderSelfHelpId.HasValue && r.Status != ShopOrderStatus.已退款)
                .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime).Select(r => r.Payment).Sum();
            var SelfHelpList = db.Set<ShopOrder>().Where(r => r.ShopId == args.ShopId && r.ShopOrderSelfHelpId.HasValue && r.Status != ShopOrderStatus.已退款)
               .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime);
            foreach (var item in SelfHelpList)
            {
                if (item.MoneyOffRuleId != null)
                {
                    var MoneyOffRule = db.ShopOrderMoneyOffRules.FirstOrDefault(r => r.Id == item.MoneyOffRuleId.Value);
                    if (MoneyOffRule != null)
                    {
                        MoneyOffRuleSum += MoneyOffRule.Discount;
                    }
                }


            }
            //拼团
            var FightGroupAmount = db.Set<ConglomerationOrder>().Where(r => r.ShopId == args.ShopId && r.Status != ShopOrderStatus.已退款)
                .Where(r => r.PayTime != null && r.PayTime.Value >= args.StartTime && r.PayTime.Value <= args.EndTime).Select(r => r.Payment).Sum();

          var weChatAmount =  db.Set<MemberTradeForRechange>().Where(r=>r.ShopId==args.ShopId&&r.PayWay==1&&r.OrderType!= OrderType.充值订单&&r.Status== MemberTradeForRechangeStatus.成功).Sum(r=>r.TotalFee);
            db.Set<ShopMemberConsume>().Where(r=>!r.IsDel);

          var  shopMemberAmount =  (from shopMemberConsume in db.ShopMemberConsumes join shopMember in db.ShopMembers on shopMemberConsume.ShopMemberId equals
            shopMember.Id where shopMember.ShopId == args.ShopId select shopMemberConsume).Sum(r=>r.Amount);

            var takeawayAmount = Convert.ToDecimal(Exvalue(TakeawayAmount == null ? 0 : TakeawayAmount.Value / 100M));
            var scancodeAmount = Convert.ToDecimal(Exvalue(ScanCodeAmount == null ? 0 : ScanCodeAmount.Value / 100M));
            var selfhelpAmount = Convert.ToDecimal(Exvalue(SelfHelpAmount == null ? 0 : SelfHelpAmount.Value / 100M));
            var fightgroupAmount = Convert.ToDecimal(Exvalue(FightGroupAmount == null ? 0 : FightGroupAmount.Value / 100M));
            // var moneyotherfeeldsum = MoneyOtherFeeldSum / 100M; //C餐盒和配送
            var moneyoffrultsum = MoneyOffRuleSum / 100M;
            //外卖 + 扫码 + 自助 + 拼团 
            var sumAmount = takeawayAmount + scancodeAmount + selfhelpAmount + fightgroupAmount; //- moneyoffrultsum;
            var shopDayOpenReportAPIModels = new ShopDayOpenReportAPIModels()
            {
                ShopName = shop.Name,
                Operator = member.Truename,
                TakeawayAmount = takeawayAmount,
                ScanCodeAmount = scancodeAmount,
                SelfHelpAmount = selfhelpAmount,
                FightGroupAmount = fightgroupAmount,
                SalesAmount = sumAmount,
                FullReductionAmount = MoneyOffRuleSum / 100M,
                WeChatAmount =  weChatAmount / 100M,
                BalanceAmount = shopMemberAmount / 100M,
                StartTime = args.StartTime,
                EndTime = args.EndTime

            };
            return shopDayOpenReportAPIModels;
        }

        public static object Exvalue(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj;


        }
    }
}
