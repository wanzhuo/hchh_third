using System;
using System.Linq;
using ZRui.Web.BLL.ShopTakeOutInfoExtension;

namespace ZRui.Web.BLL.MemberAddressExtension
{
    public static class MemberAddressExtension
    {
        /// <summary>
        /// 完整地址
        /// </summary>
        /// <param name="memberAddress"></param>
        /// <returns></returns>
        public static string GetAddress(this MemberAddress memberAddress)
        {
            return memberAddress.Province + memberAddress.City + memberAddress.Area
                + memberAddress.Detail;
        }

        /// <summary>
        /// 获取用户地址所对应的坐标
        /// </summary>
        /// <param name="memberAddress"></param>
        /// <returns></returns>
        public static bool GetCoordinates(this MemberAddress memberAddress)
        {
            var coordinates = BaiduMapUtil.GetGeoCoder(memberAddress.GetAddress());
            if (!coordinates.HasValue) return false;
            memberAddress.Latitude = coordinates.Value.lat;
            memberAddress.Longitude = coordinates.Value.lng;
            return true;
        }


        /// <summary>
        /// 设置当前地址为使用中的地址
        /// </summary>
        /// <param name="memberAddress"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool SetAddressIsUsed(this MemberAddress memberAddress,ShopDbContext db, int shopId,int memberId , bool IsConglomeration)
        {
            if (IsConglomeration)
            {
                memberAddress.CheckIsInScope(db, shopId);
            }

            var list = db.Query<MemberAddress>()
                .Where(m => m.MemberId == memberId)
                .Where(m => !m.IsDel)
                .ToList();

            foreach (var item in list)
            {
                item.IsUsed = false;
            }
            memberAddress.IsUsed = true;
            db.SaveChanges();

            return true;
        }


        /// <summary>
        /// 检查是否在配送范围
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="shopId"></param>
        public static void CheckIsInScope(this MemberAddress model, ShopDbContext db ,int shopId)
        {
            var shop = db.GetSingle<Shop>(shopId);
            model.CheckIsInScope(db, shop);
        }

        /// <summary>
        /// 检查是否在配送范围
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="shop"></param>
        public static void CheckIsInScope(this MemberAddress model, ShopDbContext db, Shop shop)
        {
            ShopTakeOutInfo toInfo = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shop.Id)
                .FirstOrDefault();
            if (toInfo == null) throw new Exception("外卖基础设置不存在");
            model.CheckIsInScope(shop, toInfo);
        }

        /// <summary>
        /// 检查是否在配送范围
        /// </summary>
        /// <param name="model"></param>
        /// <param name="shop"></param>
        /// <param name="shopTakeOutInfo"></param>
        public static void CheckIsInScope(this MemberAddress model, Shop shop, ShopTakeOutInfo toInfo)
        {
            if (!toInfo.IsInScope(shop, model.Latitude.Value, model.Longitude.Value))
                throw new Exception("当前位置不在商家配送范围内");
        }


    }
}
