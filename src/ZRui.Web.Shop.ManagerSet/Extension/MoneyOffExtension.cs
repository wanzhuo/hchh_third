using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZRui.Web.ShopManager.MoneyOffExtension
{
    public static class MoneyOffExtension
    {
        public static void UpdateMoneyOffCache()
        {
            //TODO:未能获取依赖注入容器中的对象，此处为缓兵之计
            var _contextOptions = new DbContextOptionsBuilder<ShopDbContext>()
                .UseMySql("Server=120.79.31.209;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;")
                .Options;
            using (ShopDbContext db = new ShopDbContext(_contextOptions))
            {
                DateTime now = DateTime.Now.Date;
                var moneyOffList = db.Query<ShopOrderMoneyOff>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.EndDate >= now)
                    .Where(m => m.StartDate <= now)
                    .ToList();

                var moneyOffCacheList = db.Set<ShopOrderMoneyOffCache>()
                    .Where(m => !m.IsDel);

                var moneyOffFromCache = moneyOffCacheList.Select(m => m.ShopOrderMoneyOff).ToList();

                //新增部分
                List<ShopOrderMoneyOff> inCreased = moneyOffList.Except(moneyOffFromCache).ToList();
                foreach (var item in inCreased)
                {
                    ShopOrderMoneyOffCache model = new ShopOrderMoneyOffCache()
                    {
                        ShopId = item.ShopId,
                        MoneyOffId = item.Id,
                        IsDel = false
                    };
                    db.AddTo(model);
                }

                //减少部分
                var reduceIDs = moneyOffFromCache.Except(moneyOffList).Select(m => m.Id);
                var reduceCacheList = db.Query<ShopOrderMoneyOffCache>()
                    .Where(m => reduceIDs.Contains(m.MoneyOffId)).ToList();
                foreach (var item in reduceCacheList)
                {
                    item.IsDel = true;
                }
                db.SaveChanges();
            }
        }
    }
}
