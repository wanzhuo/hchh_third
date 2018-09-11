using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ZRui.Web.Common;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;

namespace ZRui.Web.OrderHandlers
{
    public class SelfHelpStrategy : IOrderStrategy
    {

        private IMapper _mapper { get; set; }
        public SelfHelpStrategy(IMapper mapper)
        {
            this._mapper = mapper;
        }

        PagedList<GetPagedListResulrModel> IOrderStrategy.GetPagedList(GetPagedListRequestModel input, ShopDbContext db)
        {
            var query = db.Query<ShopOrder>()
               .Where(m => m.PayTime.HasValue)
               .Where(m => m.ShopOrderSelfHelpId.HasValue)
                .Where(m => m.ShopId == input.ShopId)
               .Where(m => m.OrderNumber.Contains(input.SearchId))
                .OrderByDescending(m => m.AddTime)
               .AsNoTracking()
               .ToPagedList(input.PageIndex, input.PageSize);

            var result = _mapper.Map<PagedList<GetPagedListResulrModel>>(query);
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
