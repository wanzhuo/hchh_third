using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.BLL.ServerDto;
using ZRui.Web.Models;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;

namespace ZRui.Web.Utils
{
    public class ShopOrderProfile : Profile
    {

        public ShopOrderProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<ShopOrder, GetPagedListResulrModel>()
              .ForMember(m => m.ShopPartTitle, d => d.MapFrom(m => m.ShopPartId != 0 ? m.ShopPart.Title : ""))
              .ForMember(m => m.StatusStr, d => d.MapFrom(m => m.Status.ToString()))
              .ForMember(m => m.Address, d => d.MapFrom(m => ""))
              .ForMember(m => m.TakeOutPhone, d => d.MapFrom(m => ""))
              .ForMember(m => m.TakeOutName, d => d.MapFrom(m => ""))
              .ForMember(m => m.TakeOutStatus, d => d.MapFrom(m => Status.待确认))
              .ForMember(m => m.CreateTime, d => d.MapFrom(m => m.AddTime))
              .ForMember(m => m.OrderId, d => d.MapFrom(m => m.Id))
              .ForMember(m => m.OrderType, d => d.MapFrom(m =>
                       m.IsTakeOut ?
                            OrderTypeE.外卖订单
                           : m.ShopOrderSelfHelpId.HasValue ?
                            OrderTypeE.自助点餐订单
                           : OrderTypeE.扫码点餐订单
                 ))

              ;
            CreateMap<ConglomerationOrder, ConglomerationOrderListResultModel>()
              .ForMember(m => m.ConglomerationSetUpCreateTime, d => d.MapFrom(c => c.ConglomerationSetUp.CreateTime))
              .ForMember(m => m.CoverPortal, d => d.MapFrom(c => c.ConglomerationActivity.CoverPortal))
              .ForMember(m => m.ActivityName, d => d.MapFrom(c => c.ConglomerationActivity.ActivityName))
              .ForMember(m => m.AvatarUrl, d => d.MapFrom(c => ""))
              .ForMember(m => m.NickName, d => d.MapFrom(c => ""))
              .ForMember(m => m.Deliverys, d => d.MapFrom(c => $"{DateTime.Now.Year}-{c.ConglomerationActivity.DeliveryTakeTheirBeginTimeMD.ToString("MM-dd")} — {DateTime.Now.Year}/{c.ConglomerationActivity.DeliveryTakeTheirEndTimeMD.ToString("MM-dd")}   {c.ConglomerationActivity.DeliveryTakeTheirBeginTimeHM.ToString("HH:mm")}-{c.ConglomerationActivity.DeliveryTakeTheirEndTimeHM.ToString("HH:mm")}"))
              ;


            CreateMap<ConglomerationOrder, ConglomerationOrderModel>()
              .ForMember(m => m.TypeStr, d => d.MapFrom(c => c.Type.ToString()))
              .ForMember(m => m.StatusStr, d => d.MapFrom(c => c.Status.ToString()))
              .ForMember(m => m.AmountM, d => d.MapFrom(c => c.Amount / 100.00M))
              .ForMember(m => m.PaymentM, d => d.MapFrom(c => c.Payment / 100.00M))
              .ForMember(m => m.SuccessfulTime, d => d.MapFrom(c => c.ConglomerationSetUp.SuccessfulTime))
              .ForMember(m => m.OrderShopMember, d => d.MapFrom(c => new OrderShopMember() {  Name = "", Phone = "", Sex =""}))

              ;
            CreateMap<ConglomerationExpress, ConglomerationOrderConglomerationExpressModel>()
                .ForMember(m => m.ActivityDeliveryFeeM, d => d.MapFrom(c => c.ActivityDeliveryFee / 100.00M))
              ;
            CreateMap<ConglomerationOrder, GetPagedListResulrModel>()
                   .ForMember(m => m.ConglomerationSetUpStatus, d => d.MapFrom(m => m.ConglomerationSetUp.Status))
                   .ForMember(m => m.OrderId, d => d.MapFrom(m => m.Id))
                   .ForMember(m => m.OrderType, d => d.MapFrom(m => OrderTypeE.拼团订单))
                   .ForMember(m => m.StatusStr, d => d.MapFrom(m => m.Status.ToString()))
                ;


