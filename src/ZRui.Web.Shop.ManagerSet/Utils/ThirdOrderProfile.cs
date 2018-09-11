using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Data;
using ZRui.Web.Models.ThirdPartyModel;

namespace ZRui.Web.Utils
{
    public class ThirdOrderProfile : Profile
    {
        public ThirdOrderProfile()
        {
            CreateMap<ThirdOrder, ThirdOrdersModel>().ForMember(m => m.CancelFromStr, d => d.MapFrom(m => m.CancelFrom))
          .ForMember(m => m.OrderStatusStr, d => d.MapFrom(m => m.OrderStatus))
          .ForMember(m => m.ThirdTypeStr, d => d.MapFrom(m => m.ThirdType))
          .ForMember(m => m.ThirdUpdateTime, d => d.MapFrom(m => m.ConvertStringToDateTime()));
                        CreateMap<ThirdOrder, ThirdOrderModel>().ForMember(m => m.CancelFromStr, d => d.MapFrom(m => m.CancelFrom))
          .ForMember(m => m.OrderStatusStr, d => d.MapFrom(m => m.OrderStatus))
          .ForMember(m => m.ThirdTypeStr, d => d.MapFrom(m => m.ThirdType))
          .ForMember(m => m.ThirdUpdateTime, d => d.MapFrom(m => m.ConvertStringToDateTime()));
            CreateMap<ThirdMoneyReport, ThirdMoneyReportModel>().ForMember(m => m.ProduceTypeStr, d => d.MapFrom(m => m.ProduceType));
            CreateMap<ThirdShop, ThirdShopModel>().ForMember(m => m.statusstr, d => d.MapFrom(m => m.Status));
        }
    }
}
