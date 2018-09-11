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
    /// 拼团订单处理
    /// </summary>
    public class ConglomerationOrderStrategy : IOrderStrategy
    {
        private IMapper _mapper { get; set; }
        public ConglomerationOrderStrategy(IMapper mapper)
        {
            this._mapper = mapper;
        }
        PagedList<GetPagedListResulrModel> IOrderStrategy.GetPagedList(GetPagedListRequestModel input, ShopDbContext db)
        {
            var query = db.Query<ConglomerationOrder>()
                .Where(m => m.PayTime.HasValue && m.PayTime != null)
                .Where(m => m.ShopId == input.ShopId)
                .Where(m => m.OrderNumber.Contains(input.SearchId) || m.PickupCode.Contains(input.SearchId))
                .Include(m => m.ConglomerationSetUp)

                .AsNoTracking()
                .OrderByDescending(m => m.CreateTime)
                .ToPagedList(input.PageIndex, input.PageSize);
            foreach (var item in query)
            {
                if (item.Type == ConsignmentType.快递)
                {
                    item.ConglomerationExpress = db.ConglomerationExpress.Find(item.ConglomerationExpressId);
                }
            }
            var result = _mapper.Map<PagedList<GetPagedListResulrModel>>(query);
            result.PageIndex = query.PageIndex;
            result.PageSize = query.PageSize;
            result.TotalItemCount = query.TotalItemCount;
            return result;
        }

        public T GetOrderItems<T>(GetOrderItemsArgsModel input, ShopDbContext db)
        {
            var query = db.ConglomerationOrder.Find(input.OrderId);
            query.ConglomerationActivity = db.ConglomerationActivity.Find(query.ConglomerationActivityId);
            query.ConglomerationSetUp = db.ConglomerationSetUp.Find(query.ConglomerationSetUpId);
            if (query.Type == ConsignmentType.快递)
            {
                query.ConglomerationExpress = db.ConglomerationExpress.Find(query.ConglomerationExpressId);
            }

            var result = _mapper.Map<T>(query);
            return result;

        }
    }
}