            CreateMap<ConglomerationActivityTypeModel, ConglomerationActivityType>()
              .ForMember(m => m.ConglomerationPrice, d => d.MapFrom(c => (int)(c.ConglomerationPriceM * 100)))

            ;

            CreateMap<ShopMember, ShopMemberModel>()
            .ForMember(m => m.LevelInfo, d => d.MapFrom(c => $"{c.ShopMemberLevel.LevelName}({c.ShopMemberLevel.MemberLevel})"))
             .ForMember(m => m.BalanceM, d => d.MapFrom(c => c.Balance / 100.00M))
          ;

            CreateMap<ShopMember, ExportMemberListModel>()
            .ForMember(m => m.等级, d => d.MapFrom(c => c.ShopMemberLevel == null ? "无等级" : $"{c.ShopMemberLevel.LevelName}({c.ShopMemberLevel.MemberLevel})"))
            .ForMember(m => m.生日, d => d.MapFrom(c => c.BirthDay.ToString("yyyy-MM-dd")))
            .ForMember(m => m.余额, d => d.MapFrom(c => c.Balance / 100m))
            .ForMember(m => m.手机号, d => d.MapFrom(c => c.Phone))
            .ForMember(m => m.积分, d => d.MapFrom(c => c.Credits))
            .ForMember(m => m.姓名, d => d.MapFrom(c => c.Name))
          ;

            CreateMap<ShopMemberSet, ShopMemberSetModel>()
            .ForMember(m => m.ConsumeAmountM, d => d.MapFrom(c => c.ConsumeAmount / 100.00M))
            .ForMember(m => m.IsShowTopUpSet, d => d.MapFrom(c => false))
          ;
            CreateMap<ShopTopUpSet, TopUpSetModel>()
            .ForMember(m => m.FixationTopUpAmountM, d => d.MapFrom(c => c.FixationTopUpAmount / 100.00M))
            .ForMember(m => m.PresentedAmountM, d => d.MapFrom(c => c.PresentedAmount / 100.00M))

          ;

            CreateMap<ShopCustomTopUpSet, ShopCustomTopUpSetModel>()
         .ForMember(m => m.MeetAmountM, d => d.MapFrom(c => c.MeetAmount == 0 ? "" : (c.MeetAmount / 100.00M).ToString()))
         .ForMember(m => m.StartAmountM, d => d.MapFrom(c => c.StartAmount == 0 ? "" : (c.StartAmount / 100.00M).ToString()))
         .ForMember(m => m.AdditionalS, d => d.MapFrom(c => c.Additional == 0 ? "" : c.Additional.ToString()))
          ;
            CreateMap<ConglomerationActivity, Models.ActivityModel>()
            .ForMember(m => m.MarketPriceM, d => d.MapFrom(c => c.MarketPrice / 100.00M))
            .ForMember(m => m.Type, d => d.MapFrom(c => c.ConglomerationActivityTypes.Select(g => g.Id).ToArray()))
            .ForMember(m => m.ConglomerationActivityId, d => d.MapFrom(c => c.Id))
            .ForMember(m => m.PickingSettingName, d => d.MapFrom(c => ""))
            .ForMember(m => m.ActivityDeliveryFee, d => d.MapFrom(c => c.ActivityDeliveryFee / 10000.00M))
          ;


            CreateMap<ConglomerationActivity, ConglomerationOrderActivityModel>()
           .ForMember(m => m.Deliverys, d => d.MapFrom(c => $"{DateTime.Now.Year}-{c.DeliveryTakeTheirBeginTimeMD.ToString("MM-dd")} — {DateTime.Now.Year}/{c.DeliveryTakeTheirEndTimeMD.ToString("MM-dd")}   {c.DeliveryTakeTheirBeginTimeHM.ToString("HH:mm")}-{c.DeliveryTakeTheirEndTimeHM.ToString("HH:mm")}"));
        }
    }
}
