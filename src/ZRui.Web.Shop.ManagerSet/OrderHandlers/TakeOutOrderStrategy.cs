using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;

namespace ZRui.Web.OrderHandlers
{
    /// <summary>
    /// 外卖订单处理
    /// </summary>
    public class TakeOutOrderStrategy : IOrderStrategy
    {
        private IMapper _mapper { get; set; }
        public TakeOutOrderStrategy(IMapper mapper)
        {
            this._mapper = mapper;
        }

        PagedList<GetPagedListResulrModel> IOrderStrategy.GetPagedList(GetPagedListRequestModel input, ShopDbContext db)
        {
            var query = db.Query<ShopOrder>()
               .Where(m => m.PayTime.HasValue && m.PayTime != null)
               .Where(m => m.IsTakeOut)
               .Where(m => m.ShopId == input.ShopId)
               .Where(m => m.OrderNumber.Contains(input.SearchId))
               .AsNoTracking()
               .OrderByDescending(m => m.AddTime)
               .ToPagedList(input.PageIndex, input.PageSize);

            var result = _mapper.Map<PagedList<GetPagedListResulrModel>>(query);

            foreach (var resultItem in result)
            {
                var takoutInfo = db.ShopOrderTakeouts.FirstOrDefault(m => m.ShopOrderId.Equals(resultItem.OrderId));
                if (takoutInfo!= null)
                {
                    resultItem.Address = takoutInfo.Address;
                    resultItem.TakeOutPhone = takoutInfo.Phone;
                    resultItem.TakeOutName = takoutInfo.Name;
                    resultItem.TakeOutStatus = takoutInfo.Status;
                    resultItem.TakeWay = takoutInfo.TakeWay;
                }
              

            }

            result.PageIndex = query.PageIndex;
            result.PageSize = query.PageSize;
            result.TotalItemCount = query.TotalItemCount;
            return result;
        }

        public T GetOrderItems<T>(GetOrderItemsArgsModel input, ShopDbContext db)
        {
            var query = db.Query<ShopOrderItem>()
              .Where(m => !m.IsDel);
            var list = query
                .Where(m => m.ShopOrderId == input.OrderId)
                .OrderByDescending(m => m.Id)
                .ToList();
            var result = _mapper.Map<T>(query);
            return result;
        }
    }
}
