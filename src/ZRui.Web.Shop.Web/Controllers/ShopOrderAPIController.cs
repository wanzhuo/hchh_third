using System;
using System.Linq;
using ZRui.Web.ShopOrderAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using ZRui.Web.BLL;
using ZRui.Web.Common;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Models;
using ZRui.Web.Utils;
using ZRui.Web.BLL.Servers;
using System.Threading.Tasks;
using ZRui.Web.BLL.Third;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopOrderAPIController : WechatApiControllerBase
    {
        static object lockAddObject = new object();
        ShopDbContext db;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        // ThirdServer thirdServer;
        ThirdConfig thirdConfig;
        public ShopOrderAPIController(
            IOptions<MemberAPIOptions> options
            , IOptions<WechatTemplateSendOptions> tOption
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment
            , IOptions<ThirdConfig> pOption

            // , ThirdServer thirdServer
            )
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            wechatTemplateSendOptions = tOption.Value;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
            //  this.thirdServer = thirdServer;
            this.thirdConfig = pOption.Value;
        }

        /// <summary>
        /// 获取我的订单列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetListForMe([FromBody]GetListForMeArgsModel args)
        {
            var memberId = GetMemberId();
            var query = db.Query<ShopOrder>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId);

            if (args.orderType.Equals(OrderTypeF.外卖订单))
            {
                query = query.Where(m => m.IsTakeOut);
            }
            else
            {
                query = query.Where(m => !m.IsTakeOut);

            }

            if (!string.IsNullOrEmpty(args.ShopFlag))
            {
                query = query.Where(m => m.Shop.Flag == args.ShopFlag);
            }

            var items = query
                .OrderByDescending(m => m.Id)
                 .Select(m => new RowItem
                 {
                     AddTime = m.AddTime,
                     Id = m.Id,
                     OrderNumber = m.OrderNumber,
                     Phone = m.Phone,
                     Remark = m.Remark,
                     ShopName = m.Shop.Name,
                     AddUser = m.AddUser,
                     FinishTime = m.FinishTime,
                     Flag = m.Flag,
                     PayTime = m.PayTime,
                     Payment = m.Payment,
                     Amount = m.Amount,
                     ShopPartId = m.ShopPartId,
                     IsTakeOut = m.IsTakeOut,
                     ShopPartTitle = m.ShopPart.Title,
                     Cover = m.Shop.Cover,
                     OrderStatus = ExOrderStatus.ExOrderStatusAction(m.Status),
                     Status = m.Status,
                     takeDistributionType = m.takeDistributionType
                 })
                 .ToPagedList(args.PageIndex, args.PageSize);
            //获取订单状态
            //items.ForEach(m =>
            //{
            //    if (m.IsTakeOut)
            //        m.OrderStatus = db.Query<ShopOrderTakeout>()
            //        .Where(x => !x.IsDel && x.ShopOrderId == m.Id)
            //        .FirstOrDefault()?.Status.ToString();
            //    else
            //        m.OrderStatus = m.Status.ToString();
            //});

            return Success(new GetListForMeModel()
            {
                Items = items
            });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            //TODO:这里还需要进行锁操作，保证每个店铺的库存不是负数
            if (args.Items == null || args.Items.Count <= 0) throw new Exception("商品内容不能为空");
            bool isTakeOut = args.IsTakeOut == null ? false : args.IsTakeOut.Value;
            bool isThird = false;
            var memberId = GetMemberId();
            var shop = db.Query<Shop>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.ShopId)
                .FirstOrDefault();
            if (shop == null) throw new Exception("商铺纪录不存在");

            ShopMember shopMember = ShopMemberServer.GetShopMember(db, shop.Id, memberId);
            var shopMemberSet = ShopMemberServer.GetShopMemberSet(db, shop.Id);
            bool IsTopUpDiscount = shopMemberSet == null ? false : shopMemberSet.IsTopUpDiscount;
            double memberDiscount = 1;
            if (shopMember != null)
            {
                var shopMemberLevel = db.GetSingle<ShopMemberLevel>(shopMember.ShopMemberLevelId);
                if (shopMemberLevel != null) memberDiscount = shopMemberLevel.Discount;
            }

            var shoptakeoutinfo = db.ShopTakeOutInfo.FirstOrDefault(r => r.ShopId == args.ShopId && !r.IsDel);
            if (shoptakeoutinfo != null && shoptakeoutinfo.TakeDistributionType == TakeDistributionType.达达配送 && isTakeOut && args.TakeWay == TakeWay.送货上门)
            {
                isThird = true;
            }

            var model = new ShopOrder()
            {
                AddTime = DateTime.Now,
                MemberId = memberId,
                ShopId = args.ShopId,
                Remark = args.Remark,
                AddUser = $"member{memberId}",
                AddIp = GetIp(),
                Flag = Guid.NewGuid().ToString(),
                IsTakeOut = isTakeOut,
                Status = ShopOrderStatus.待支付,
                takeDistributionType = isThird == true ? TakeDistributionType.达达配送 : TakeDistributionType.商家配送
            };
            ShopOrderServer shopOrderServer = new ShopOrderServer(model);
            db.AddTo(model);
            if (isTakeOut)
                shopOrderServer.RecordTakeout(db, shop, memberId, args.TakeWay.Value, args.PickupTime);
            else if (args.IsSelfHelp.HasValue && args.IsSelfHelp.Value)
                shopOrderServer.RecordShopOrderSelfHelp(db, shop.Id, args.IsSelfHelpTakeOut ?? false);
            else if (!string.IsNullOrEmpty(args.ShopPartFlag))
                shopOrderServer.RecordScancode(db, args.ShopPartFlag);
            else
                throw new Exception("请先扫描桌上二唯码");
            int cartCount = 0;
            args.Items.ForEach(m => cartCount += m.Count);
            shopOrderServer.RecordOtherFee(db, args.TakeWay, cartCount);
            var commodityIdAndCounts = new Dictionary<int, int>();
            //商铺库存判断
            foreach (var item in args.Items)
            {
                var stock = db.Query<ShopCommodityStock>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == shop.Id)
                    .Where(m => m.Sku.Flag == item.SkuFlag)
                    .Select(m => new
                    {
                        m.Id,
                        m.Stock,
                        IsCombo = m.Sku.Commodity.CategoryId == 0, //是否套餐
                        CommodityName = m.Sku.Commodity.Name,
                        m.Sku.CommodityId,
                        m.CostPrice,
                        m.SalePrice,
                        m.MarketPrice,
                        HasVipPrice = m.Sku.Commodity.UseMemberPrice,
                        SkuSummary = m.Sku.Summary
                    })
                    .FirstOrDefault();
                if (stock == null) throw new Exception("商品不存在或者商铺商品未上架");
                if (stock.Stock <= 0 || stock.Stock < item.Count) throw new Exception("库存不足");

                db.AddStock(stock.Id, -item.Count);

                //纪录下commodity的count
                if (commodityIdAndCounts.ContainsKey(stock.CommodityId))
                {
                    commodityIdAndCounts[stock.CommodityId] += item.Count;
                }
                else
                {
                    commodityIdAndCounts.Add(stock.CommodityId, item.Count);
                }


                var orderItem = new ShopOrderItem()
                {
                    CommodityName = stock.CommodityName,
                    CommodityStockId = stock.Id,
                    AddIp = model.AddIp,
                    AddTime = model.AddTime,
                    AddUser = model.AddUser,
                    CostPrice = stock.CostPrice,
                    SalePrice = (stock.HasVipPrice && !stock.IsCombo && (!IsTopUpDiscount || args.IsBalance)) ? ShopMemberServer.GetMemberPrice(stock.SalePrice, memberDiscount) : stock.SalePrice,
                    PrimePrice = stock.SalePrice,
                    ShopOrder = model,
                    Count = item.Count,
                    SkuSummary = stock.SkuSummary
                };
                db.AddTo(orderItem);

                model.Amount += orderItem.SalePrice * orderItem.Count;
            }

            if (args.ComboPrice.HasValue) model.Amount += args.ComboPrice.Value;

            //这里尝试扣钱
            //var memberAmount = db.GetMemberAmountList(memberId);
            //var availAmount = memberAmount.GetAvailAmount();
            //if (availAmount >= model.Amount)
            //{
            //    memberAmount.DecreaseAvailAmount(model.Amount, 0, $"{shop.Name}消费", Newtonsoft.Json.JsonConvert.SerializeObject(new { shopId = shop.Id, orderFlag = model.Flag }), FinaceType.商品购买支出);
            //    memberAmount.UpdateMemberAmountCache();
            //    model.PayTime = DateTime.Now;

            //    //更新商品的销售量
            //    //注意，这里如果有一个品牌，多个店铺的情况，会出现销售额共享的情况
            //    var commodityIds = commodityIdAndCounts.Select(m => m.Key).ToList();
            //    var commoditys = db.Query<ShopBrandCommodity>()
            //            .Where(m => commodityIds.Contains(m.Id))
            //            .ToList();
            //    foreach (var item in commoditys)
            //    {
            //        item.SalesForMonth += commodityIdAndCounts[item.Id];
            //    }
            //    db.SaveChanges();
            //    AfterOrderPlacing(model, shop.Name);
            //}
            //else
            //{
            //    db.SaveChanges();
            //}
            model.OrderNumber = shopOrderServer.GenerateOrderNumber();

            shopOrderServer.ComputePayment(db);
            db.SaveChanges();
            model.Shop = null;  //不返回商店信息
            return Success(model);
        }

        [HttpPost]
        public async Task<APIResult> CTestThird([FromBody]ThirdOrderAPIModels model)
        {
            var order = new ShopOrder()
            {
                AddTime = DateTime.Now,
                MemberId = 0,
                ShopId = model.ShopId,
                Remark = "达达配送",
                AddUser = $"member",
                AddIp = GetIp(),
                Flag = Guid.NewGuid().ToString(),
                IsTakeOut = true,
                Status = ShopOrderStatus.待支付,
                OrderNumber = model.OrderId
                //TakeDistributionType = args.TakeDistributionType
            };
            db.Add(order);

            //处理达达配送
            ThirdServer thirdServer = new ThirdServer(db, thirdConfig);
            var shoptakeoutinfo = db.ShopTakeOutInfo.FirstOrDefault(r => r.ShopId == model.ShopId && !r.IsDel);
            if (shoptakeoutinfo.TakeDistributionType == TakeDistributionType.达达配送)
            {
                var merchant = db.Merchant.FirstOrDefault(r => r.ShopId == model.ShopId);
                if (merchant == null) throw new Exception("商户未在达达开户");
                var thirdshop = db.ThirdShop.FirstOrDefault(r => r.ShopId == model.ShopId);
                if (thirdshop == null) throw new Exception("商户门店不存在");
                model.thirdShopAddOrderModel.origin_id = model.OrderId;
                var result = await thirdServer.ThirdAddOrder(model.thirdShopAddOrderModel, null);
                if (result.errorCode == 0 || result.status == "success")
                {
                    db.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
                    {
                        OrderNumber = model.OrderId,
                        Amount = result.result.fee,
                        AddTime = DateTime.Now,
                        ProduceType = Data.ProduceType.发起订单
                    });

                }
                db.SaveChanges();
                return Success(result);
            }
            return Error("error");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<GetSingleModel> GetSingle([FromBody]IdArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopOrder>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            var items = db.Query<ShopOrderItem>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopOrderId == model.Id)
                .ToList();

            string title = null, orderStatus = null;
            if (model.ShopPartId.HasValue)
                title = db.GetSingle<ShopPart>(model.ShopPartId.Value)?.Title;
            string takeway, address = null, name = null, phone = null, orderTitle = null, selfHelpNumber = null;
            ShopOrderTakeout takeout = null;
            ShopOrderOtherFee otherFee = null;
            if (model.IsTakeOut)
            {
                orderTitle = "外卖单";
                takeout = db.Query<ShopOrderTakeout>()
                    .Where(m => !m.IsDel && m.ShopOrderId == model.Id)
                    .FirstOrDefault();
                if (takeout == null)
                    takeway = "外卖";
                else
                {
                    takeway = takeout.TakeWay.ToString();
                    address = takeout.Address;
                    name = takeout.Name;
                    phone = takeout.Phone;
                    orderStatus = takeout.Status.ToString();
                }
            }
            else if (model.ShopOrderSelfHelpId.HasValue)
            {
                orderTitle = "自助点餐";
                var selfHelp = db.Set<ShopOrderSelfHelp>().Find(model.ShopOrderSelfHelpId.Value);
                if (selfHelp.IsTakeOut)
                    takeway = "外带";
                else
                    takeway = "堂食";
                orderStatus = model.Status.ToString();
                selfHelpNumber = selfHelp.Number;
            }
            else
            {
                orderTitle = "堂食点餐";
                orderStatus = model.Status.ToString();
                takeway = "堂食点餐";
            }

            if (model.OtherFeeId.HasValue)
                otherFee = db.Set<ShopOrderOtherFee>().Find(model.OtherFeeId.Value);

            return Success(new GetSingleModel
            {
                Takeway = takeway,
                Address = address,
                OrderStatus = orderStatus,
                Name = name,
                Phone = phone,
                OrderTitle = orderTitle,
                SelfHelpNumber = selfHelpNumber,
                Title = title,
                OtherFee = otherFee,
                ExpectTitle = takeout?.TakeWay == TakeWay.自提 ? "预计自提时间" : "期望配送时间",
                ExpectTime = takeout?.PickupTime.ToString("HH:mm"),
                Order = model,
                Items = items
            });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetCancel([FromBody]IdArgsModel args)
        {
            //这里注意，会涉及到退款操作
            var memberId = GetMemberId();
            var model = db.Query<ShopOrder>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            model.Status = ShopOrderStatus.已取消;

            //这里直接扣钱
            //var memberAmount = db.GetMemberAmountList(memberId);
            //memberAmount.IncreaseAvailAmount(model.Amount, 0, $"订单{model.Flag}取消", $"订单编号：{model.Flag}", FinaceType.商品购买支出);
            //memberAmount.UpdateMemberAmountCache();

            db.SaveChanges();
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetRemark([FromBody]SetRemarkArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopOrder>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            model.Remark = args.Remark;
            db.SaveChanges();
            return Success(model);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetOrderInfo([FromBody]GetOrderInfoArgsModel args)
        {
            if (!args.shopOrderId.HasValue) throw new Exception("shopOrderId不能为空");
            ShopOrder shopOrder = db.Query<ShopOrder>()
                .Where(m => m.Id == args.shopOrderId.Value)
                .Include(m => m.ShopOrderOtherFee)
                .Include(m => m.ShopOrderSelfHelp)
                .Include(m => m.ShopOrderMoneyOffRule)
                .Include(m => m.ShopPart)
                .FirstOrDefault();

            if (shopOrder == null) throw new Exception("订单记录不存在");

            ShopOrderServer shopOrderServer = new ShopOrderServer(shopOrder);



            GetOrderInfoResultModel rtn = new GetOrderInfoResultModel()
            {
                Code = shopOrder.OrderNumber,
                CreateTime = shopOrder.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                PayTime = shopOrder.PayTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                PayMent = shopOrder.Payment / 100m,
                PayStatus = shopOrderServer.GetPayStatus(),
                OrderStatus = shopOrder.Status.ToString(),
                Remark = shopOrder.Remark ?? "",
                IsTakeOut = shopOrder.IsTakeOut,
                PayWay = shopOrder.PayWay

            };
            if (shopOrder.ShopOrderMoneyOffRule != null)
            {
                rtn.MoneyOffRule = new MoneyOffRule()
                {
                    Discount = shopOrder.ShopOrderMoneyOffRule.Discount / 100m,
                    FullAmount = shopOrder.ShopOrderMoneyOffRule.FullAmount / 100m
                };
            }
            if (shopOrder.ShopOrderOtherFee != null)
            {
                rtn.OtherFee = new OtherFee()
                {
                    BoxFee = shopOrder.ShopOrderOtherFee.BoxFee / 100m,
                    DeliveryFee = shopOrder.ShopOrderOtherFee.DeliveryFee / 100m
                };
            }
            if (shopOrder.ShopOrderSelfHelp != null)
            {
                rtn.SelfHelp = new SelfHelpInfo()
                {
                    SelfHelpCode = shopOrder.ShopOrderSelfHelp.Number,
                    TakeWay = shopOrder.ShopOrderSelfHelp.IsTakeOut ? "外带" : "堂食"
                };
            }
            if (shopOrder.ShopPart != null) rtn.ShopPartName = shopOrder.ShopPart.Title;
            if (shopOrder.IsTakeOut)
            {
                var takeOut = db.Query<ShopOrderTakeout>()
                    .Where(m => m.ShopOrderId == shopOrder.Id)
                    .FirstOrDefault();
                if (takeOut != null)
                {
                    rtn.TakeOutInfo = new TakeOutInfo()
                    {
                        DiningWay = takeOut.TakeWay.ToString(),
                        PickTile = takeOut.TakeWay == TakeWay.自提 ? "预计自提时间" : "期望配送时间",
                        PickUpTime = takeOut.PickupTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Address = takeOut.Address,
                        Person = takeOut.Name,
                        Phone = takeOut.Phone
                    };
                }

            }

            var orderItems = db.Query<ShopOrderItem>()
                .Where(m => !m.IsDel && m.ShopOrderId == args.shopOrderId.Value)
                .Select(m => new CommodityInfo()
                {
                    Cover = m.CommodityStock.Sku.Commodity.Cover,
                    Count = m.Count,
                    VipPrice = (m.CommodityStock.Sku.Commodity.CategoryId != 0 && m.CommodityStock.Sku.Commodity.UseMemberPrice),
                    PrimePrice = m.PrimePrice * m.Count / 100m,
                    Name = $"{m.CommodityName}({m.SkuSummary})",
                    SalePrice = m.SalePrice * m.Count / 100m
                }).ToList();
            rtn.Items = orderItems;
            if (shopOrder.takeDistributionType == TakeDistributionType.达达配送)
            {
                ThirdServer thirdServer = new ThirdServer(db, thirdConfig);
                var thirdqueryresult = await thirdServer.ThirdOrderQuery(new BLL.Third.COrderInfoQueryModel() { ShopId = shopOrder.ShopId, order_id = shopOrder.OrderNumber });
                rtn.cThirdInfoQuery = thirdqueryresult;
            }
            return Success(rtn);
        }



        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetOrderStatus([FromBody]SetStatusArgsModel args)
        {
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            model.Status = args.Status;
            db.SaveChanges();
            return Success();

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsPay([FromBody]IdArgsModel args)
        {
            //这里注意，会涉及到退款操作
            //var memberId = GetMemberId();
            //var model = db.Query<ShopOrder>()
            //    .Where(m => !m.IsDel)
            //    .Where(m => m.MemberId == memberId)
            //    .Where(m => m.Id == args.Id)
            //    .FirstOrDefault();
            //if (model == null)
            //{
            //    _logger.LogError("找不到订单,订单ID:{0},memberID:{1}", args.Id, memberId);
            //    throw new Exception("指定纪录不存在");
            //}
            ////if (model.PayTime.HasValue) throw new Exception("该订单已经支付");

            ////取消余额
            ////var memberAmount = db.GetMemberAmountList(memberId);
            ////var availAmount = memberAmount.GetAvailAmount();
            ////if (availAmount < model.Amount) throw new Exception("余额不足");

            //var shopName = db.Query<Shop>()
            //    .Where(m => m.Id == model.ShopId)
            //    .Select(m => m.Name)
            //    .FirstOrDefault();

            ////memberAmount.DecreaseAvailAmount(model.Amount, 0, $"{shopName}消费", Newtonsoft.Json.JsonConvert.SerializeObject(new { shopId = model.ShopId, orderFlag = model.Flag }), FinaceType.商品购买支出);
            ////memberAmount.UpdateMemberAmountCache();
            //model.PayTime = DateTime.Now;
            //AfterOrderPlacing(model, shopName);

            ////获取订单中商品的数量
            //var commodityIdAndCounts = db.Query<ShopOrderItem>()
            //    .Where(m => m.ShopOrderId == model.Id)
            //    .Where(m => !m.IsDel)
            //    .Select(m => new
            //    {
            //        CommodityId = m.CommodityStock.Sku.CommodityId,
            //        Count = m.Count
            //    })
            //    .ToList()
            //    .GroupBy(m => m.CommodityId)
            //    .ToDictionary(m => m.Key, m => m.Select(x => x.Count).Sum());

            ////更新商品的销售量
            ////注意，这里如果有一个品牌，多个店铺的情况，会出现销售额共享的情况
            //var commodityIds = commodityIdAndCounts.Select(m => m.Key).ToList();
            //var commoditys = db.Query<ShopBrandCommodity>()
            //        .Where(m => commodityIds.Contains(m.Id))
            //        .ToList();
            //foreach (var item in commoditys)
            //{
            //    item.SalesForMonth += commodityIdAndCounts[item.Id];
            //}
            //db.SaveChanges();
            return Success();
        }


    }
}
