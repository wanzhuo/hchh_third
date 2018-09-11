using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;
using ZRui.Web.ShopManager.PrintExtension;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Wechat;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.OrderHandlers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopOrderSetAPIController : ShopManagerApiControllerBase
    {
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        PrintDbContext printDbContext;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        private IMapper _mapper { get; set; }
        public ShopOrderSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , PrintDbContext printDbContext
            , ILoggerFactory loggerFactory
            , IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions
            , MemberDbContext memberDb
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.printDbContext = printDbContext;
            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
            this.hostingEnvironment = hostingEnvironment;
            this._mapper = mapper;
            _logger = loggerFactory.CreateLogger<ShopOrderSetAPIController>();
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopOrder>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddTime = m.AddTime,
                    Remark = m.Remark,
                    Status = m.Status,
                    Phone = m.Phone,
                    Id = m.Id,
                    MemberId = m.MemberId,
                    ShopId = m.ShopId,
                    ShopPartTitle = m.ShopPart.Title,
                    ShopPartId = m.ShopPartId,
                    AddIp = m.AddIp,
                    AddUser = m.AddUser,
                    Amount = m.Amount,
                    FinishTime = m.FinishTime,
                    Flag = m.Flag,
                    PayTime = m.PayTime
                })
                .ToList();

            return Success(new ShopOrderSetAPIModels.GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        //[Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListRequestModel args)
        {

            CheckShopActor(args.ShopId, ShopActorType.超级管理员);
            var orderContext = new OrderContext(args.OrderType, _mapper);
            var list = orderContext.ExecuteGetPagedList(args, db);


            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.ToList()
            });

        }


        [HttpPost]
        [Authorize]
        public APIResult GetOrderItems([FromBody]GetOrderItemsArgsModel args)
        {
            if (args.OrderType.Equals(OrderTypeE.拼团订单))
            {
                var model = db.GetSingle<ConglomerationOrder>(args.OrderId);
                if (model == null) throw new Exception("订单记录不存在");
                //获取到订单后判断是否拥有指定的店铺的权限
                CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            }
            else
            {
                var model = db.GetSingle<ShopOrder>(args.OrderId);
                if (model == null) throw new Exception("订单记录不存在");
                //获取到订单后判断是否拥有指定的店铺的权限
                CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            }


            var orderContext = new OrderContext(args.OrderType, _mapper);

            if (args.OrderType.Equals(OrderTypeE.拼团订单))
            {
                var query = orderContext.ExecuteGetOrderItems<ConglomerationOrderModel>(args, db);
                var shopMember = db.ShopMembers.FirstOrDefault(m => !m.IsDel && m.MemberId.Equals(query.MemberId));
                query.OrderShopMember = _mapper.Map<OrderShopMember>(shopMember);
                return Success(query);
            }
            else
            {
                var query = orderContext.ExecuteGetOrderItems<List<ShopOrderItem>>(args, db);
                var list = query
                    .Where(m => m.ShopOrderId == args.OrderId)
                    .OrderByDescending(m => m.Id)
                    .Select(m => new
                    {
                        AddTime = m.AddTime,
                        Id = m.Id,
                        AddIp = m.AddIp,
                        AddUser = m.AddUser,
                        CommodityName = m.CommodityName,
                        CostPrice = m.CostPrice,
                        Count = m.Count,
                        MarketPrice = m.MarketPrice,
                        SalePrice = m.SalePrice,
                        SkuSummary = m.SkuSummary
                    })
                    .ToList();

                ShopOrder shopOrder = db.Query<ShopOrder>()
                .Where(m => m.Id == args.OrderId)
                .Include(m => m.ShopOrderOtherFee)
                .Include(m => m.ShopOrderSelfHelp)
                .Include(m => m.ShopOrderMoneyOffRule)
                .Include(m => m.ShopPart)
                .FirstOrDefault();
                GetOrderInfoResultModel rtn = new GetOrderInfoResultModel()
                {
                    Code = shopOrder.OrderNumber,
                    CreateTime = shopOrder.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    PayTime = shopOrder.PayTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    PayMent = shopOrder.Payment / 100m,
                    Remark = shopOrder.Remark ?? "",
                    PayWay = shopOrder.PayWay ?? ""
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

                var shopMember = db.ShopMembers.FirstOrDefault(m => !m.IsDel && m.MemberId.Equals(shopOrder.MemberId));
                rtn.Items = list;
                rtn.OrderShopMember = _mapper.Map<OrderShopMember>(shopMember);



                return Success(rtn);
            }

        }

        [HttpPost]
        [Authorize]
        public APIResult SetStatus([FromBody]SetStatusArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.Status = args.Status;


            if (model.Status == ShopOrderStatus.已确认 && model.IsTakeOut)
            {
                //确认外卖订单
                var shopName = db.Query<Shop>()
                .Where(m => m.Id == model.ShopId)
                .Select(m => m.Name)
                .FirstOrDefault();
                printDbContext.PrintOrder(db, model, shopName);
            }
            else if (model.Status == ShopOrderStatus.已完成)
            {
                model.FinishTime = DateTime.Now;
            }

            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public async Task<APIResult> SetOrderStatus([FromBody]SetStatusArgsModel args)
        {
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            model.Status = args.Status;
            db.SaveChanges();
            #region 积分获取
            var sourceType = await ShopIntegralRechargeServer.GetOrderSourceType(db, args.Id, false, _logger);
            await ShopIntegralRechargeServer.GetOrderIntegral(db, args.Id, sourceType, _logger);
            #endregion
            return Success();

        }

        [HttpGet]
        public ActionResult GetShopOrderView(GetShopOrderViewArgsModel args)
        {
            var order = db.Set<ShopOrder>().Find(args.orderid);
            if (order == null) throw new Exception("该订单不存在");

            var Oauth = db.Query<ShopOrderReceiver>()
                .Where(m => !m.IsDel)
                .Where(m => m.ReceiverOpenId == args.openid)
                .Where(m => m.ShopId == order.ShopId)
                .FirstOrDefault();

            if (Oauth == null) return View("unauthorized");

            var items = db.Query<ShopOrderItem>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopOrderId == order.Id)
                    .ToList();
            if (items.Count == 0) throw new Exception($"该订单出错，订单号为：{order.Id}");

            var orderitems = items.Select(m =>
            {
                double p = Math.Round(m.SalePrice / 100d, 2);
                return new ShopOrderItemInfo()
                {
                    Name = m.CommodityName,
                    Price = p,
                    Count = m.Count,
                    Amount = p * m.Count,
                    SkuSummary = m.SkuSummary
                };
            }).ToList();
            string address = null;
            string takeway = null;
            string phone = null;
            string name = null;
            DateTime pickupTime = new DateTime();
            if (order.IsTakeOut)
            {
                var takeout = db.Query<ShopOrderTakeout>()
                    .Where(m => !m.IsDel && m.ShopOrderId == order.Id)
                    .FirstOrDefault();
                if (takeout != null)
                {
                    address = takeout.Address;
                    takeway = takeout.TakeWay.ToString();
                    phone = takeout.Phone;
                    name = takeout.Name;
                    pickupTime = takeout.PickupTime;
                }
            }

            if (!order.Payment.HasValue) order.Payment = 0;
            //其它费用
            ShopOrderOtherFee otherFee = null;
            if (order.OtherFeeId.HasValue)
                otherFee = db.GetSingle<ShopOrderOtherFee>(order.OtherFeeId.Value);
            //满减折扣
            ShopOrderMoneyOffRule moneyOff = null;
            if (order.MoneyOffRuleId.HasValue)
                moneyOff = db.GetSingle<ShopOrderMoneyOffRule>(order.MoneyOffRuleId.Value);

            Member member = memberDb.GetSingleMember(order.MemberId);

            GetShopOrderViewResultModel rtn = new GetShopOrderViewResultModel()
            {
                MemberId = order.MemberId,
                Id = order.Id,
                PayTime = order.PayTime,
                Address = address,
                ShopOrderItems = orderitems,
                OrderAmount = Math.Round(order.Amount / 100d, 2),
                ShopOrderOtherFee = otherFee,
                ShopOrderMoneyOffRule = moneyOff,
                Remark = order.Remark,
                TakeWay = takeway,
                PayAmount = Math.Round(order.Payment.Value / 100d, 2),
                Headimgurl = member.Avatar,
                NickName = member.NickName,
                Name = name,
                Phone = phone,
                OrderNumber = order.OrderNumber,
                PickupTime = pickupTime,
                PayWay = order.PayWay??""

            };
            if (order.ShopPartId.HasValue)
            {
                rtn.ShopPartName = db.Set<ShopPart>().Find(order.ShopPartId).Title;
            }
            else
            {
                rtn.ShopPartName = null;
            }
            if (order.ShopOrderSelfHelpId.HasValue)
            {
                rtn.ShopOrderSelfHelp = db.GetSingle<ShopOrderSelfHelp>(order.ShopOrderSelfHelpId.Value);
            }

            return View(rtn);
        }


        /// <summary>
        /// 拼团订单推送页面
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetConglomerationOrderView(GetShopOrderViewArgsModel args)
        {
            var order = db.Set<ConglomerationOrder>().Find(args.orderid);
            order.ConglomerationActivity = db.ConglomerationActivity.Find(order.ConglomerationActivityId);
            order.ConglomerationExpress = db.ConglomerationExpress.Find(order.ConglomerationExpressId);
            if (order == null) throw new Exception("该订单不存在");
            var member = db.Member.Find(order.MemberId);

            var result = _mapper.Map<ConglomerationOrderListResultModel>(order);
            result.NickName = member.NickName;
            result.AvatarUrl = member.Avatar;
            return View(result);
        }

        [HttpGet]
        public ActionResult LastShopOrderView(string code)
        {
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(wechatTemplateSendOptions.AppId,
                wechatTemplateSendOptions.AppSecret, code);
            var shopIds = db.Query<ShopOrderReceiver>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ReceiverOpenId == result.openid)
                        .Select(m => m.ShopId)
                        .Distinct();

            var orders = db.Query<ShopOrder>()
                //.Where(m => !m.IsDel)
                .Where(m => shopIds.Contains(m.ShopId))
                .Where(m => m.PayTime.HasValue)
                .OrderByDescending(m => m.AddTime)
                .Take(20);
            return View(orders.ToList());
        }

        [HttpPost]
        [Authorize]
        public APIResult SetTakeoutStatus([FromBody]SetTakeoutStatusArgsModel args)
        {
            if (!args.Id.HasValue) throw new Exception("订单id不能为空");
            if (!args.status.HasValue) throw new Exception("状态不能为空");

            var model = db.Query<ShopOrderTakeout>()
                .Where(m => !m.IsDel && m.ShopOrderId == args.Id.Value)
                .FirstOrDefault();
            if (model == null) throw new Exception("记录不存在");
            model.Status = args.status.Value;
            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);


            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

    }
}
