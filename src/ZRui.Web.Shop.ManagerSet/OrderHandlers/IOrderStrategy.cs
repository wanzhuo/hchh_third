using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;

namespace ZRui.Web.OrderHandlers
{
    /// <summary>
    /// 订单处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrderStrategy
    {
        PagedList<GetPagedListResulrModel> GetPagedList(GetPagedListRequestModel input, ShopDbContext db);

       T GetOrderItems<T>(GetOrderItemsArgsModel input, ShopDbContext db);
    }
}
