using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.ShopAPIModels;

namespace ZRui.Web.Utils
{
    public static class ExBannerModels
    {
        public static List<BannerModel> GetBannerList(ShopDbContext _db, string Flag)
        {
            var shopbanners = _db.Query<Shop>().FirstOrDefault(r => r.Flag == Flag).Banners;
            if (shopbanners == null)
            {
                var bannerdefault = new List<BannerModel>();
                bannerdefault.Add(new BannerModel() { Id = 1, IsShow = true, Name = "bannerdefault", Link = "", Sorting = 1, Url = "" });
                return bannerdefault;
            }

            var list = JsonConvert.DeserializeObject<List<BannerModel>>(shopbanners)
            .Where(r => r.IsShow == true)
            .OrderByDescending(r => r.Sorting).ToList();
            return list;
        }
    }
}
