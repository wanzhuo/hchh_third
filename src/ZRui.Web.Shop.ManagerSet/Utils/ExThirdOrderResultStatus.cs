using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Data;

namespace ZRui.Web.Utils
{
    public class ExThirdOrderResultStatus
    {
        public static ShopOrderStatus ExStatus(OrderStatus status)
        {
            if (status == OrderStatus.待取货)
            {
                return ShopOrderStatus.待取货;
            }
            if (status == OrderStatus.配送中)
            {
                return ShopOrderStatus.配送中;
            }
            if (status == OrderStatus.已完成)
            {
                return ShopOrderStatus.已完成;
            }
            if (status == OrderStatus.已取消)
            {
                return ShopOrderStatus.已取消;
            }

            return (ShopOrderStatus)status;

        }
    }
}
