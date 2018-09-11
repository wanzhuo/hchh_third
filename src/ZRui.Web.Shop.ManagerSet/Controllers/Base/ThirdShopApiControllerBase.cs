using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web.Controllers.Base
{
    public class ThirdShopApiControllerBase : ApiControllerBase
    {
        ShopDbContext shopDb;
        public ThirdShopApiControllerBase(ShopDbContext shopDb)
        {
            this.shopDb = shopDb;
        }

        protected Merchant GetMerchant(int shopid)
        {
            var merchant = shopDb.Merchant.FirstOrDefault(r => r.ShopId == shopid);
            if (merchant == null)
            {
                throw new Exception("请先开户");
            }
            return merchant;
        }
    }
}
