using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public class ShopIntegralRechargeServer
    {
        ShopDbContext db;
        ILogger _logger;
        public ShopIntegralRechargeServer(
             IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , MemberDbContext memberDb
            )
        {
            this.db = db;
            _logger = loggerFactory.CreateLogger<ShopIntegralRechargeServer>();
        }
        /// <summary>
        /// 开始计算积分（通过支付记录）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="memberTradeForRechange"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        public static async Task<bool> GetIntegralStartAsync(ShopDbContext db, MemberTradeForRechange memberTradeForRechange, ILogger _logger)
        {
            try
            {
                var sourceType = SourceType.拼团订单;

                if (memberTradeForRechange.OrderType != OrderType.拼团订单)
                {
                    var order = await db.ShopOrders.FindAsync(memberTradeForRechange.OrderId);
                    if (order.IsTakeOut)
                    {
                        sourceType = SourceType.外卖订单;
                    }
                    if (order.ShopPartId.HasValue)
                    {
                        sourceType = SourceType.扫码点餐订单;
                    }
                    if (order.ShopOrderSelfHelpId.HasValue)
                    {
                        sourceType = SourceType.自助点餐订单;
                    }
                }
                return await GetOrderIntegral(db, memberTradeForRechange.OrderId, sourceType, _logger);
            }
            catch (Exception e)
            {
                _logger.LogError($"开始计算积分GetIntegralStartAsync 错误信息：{e}");
                return await Task.FromResult(true);
            }
        }

        /// <summary>
        /// 获取积分来源Type
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderId"></param>
        /// <param name="isConglomeration"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        public static async Task<SourceType> GetOrderSourceType(ShopDbContext db, int orderId, bool isConglomeration, ILogger _logger)
        {
            SourceType sourceType = SourceType.拼团订单;
            if (!isConglomeration)
            {
                var order = await db.ShopOrders.FindAsync(orderId);
                if (order.IsTakeOut)
                {
                    sourceType = SourceType.外卖订单;
                }
                if (order.ShopPartId.HasValue)
                {
                    sourceType = SourceType.扫码点餐订单;
                }
                if (order.ShopOrderSelfHelpId.HasValue)
                {
                    sourceType = SourceType.自助点餐订单;
                }
            }
            return sourceType;
        }


        /// <summary>
        /// 获取消费积分（根据订单Id）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="conglomerationOrder"></param>
        /// <returns></returns>
        public async static Task<bool> GetOrderIntegral(ShopDbContext db, int orderId, SourceType sourceType, ILogger _logger)
        {
            _logger.LogInformation($"-----获取消费积分积分GetOrderIntegral 开始---");
            try
            {

                var shopIntegralRechargeParameter = await GetShopIntegralRechargeParameterAsync(db, sourceType, orderId, _logger);
                if (shopIntegralRechargeParameter.ShopId == 0)
                {
                    return false;
                }
                var shopMembers = await db.ShopMembers.FirstOrDefaultAsync(m => !m.IsDel && m.ShopId.Equals(shopIntegralRechargeParameter.ShopId) && m.MemberId.Equals(shopIntegralRechargeParameter.MemberId));
                if (shopMembers == null)
                {
                    return false;
                }
                var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => m.ShopId.Equals(shopIntegralRechargeParameter.ShopId));
                if (shopMemberSet == null)
                {
                    return false;
                }
                if (!shopMemberSet.IsConsumeIntegral) //消费积分判断
                {
                    return false;
                }

                var rate = await GetShopMemberSet(db, shopMemberSet, shopIntegralRechargeParameter.ShopId, _logger);
                _logger.LogInformation($"获取积分比例rate：{rate}");

                var shopIntegralRecharge = await db.ShopIntegralRecharge.FirstOrDefaultAsync(m => !m.SourceType.Equals(sourceType) && m.SourceOrderId.Equals(orderId) && m.CodeStatut.Equals(1));
                if (shopIntegralRecharge != null)
                {
                    throw new Exception("记录已存在");
                }
                var count = (int)Math.Floor((shopIntegralRechargeParameter.Count * rate));
                _logger.LogInformation($"添加的积分shopIntegralRechargeParameter.Count：{shopIntegralRechargeParameter.Count}  count：{count}");
                if (count == 0)
                {
                    return false;
                }
                ShopIntegralRecharge addshopIntegralRecharge = new ShopIntegralRecharge();
                addshopIntegralRecharge.AddIp = "";
                addshopIntegralRecharge.AddTime = DateTime.Now;
                addshopIntegralRecharge.CodeStatut = 1;
                addshopIntegralRecharge.Count = count;
                addshopIntegralRecharge.ShopMemberId = shopMembers.Id;
                addshopIntegralRecharge.MemberId = shopIntegralRechargeParameter.MemberId;
                addshopIntegralRecharge.ShopId = shopIntegralRechargeParameter.ShopId;
                addshopIntegralRecharge.SourceOrderId = orderId;
                addshopIntegralRecharge.SourceRemark = $"{sourceType.ToString()}订单积分";
                addshopIntegralRecharge.SourceType = sourceType;
                await db.ShopIntegralRecharge.AddAsync(addshopIntegralRecharge);
                var isOk = await db.SaveChangesAsync();
                await CalculateMemberIntegral(db, shopIntegralRechargeParameter.MemberId, shopIntegralRechargeParameter.ShopId, _logger);
                await ShopMemberLevelServer.UpdateMemberLevel(db, shopMemberSet, shopIntegralRechargeParameter.MemberId, shopIntegralRechargeParameter.ShopId, _logger);
                _logger.LogInformation($"-----获取消费积分积分GetOrderIntegral 结束---");


                return isOk > 0;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        /// <summary>
        /// 订单申请退款，积分退回，积分余额不足则清零
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderId"></param>
        /// <param name="sourceType"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        public async static Task<bool> IntegralReturn(ShopDbContext db, int orderId, SourceType sourceType, ILogger _logger)
        {
            try
            {
                int returnCount = 0;
                var shopIntegralRechargeParameter = await GetShopIntegralRechargeParameterAsync(db, sourceType, orderId, _logger);
                if (shopIntegralRechargeParameter.ShopId == 0)
                {
                    return false;
                }
                var shopMembers = await db.ShopMembers.FirstOrDefaultAsync(m => !m.IsDel && m.ShopId.Equals(shopIntegralRechargeParameter.ShopId) && m.MemberId.Equals(shopIntegralRechargeParameter.MemberId));
                if (shopMembers == null)
                {
                    return false;
                }
                var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => m.ShopId.Equals(shopIntegralRechargeParameter.ShopId));
                if (shopMemberSet == null)
                {
                    return false;
                }
                var shopIntegralRecharge = await db.ShopIntegralRecharge.FirstOrDefaultAsync(m => !m.IsDel && m.CodeStatut.Equals(1) && m.SourceOrderId.Equals(orderId) && m.SourceType.Equals(sourceType));

                if (shopIntegralRecharge == null) //没有获取过积分
                {
                    return true;
                }
                var membershopIntegral = await GetMemberIntegral(db, shopIntegralRechargeParameter.MemberId, shopIntegralRechargeParameter.ShopId, _logger);
                if (membershopIntegral < shopIntegralRecharge.Count)
                {
                    returnCount = membershopIntegral;
                }
                else
                {
                    returnCount = shopIntegralRecharge.Count;
                }
                ShopIntegralRecharge addshopIntegralRecharge = new ShopIntegralRecharge();
                addshopIntegralRecharge.AddIp = "";
                addshopIntegralRecharge.AddTime = DateTime.Now;
                addshopIntegralRecharge.CodeStatut = -1;
                addshopIntegralRecharge.Count = returnCount;
                addshopIntegralRecharge.ShopMemberId = shopIntegralRecharge.ShopMemberId;
                addshopIntegralRecharge.MemberId = shopIntegralRecharge.MemberId;
                addshopIntegralRecharge.ShopId = shopIntegralRecharge.ShopId;
                addshopIntegralRecharge.SourceOrderId = orderId;
                addshopIntegralRecharge.SourceRemark = $"{sourceType.ToString()}订单积分退还";
                addshopIntegralRecharge.SourceType = sourceType;
                await db.ShopIntegralRecharge.AddAsync(addshopIntegralRecharge);
                var isOk = await db.SaveChangesAsync();
                await CalculateMemberIntegral(db, shopIntegralRechargeParameter.MemberId, shopIntegralRechargeParameter.ShopId, _logger);
                await ShopMemberLevelServer.UpdateMemberLevel(db, shopMemberSet, shopIntegralRechargeParameter.MemberId, shopIntegralRechargeParameter.ShopId, _logger);
                return isOk > 0;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"退还积分错误，错误信息：{e}");

                return false;
            }
        }


        /// <summary>
        /// 更新会员积分
        /// </summary>
        /// <param name="db"></param>
        /// <param name="memberId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static async Task CalculateMemberIntegral(ShopDbContext db, int memberId, int shopId, ILogger _logger)
        {
            _logger.LogInformation($"----------更新会员积分CalculateMemberIntegral 开始----------");

            int count = await GetMemberIntegral(db, memberId, shopId, _logger);
            var shopMember = await db.ShopMembers.FirstOrDefaultAsync(m => m.MemberId.Equals(memberId) && !m.IsDel);
            if (shopMember != null)
            {
                shopMember.Credits = count;
            }
            await db.SaveChangesAsync();
            _logger.LogInformation($"----------更新会员积分CalculateMemberIntegral 结束----------");



        }

        /// <summary>
        /// 获取会员积分
        /// </summary>
        /// <param name="db"></param>
        /// <param name="memberId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static async Task<int> GetMemberIntegral(ShopDbContext db, int memberId, int shopId, ILogger _logger)
        {
            var shopMembers = await db.ShopMembers.FirstOrDefaultAsync(m => !m.IsDel && m.ShopId.Equals(shopId) && m.MemberId.Equals(memberId));
            if (shopMembers == null)
            {
                return 0;
            }
            return await Task.FromResult(db.ShopIntegralRecharge.Where(m => m.ShopId.Equals(shopId) && m.MemberId.Equals(memberId) && m.ShopMemberId.Equals(shopMembers.Id) && !m.IsDel).Sum(m => m.Count * m.CodeStatut));
        }



        /// <summary>
        /// 获取积分比率
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        private async static Task<decimal> GetShopMemberSet(ShopDbContext db, ShopMemberSet shopMemberSet, int shopId, ILogger _logger)
        {

            if (shopMemberSet.ConsumeAmount == 0 || shopMemberSet.GetIntegral == 0)
            {
                return 0M;
            }
            var rate = shopMemberSet.GetIntegral / (shopMemberSet.ConsumeAmount / 100.00M);
            return await Task.FromResult(rate);
        }

        /// <summary>
        /// 获取指定类型订单积分参数
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<ShopIntegralRechargeParameter> GetShopIntegralRechargeParameterAsync(ShopDbContext db, SourceType sourceType, int orderId, ILogger _logger)
        {
            _logger.LogInformation($"---获取指定类型订单积分参数GetShopIntegralRechargeParameterAsync 开始---");

            ShopIntegralRechargeParameter shopIntegralRechargeParameter = new ShopIntegralRechargeParameter();
            if (sourceType == SourceType.拼团订单)
            {
                var conglomerationOrder = await db.ConglomerationOrder.FindAsync(orderId);
                if (conglomerationOrder == null)
                {
                    throw new Exception("订单不存在");
                }
                _logger.LogInformation($"订单状态conglomerationOrder.Status：{conglomerationOrder.Status}");

                shopIntegralRechargeParameter.Count = conglomerationOrder.Payment.Value / 100.00M;
                shopIntegralRechargeParameter.MemberId = conglomerationOrder.MemberId;
                shopIntegralRechargeParameter.ShopId = conglomerationOrder.ShopId;


            }
            else
            {
                var shopOrders = await db.ShopOrders.FindAsync(orderId);
                if (shopOrders == null)
                {
                    throw new Exception("订单不存在");
                }

                shopIntegralRechargeParameter.Count = shopOrders.Payment.Value / 100.00M;
                shopIntegralRechargeParameter.MemberId = shopOrders.MemberId;
                shopIntegralRechargeParameter.ShopId = shopOrders.ShopId;

            }
            _logger.LogInformation($"---获取指定类型订单积分参数GetShopIntegralRechargeParameterAsync 结束---");

            return shopIntegralRechargeParameter;
        }

        ///// <summary>
        ///// 退回积分
        ///// </summary>
        ///// <returns></returns>
        //public static async Task<bool> ExpenditureIntgral(ShopDbContext db, int orderId, SourceType sourceType, ILogger _logger)
        //{

        //    var shopIntegralRecharge = await db.ShopIntegralRecharge.FirstOrDefaultAsync(m => !m.SourceType.Equals(sourceType) && m.SourceOrderId.Equals(orderId) && m.CodeStatut.Equals(1));
        //    if (shopIntegralRecharge == null)
        //    {
        //        return false;
        //    }
        //    var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => m.ShopId.Equals(shopIntegralRecharge.ShopId));
        //    if (shopMemberSet == null)
        //    {
        //        return false;
        //    }
        //    ShopIntegralRecharge addshopIntegralRecharge = new ShopIntegralRecharge();
        //    addshopIntegralRecharge.AddIp = "";
        //    addshopIntegralRecharge.AddTime = DateTime.Now;
        //    addshopIntegralRecharge.CodeStatut = -1;
        //    addshopIntegralRecharge.Count = shopIntegralRecharge.Count;
        //    addshopIntegralRecharge.MemberId = shopIntegralRecharge.MemberId;
        //    addshopIntegralRecharge.ShopId = shopIntegralRecharge.ShopId;
        //    addshopIntegralRecharge.SourceOrderId = shopIntegralRecharge.SourceOrderId;
        //    addshopIntegralRecharge.SourceRemark = $"{sourceType.ToString()}订单积分退回";
        //    addshopIntegralRecharge.SourceType = shopIntegralRecharge.SourceType;
        //    await db.ShopIntegralRecharge.AddAsync(addshopIntegralRecharge);
        //    var isOk = await db.SaveChangesAsync();
        //    await CalculateMemberIntegral(db, shopIntegralRecharge.MemberId, shopIntegralRecharge.ShopId, _logger);
        //    await ShopMemberLevelServer.UpdateMemberLevel(db, shopMemberSet, shopIntegralRecharge.MemberId, shopIntegralRecharge.ShopId, _logger);
        //    return await Task.FromResult(true);
        //}

    }


    public class ShopIntegralRechargeParameter
    {
        public decimal Count { get; set; }
        public int MemberId { get; set; }
        public int ShopId { get; set; }
    }
}
