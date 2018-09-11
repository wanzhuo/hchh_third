using System;

namespace ZRui.Web.BLL.ShopTakeOutInfoExtension
{
    public static class ShopTakeOutInfoExtension
    {
        /// <summary>
        /// 是否在配送范围内
        /// </summary>
        /// <param name="to"></param>
        /// <param name="shop"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static bool IsInScope(this ShopTakeOutInfo to,Shop shop,double lat,double lng)
        {

            //int distance = BaiduMapUtil.GetDistance(new Tuple<double, double>(shop.Latitude.Value, shop.Longitude.Value)
            //    , new Tuple<double, double>(lat, lng));

            //if (distance == -1) throw new Exception("获取距离失败");
            int distance = (int)Math.Floor(LatLngUtil.GetDistance(shop.Longitude.Value, shop.Latitude.Value, lng, lat));
            return distance <= to.Area;
        }

        /// <summary>
        /// 外卖是否可以用
        /// </summary>
        /// <param name="to"></param>
        /// <param name="shop"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static ShopTakeOutStatus CanUsed(this ShopTakeOutInfo to, Shop shop)
        {
            if (!to.IsUseTakeOut) throw new Exception("抱歉，商家未启动外卖功能");
            if (!to.IsOpen) return ShopTakeOutStatus.不在营业状态;
            //bool isInScope = to.IsInScope(shop, lat, lng);
            //if (!isInScope) return ShopTakeOutStatus.不在配送范围内;
            if (!to.StartTime.HasValue || !to.EndTime.HasValue) return ShopTakeOutStatus.未设置外卖时间;
            if (!isInOpenTime(to.StartTime.Value, to.EndTime.Value)) return ShopTakeOutStatus.不在外卖时段内;
            return ShopTakeOutStatus.可用;
        }

        static bool isInOpenTime(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            double nowSeconds = now.TimeOfDay.TotalSeconds;
            double startSeconds = start.TimeOfDay.TotalSeconds;
            double endSeconds = end.TimeOfDay.TotalSeconds;
            if (startSeconds <= endSeconds)
            {
                return nowSeconds >= startSeconds && nowSeconds <= endSeconds;
            }
            else
            {
                return nowSeconds <= startSeconds || nowSeconds >= endSeconds;
            }
        }
    }

    public enum ShopTakeOutStatus
    {
        可用 = 0,
        未设置外卖时间 = 1,
        不在外卖时段内 = 2,
        不在配送范围内 = 3,
        不在营业状态 = 4
    }
}
