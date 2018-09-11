using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZRui.Web.BLL.Servers
{

    /// <summary>
    /// 积分服务
    /// </summary>
    public class ShopMemberLevelServer
    {


        /// <summary>
        /// 更新腻会员等级
        /// </summary>
        /// <param name="db"></param>
        /// <param name="memberId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static async Task UpdateMemberLevel(ShopDbContext db, ShopMemberSet shopMemberSet, int memberId, int shopId, ILogger _logger)
        {
            int count = await ShopIntegralRechargeServer.GetMemberIntegral(db, memberId, shopId, _logger);
            var shopmember = await db.ShopMembers.FirstOrDefaultAsync(m => m.MemberId.Equals(memberId) && m.ShopId.Equals(shopId) && !m.IsDel);
            ShopMemberLevel shopMemberLevels = null;
            if (shopMemberSet.IsSavaLevel)
            {
                shopMemberLevels = await GetLevelByIntegral(db, count, shopId, _logger);
            }
            else
            {
                shopMemberLevels = await GetLevelUp(db, count, shopId, shopmember, _logger);

            }
            if (shopMemberLevels == null)
            {
                return;
            }
            shopmember.ShopMemberLevelId = shopMemberLevels.Id;
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// 更新腻所有会员等级
        /// </summary>
        /// <param name="db"></param>
        /// <param name="memberId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static async Task UpdateAllMemberLevel(ShopDbContext db, ShopMemberSet shopMemberSet, int shopId, ILogger _logger)
        {
            var shopMembers = db.ShopMembers.Where(m => !m.IsDel && m.ShopId.Equals(shopId));
            foreach (var member in shopMembers)
            {
                await UpdateMemberLevel(db, shopMemberSet, member.MemberId, shopId, _logger);
            }
        }

        /// <summary>
        /// 根据积分获取等级(开启降级使用)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private async static Task<ShopMemberLevel> GetLevelByIntegral(ShopDbContext db, int count, int shopId, ILogger _logger)
        {
            ShopMemberLevel shopMemberLevel = null;
            var shopMemberLevels = db.ShopMemberLevel.OrderBy(m=>m.Sort).Where(m => !m.IsDel && m.ShopId.Equals(shopId));
            if (shopMemberLevels.Count()==0)
            {
                return await Task.FromResult(new ShopMemberLevel() { Id = 0, LevelName = "", MemberLevel = "" });
            }
            var maxIntegralshopMemberLevels = await shopMemberLevels.MaxAsync(m => m.MaxIntegral);
            if (count> maxIntegralshopMemberLevels)
            {
                return await Task.FromResult( await shopMemberLevels.LastOrDefaultAsync());
            }

            var minIntegralshopMemberLevels = await shopMemberLevels.MinAsync(m => m.MinIntegral);
            if (count < minIntegralshopMemberLevels)
            {
                return await Task.FromResult(new ShopMemberLevel() { Id=0 , LevelName = "", MemberLevel=""});
            }
            foreach (var item in shopMemberLevels)
            {
                if (count >= item.MinIntegral && count <= item.MaxIntegral)
                {
                    shopMemberLevel = item;
                }
            }
            return await Task.FromResult(shopMemberLevel);
        }


        /// <summary>
        /// 升级(不开启降级使用)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private async static Task<ShopMemberLevel> GetLevelUp(ShopDbContext db, int count, int shopId, ShopMember shopMember, ILogger _logger)
        {
            ShopMemberLevel shopMemberLevel = null;
            var shopMemberLevels = db.ShopMemberLevel.OrderBy(m => m.Sort).Where(m => !m.IsDel && m.ShopId.Equals(shopId));
            if (shopMemberLevels.Count() == 0)
            {
                return await Task.FromResult(new ShopMemberLevel() { Id = 0, LevelName = "", MemberLevel = "" });
            }
            var maxIntegralshopMemberLevels = await shopMemberLevels.MaxAsync(m => m.MaxIntegral);
            if (count > maxIntegralshopMemberLevels)
            {
                return await Task.FromResult(await shopMemberLevels.LastOrDefaultAsync());
            }
            foreach (var item in shopMemberLevels)
            {
                if (count >= item.MinIntegral && count <= item.MaxIntegral)
                {
                    shopMemberLevel = item;
                }
            }
            var oldShopMemberLevel = await shopMemberLevels.FirstOrDefaultAsync(m => !m.IsDel && m.Id.Equals(shopMember.ShopMemberLevelId));
            if (oldShopMemberLevel != null)
            {
                if (oldShopMemberLevel.Sort >= shopMemberLevel.Sort)
                {
                    return await Task.FromResult(oldShopMemberLevel);
                }
            }
            return await Task.FromResult(shopMemberLevel);
        }




        //消费积分


    }
}
