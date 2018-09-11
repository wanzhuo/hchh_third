using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ZRui.Web.BLL.ShopDbContextExtension
{
    public static class ShopDbContextExtension
    {
        public static object LockForShopOrderSelfHelp = new object();

        /// <summary>
        /// 获取自助点餐取餐号
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static string GetCodeForShopOrderSelfHelp(this ShopDbContext db,int shopId)
        {
            int currentNumber;
            lock (LockForShopOrderSelfHelp)
            {
                var model = db.Query<CodeForShopOrderSelfHelp>()
                    .Where(m => m.ShopId == shopId)
                    .FirstOrDefault();
                if (model == null)
                {
                    var newModel = new CodeForShopOrderSelfHelp()
                    {
                        ShopId = shopId,
                        CurrentNumber = 1
                    };
                    db.Add(newModel);
                    currentNumber = 1;
                }
                else
                {
                    model.CurrentNumber++;
                    currentNumber = model.CurrentNumber;
                }
                db.SaveChanges();
                return "H" + currentNumber.ToString("000");
            }
        }

        public static ShopOrderMoneyOffRule GetMoneyOffRule(this ShopDbContext db, int shopId, int Amount, MoneyOffType moneyOffType)
        {
            var moneyOffCache = db.GetMoneyOffRuleCache(shopId, moneyOffType);
            if (moneyOffCache != null)
            {
                var rules = db.Query<ShopOrderMoneyOffRule>()
                .Where(m => m.MoneyOffId == moneyOffCache.MoneyOffId)
                .OrderByDescending(m => m.FullAmount)
                .ToList();
                int fullAmonut;
                foreach (var item in rules)
                {
                    fullAmonut = item.FullAmount;
                    if (Amount >= fullAmonut)
                    {
                        return item;
                    }
                }
            }
            return null;
        }


        public static ShopOrderMoneyOffRule GetMoneyOffRule(this ShopDbContext db, string shopFlag, int Amount, MoneyOffType moneyOffType)
        {
            var shop = db.Query<Shop>()
                .Where(m => m.Flag == shopFlag)
                .FirstOrDefault();
            return db.GetMoneyOffRule(shop.Id, Amount, moneyOffType);
        }


        public static ShopOrderMoneyOffCache GetMoneyOffRuleCache(this ShopDbContext db, int shopId, MoneyOffType moneyOffType)
        {
            var query = db.Query<ShopOrderMoneyOffCache>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Where(m => m.ShopOrderMoneyOff.IsEnable);
            switch (moneyOffType)
            {
                case MoneyOffType.堂食:
                    query = query.Where(m => m.ShopOrderMoneyOff.IsScanCode);
                    break;
                case MoneyOffType.外卖:
                    query = query.Where(m => m.ShopOrderMoneyOff.IsTakeout);
                    break;
                case MoneyOffType.自助:
                    query = query.Where(m => m.ShopOrderMoneyOff.IsSelfOrder);
                    break;
                default:
                    break;
            }
            return query.FirstOrDefault();
        }


    }
}
