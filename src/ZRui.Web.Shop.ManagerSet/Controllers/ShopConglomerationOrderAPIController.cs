using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Models;
using AutoMapper;
using System.Collections.Generic;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;
using ZRui.Web.Pay;
using ZRui.Web.BLL;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 拼团订单处理
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopConglomerationOrderAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        private IMapper _mapper { get; set; }
        PayProxyFactory _proxyFactory;
        ILogger _logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopConglomerationOrderAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , PayProxyFactory proxyFactory
            , IMapper mapper
               , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this._proxyFactory = proxyFactory;
            _logger = loggerFactory.CreateLogger<ShopConglomerationOrderAPIController>();
            _mapper = mapper;
        }

        /// <summary>
        /// 添加快递单号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> AddExpressSingle([FromBody]AddExpressSingleModel input)
        {
            var order = db.ConglomerationOrder.Find(input.OrderId);
            if (!order.Type.Equals(ConsignmentType.快递))
            {
                return Error("订单类型为自提");
            }
            if (!order.Status.Equals(ShopOrderStatus.待配送))
            {
                return Error("订单不是待配送状态");
            }
            var express = db.ConglomerationExpress.FirstOrDefault(m => m.ShopConglomerationOrderId.Equals(input.OrderId));

            express.ExpressSingle = input.ExpressSingle;
            order.Status = ShopOrderStatus.已完成;
            db.SaveChanges();

            #region 积分获取
            var sourceType = await ShopIntegralRechargeServer.GetOrderSourceType(db, input.OrderId, true, _logger);
            await ShopIntegralRechargeServer.GetOrderIntegral(db, input.OrderId, sourceType, _logger);
            #endregion
            return Success();
        }



        /// <summary>
        /// 拼团订单退款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public APIResult OnRefund([FromBody]ConglomerationOrderRequestModel input)
        {
            var order = db.ConglomerationOrder.Find(input.ConglomerationOrderId);
            if (order == null)
            {
                return Error("未找到订单数据");
            }
            _logger.LogInformation($"退款订单信息：{order.Id}");

            try
            {
                if (!order.ShopId.Equals(input.ShopId))
                {
                    return Error("订单与商铺不匹配");
                }
                if (order.Status.Equals(ShopOrderStatus.待成团) || order.Status.Equals(ShopOrderStatus.待自提) || order.Status.Equals(ShopOrderStatus.待配送) || order.Status.Equals(ShopOrderStatus.退款审批))
                {
                    var shop = db.Shops.Find(order.ShopId);
                    Refunds refunds = new Refunds(_proxyFactory);
                    var memberTradeForRechange = db.MemberTradeForRechange.FirstOrDefault(m => m.Status.Equals(MemberTradeForRechangeStatus.成功) && m.ConglomerationOrderId.Equals(order.Id));
                    if (memberTradeForRechange == null)
                    {
                        return Error("无法找到支付记录");
                    }
                    var isOk = refunds.RefundAction(new RefundArgsModel() { ShopFlag = shop.Flag, TradeNo = memberTradeForRechange.TradeNo });
                    if (isOk.Status == MemberTradeForRefundStatus.成功)
                    {
                        order.Status = ShopOrderStatus.已退款;
                        db.SaveChanges();
                        return Success("您的退款申请已提交，银行处理退款中");
                    }
                    if (isOk.Status == MemberTradeForRefundStatus.退款中)
                    {
                        order.Status = ShopOrderStatus.退款中;
                        db.SaveChanges();
                        return Success("您的退款申请已提交，银行处理退款中");
                    }
                }
                order.Status = ShopOrderStatus.退款中;
                return Error("退款失败请联系管理员");

            }
            catch (Exception e)
            {

                _logger.LogInformation($"退款订单信息：{e}");
                return Error(e.Message);
            }
        }

        /// <summary>
        /// 拼团订单自提完成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> TakeTheirFinish([FromBody]AddExpressSingleModel input)
        {
            return await TakeTheirFinishOrderAsync(input.OrderId, input.PickupCode);
        }

        [HttpGet]
        public async Task TakeTheirFinishForScan(int orderId, string pickupCode)
        {
            string resultStr = string.Empty;
            string html = "<!doctype html><html lang = \"en\" ><head><meta charset = \"UTF-8\"><meta name = \"viewport\" content = \"width=device-width,initial-scale=1 user-scalable=0\" /><title></title></head><body><p style=\"font - size: 36px;margin: 50px;\">{0}</p></body></html>";
            var result = db.ConglomerationOrder.FirstOrDefault(p => p.Id == orderId && p.PickupCode == pickupCode);
            if (result != null && result.Status == ShopOrderStatus.已完成)
            {
                resultStr = "订单" + pickupCode + "已核销成功";
            }
            resultStr = string.Format(html, resultStr);
            Response.ContentType = "text/html";
            var data = Encoding.UTF8.GetBytes(resultStr);
            await Response.Body.WriteAsync(data, 0, data.Length);
        }



        private async Task<APIResult> TakeTheirFinishOrderAsync(int orderId, string pickupCode)
        {
            await ShopConglomerationOrderOptions.TakeTheirFinishOrderAsync(db, orderId, pickupCode, _logger);
            return Success();
        }


        /// <summary>
        /// 打回申请退款的订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult NoRefund([FromBody]ConglomerationOrderRequestModel input)
        {

            var order = db.ConglomerationOrder.Find(input.ConglomerationOrderId);
            if (order == null)
            {
                return Error("未找到订单数据");
            }
            if (order.Status != ShopOrderStatus.退款审批)
            {
                return Error("订单状态错误");
            }

            order.Status = ShopOrderStatus.已完成;
            db.SaveChanges();
            return Success();
        }


    }
}
