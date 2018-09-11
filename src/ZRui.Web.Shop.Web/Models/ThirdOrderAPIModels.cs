using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.BLL.Servers;

namespace ZRui.Web.Models
{
   public class ThirdOrderAPIModels
    {
        public int ShopId { get; set; }
        public string OrderId { get; set; }
        public ThirdShopAddOrderModel thirdShopAddOrderModel { get; set; }

    }
}
