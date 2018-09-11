using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Models;
using ZRui.Web.ShopMemberTopUpAPIModel;
using ZRui.Web.ShopMemberAPIModels;
using ZRui.Web.Data;
using ZRui.Web.BLL.Third;

namespace ZRui.Web.Utils
{
    public class ShopProfile : Profile
    {

        public ShopProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<ConglomerationActivity, GetActivityPageListResultModel>()
                .ForMember(m => m.ConglomerationLowestPrice, d => d.MapFrom(c => 0))
                .ForMember(m => m.Participants, d => d.MapFrom(c => 0))
                .ForMember(m => m.ConglomerationToptPrice, d => d.MapFrom(c => 0))
                .ForMember(m => m.MarketPriceM, d => d.MapFrom(c => c.MarketPrice / 100.00M));


            CreateMap<ConglomerationSetUp, GetConglomerationSetUpResultModel>()
                .ForMember(m => m.Initiator, d => d.MapFrom(c => c.ConglomerationParticipations.FirstOrDefault(g => g.Role.Equals(ParticipationRole.团长))));

            CreateMap<ConglomerationSetUpModel, ConglomerationSetUp>()
                .ForMember(m => m.ConglomerationParticipations, d => d.MapFrom(c => new List<ConglomerationParticipation>()))
                ;

            CreateMap<ConglomerationOrder, GetOrderListResultModel>()
                .ForMember(m => m.ConglomerationSetUpCreateTime, d => d.MapFrom(c => c.ConglomerationSetUp.CreateTime))
                .ForMember(m => m.CoverPortal, d => d.MapFrom(c => c.ConglomerationActivity.CoverPortal))
                .ForMember(m => m.PaymentM, d => d.MapFrom(c => c.Payment / 100.00M))
                .ForMember(m => m.AmountM, d => d.MapFrom(c => c.Amount / 100.00M))
                .ForMember(m => m.ActivityName, d => d.MapFrom(c => c.ConglomerationActivity.ActivityName))
                .ForMember(m => m.StatusStr, d => d.MapFrom(c => c.Status.ToString()))
                .ForMember(m => m.DeliveryTakeTheirBeginTimeHM, d => d.MapFrom(c => c.ConglomerationActivity.DeliveryTakeTheirBeginTimeHM))
                .ForMember(m => m.DeliveryTakeTheirBeginTimeMD, d => d.MapFrom(c => c.ConglomerationActivity.DeliveryTakeTheirBeginTimeMD))
                .ForMember(m => m.DeliveryTakeTheirEndTimeHM, d => d.MapFrom(c => c.ConglomerationActivity.DeliveryTakeTheirEndTimeHM))
                .ForMember(m => m.DeliveryTakeTheirEndTimeMD, d => d.MapFrom(c => c.ConglomerationActivity.DeliveryTakeTheirEndTimeMD))
                .ForMember(m => m.ConglomerationExpress, d => d.MapFrom(c => new ConglomerationExpress()))
                .ForMember(m => m.SuccessfulTime, d => d.MapFrom(c => c.ConglomerationSetUp.SuccessfulTime))
                ;

            CreateMap<ConglomerationActivity, ActivityModel>()
                .ForMember(m => m.ConglomerationActivityPickingSettings, d => d.MapFrom(c => new Dictionary<string, ConsignmentType>()))
                .ForMember(m => m.MinConglomerationPriceM, d => d.MapFrom(c => c.ConglomerationActivityTypes.Min(g => g.ConglomerationPrice) / 100.00M))
                .ForMember(m => m.ConglomerationLowestPrice, d => d.MapFrom(c => c.ConglomerationActivityTypes.Min(g => g.ConglomerationPrice) / 100.00M))
                .ForMember(m => m.ConglomerationToptPrice, d => d.MapFrom(c => c.ConglomerationActivityTypes.Max(g => g.ConglomerationPrice) / 100.00M))
                ;
            CreateMap<ConglomerationSetUp, GetConglomerationSetUpDetailsResultModel>()
                .ForMember(m => m.ConglomerationPriceM, d => d.MapFrom(c => 0))
                .ForMember(m => m.ConglomerationOrderId, d => d.MapFrom(c => 0))
                .ForMember(m => m.ConglomerationActivityTypeId, d => d.MapFrom(c => 0))
                ;
            CreateMap<ShopTopUpSet, GetTopUpModel>()
                .ForMember(m => m.FixationTopUpAmountM, d => d.MapFrom(c => c.FixationTopUpAmount / 100.00M))
                .ForMember(m => m.PresentedAmountM, d => d.MapFrom(c => c.PresentedAmount / 100.00M))
            ;
            CreateMap<ShopCustomTopUpSet, GetCustomTopUpModel>()
            .ForMember(m => m.StartAmountM, d => d.MapFrom(c => c.StartAmount / 100.00M))
            .ForMember(m => m.MeetAmountM, d => d.MapFrom(c => c.MeetAmount / 100.00M))
            .ForMember(m => m.IsShowCustomTopUpSet, d => d.MapFrom(c => false))
        ;
            CreateMap<ShopMember, GetSingleModel>()
                 .ForMember(m => m.LevelName, d => d.MapFrom(c => c.ShopMemberLevel == null ? "无等级" : $"{c.ShopMemberLevel.LevelName}"))
                 .ForMember(m => m.MemberLevel, d => d.MapFrom(c => c.ShopMemberLevel == null ? "" : $"{c.ShopMemberLevel.MemberLevel}"))
        ;
            CreateMap<ThirdOrder, CThirdOrdersModel>().ForMember(m => m.CancelFromStr, d => d.MapFrom(m => m.CancelFrom))
         .ForMember(m => m.OrderStatusStr, d => d.MapFrom(m => m.OrderStatus))
         .ForMember(m => m.ThirdUpdateTime, d => d.MapFrom(m => m.ConvertStringToDateTime()));

        }
    }
}
