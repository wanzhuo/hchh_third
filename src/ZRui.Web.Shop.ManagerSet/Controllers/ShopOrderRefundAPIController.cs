using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZRui.Web.BLL.Servers;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Pay;
using ZRui.Web.ShopOrderRefundAPIModel;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 订单退款api
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopOrderRefundAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        PayProxyFactory proxyFactory;
        FinanceDbContext financeDb;
        ILogger _logger;
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="wechatOpenOptions"></param>
        /// <param name="db"></param>
        /// <param name="memberDb"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopOrderRefundAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , ShopDbContext db
            , FinanceDbContext financeDb
            , ILoggerFactory loggerFactory
            , PayProxyFactory proxyFactory
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            this.proxyFactory = proxyFactory;
            this.financeDb = financeDb;
            _logger = loggerFactory.CreateLogger<ShopOrderRefundAPIController>();
        }

        /// <summary>
        /// 退款返回余额
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ToAmount([FromBody]ToAmountArgsModel args)
        {
            ShopOrder shopOrder = db.Set<ShopOrder>().Find(args.id);
            if (shopOrder == null) throw new Exception("订单不存在");
            CheckShopActor(shopOrder.ShopId, ShopActorType.超级管理员);
            shopOrder.Status = ShopOrderStatus.已退款;
            DecreaseCommodity(shopOrder);
            RefundToAmonut(shopOrder);
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        //[Authorize]
        public async Task<APIResult> ManagerRefund([FromBody]ToAmountArgsModel args)
        {
            string exmsg = string.Empty;
            try
            {
                var shoporder = db.Set<ShopOrder>().FirstOrDefault(r => r.Id == args.id);
                //if (shoporder == null) throw new Exception("未找到此订单");
                //CheckShopActor(shoporder.ShopId, ShopActorType.超级管理员);//检测用户权限
                var shopMemberConsume = db.GetSingle<ShopMemberConsume>(shoporder.ShopMemberConsumeId ?? 0);
                if (shopMemberConsume != null)
                {
                    var shopMemberRufund = db.Query<ShopMemberRufund>()
                    .Where(m => !m.IsDel && m.ShopOrderId == shoporder.Id)
                    .FirstOrDefault();
                    if (shopMemberRufund != null)
                        throw new Exception("该订单已退款");
                    ShopMemberServer shopMemberServer = new ShopMemberServer(db, shoporder.ShopId, shoporder.MemberId);
                    shopMemberServer.RefundToBalance(shoporder);
                    shoporder.Status = ShopOrderStatus.已退款;
                    db.SaveChanges();
                    var orderType = await ShopIntegralRechargeServer.GetOrderSourceType(db, args.id, false, _logger); //积分回滚
                    await ShopIntegralRechargeServer.IntegralReturn(db, args.id, orderType, _logger);
                    return Success("退款成功");
                }
                else
                {
                    var shoppayinfo = db.Set<ShopPayInfo>().FirstOrDefault(r => r.ShopId == shoporder.ShopId && r.IsEnable == true);
                    if (shoppayinfo == null) throw new Exception("未找到商户退款配置");
                    var membertradeforrechange = financeDb.Set<MemberTradeForRechange>().Where(r => r.OrderId == shoporder.Id && r.OrderType == OrderType.普通订单 && r.Status == MemberTradeForRechangeStatus.成功);

                    if (membertradeforrechange != null && membertradeforrechange.Count() > 0) //代表有支付成功的数据
                    {
                        Refunds refunds = new Refunds(proxyFactory);
                        var refundresult = refunds.RefundAction(new RefundArgsModel() { ShopFlag = shoppayinfo.ShopFlag, TradeNo = membertradeforrechange.First().TradeNo, OrderType = OrderType.普通订单, OrderId = shoporder.Id });
                        if (refundresult.Status == MemberTradeForRefundStatus.成功)
                        {
                            shoporder.Status = ShopOrderStatus.已退款;
                            DecreaseCommodity(shoporder);
                            db.SaveChanges();
                            return Success();
                        }
                        else if (refundresult.Status == MemberTradeForRefundStatus.退款中)
                        {
                            shoporder.Status = ShopOrderStatus.退款中;
                            db.SaveChanges();

                        }
                        else
                        {
                            shoporder.Status = ShopOrderStatus.退款中;
                            db.SaveChanges();
                        }
                        var orderType = await ShopIntegralRechargeServer.GetOrderSourceType(db, args.id, false, _logger); //积分回滚
                        await ShopIntegralRechargeServer.IntegralReturn(db, args.id, orderType, _logger);
                        return Success("您的退款申请已经提交，银行处理退款中");

                    }
                    else
                    {
                        throw new Exception($"找不到订单{shoporder.OrderNumber}支付成功的记录");
                    }
                }
            }
            catch (Exception ex)
            {
                exmsg = ex.Message;
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 返回支付渠道
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public APIResult ToPayChannel([FromBody]ToAmountArgsModel args)
        //{
        //    ShopOrder shopOrder = db.Set<ShopOrder>().Find(args.id);
        //    if (shopOrder == null) throw new Exception("订单不存在");
        //    CheckShopActor(shopOrder.ShopId, ShopActorType.超级管理员);

        //    return Success();
        //}

        /// <summary>
        /// 减少统计过的销量
        /// </summary>
        /// <param name="model"></param>
        void DecreaseCommodity(ShopOrder model)
        {
            //获取订单中商品的数量
            var commodityIdAndCounts = db.Query<ShopOrderItem>()
                .Where(m => m.ShopOrderId == model.Id)
                .Where(m => !m.IsDel)
                .Select(m => new
                {
                    m.CommodityStock.Sku.CommodityId,
                    m.Count
                })
                .ToList()
                .GroupBy(m => m.CommodityId)
                .ToDictionary(m => m.Key, m => m.Select(x => x.Count).Sum());

            //更新商品的销售量
            //注意，这里如果有一个品牌，多个店铺的情况，会出现销售额共享的情况
            var commodityIds = commodityIdAndCounts.Select(m => m.Key).ToList();
            var commoditys = db.Query<ShopBrandCommodity>()
                    .Where(m => commodityIds.Contains(m.Id))
                    .ToList();
            foreach (var item in commoditys)
            {
                item.SalesForMonth -= commodityIdAndCounts[item.Id];
            }
        }

        void RefundToAmonut(ShopOrder shopOrder)
        {
            var amounts = db.GetMemberAmountList(shopOrder.MemberId);

            var totalFee = shopOrder.Amount;
            var amount = amounts.GetSingle(MemberAmountType.可用现金金额);
            long OriginalAmount = 0;
            if (amount != null)
            {
                OriginalAmount = amount.Amount;
                amount.Amount += totalFee;
            }
            else
            {
                amount = new MemberAmount();
                amount.Amount = totalFee;
                amount.AmountType = MemberAmountType.可用现金金额;
                amount.MemberId = shopOrder.MemberId;
                db.AddToMemberAmount(amount);
            }

            amounts.Increase(MemberAmountType.累计充值金额, totalFee);

            var amountlog = new MemberAmountChangeLog();
            amountlog.AddTime = DateTime.Now;
            amountlog.Amount = totalFee;
            amountlog.MemberId = shopOrder.MemberId;
            amountlog.Detail = "取消订单-可用现余额退款";
            amountlog.NowAmount = OriginalAmount + totalFee;
            amountlog.OriginalAmount = OriginalAmount;
            amountlog.Title = "退款";
            amountlog.Type = FinaceType.退款;
            db.AddToMemberAmountChangeLog(amountlog);

            #region 修改金额缓存
            amounts.UpdateMemberAmountCache();
            #endregion

        }

    }
}
