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
using AutoMapper;
using ZRui.Web.Models;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Common;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 拼团订单
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class ShopConglomerationOrderAPIController : WechatApiControllerBase
    {
        private IMapper _mapper { get; set; }
        static object lockAddObject = new object();
        ShopDbContext db;
        FinanceDbContext _financedb;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopConglomerationOrderAPIController(
            IOptions<MemberAPIOptions> options
            , IOptions<WechatTemplateSendOptions> tOption
            , ShopDbContext db
            , FinanceDbContext financedb
              , IMapper mapper
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this._financedb = financedb;
            _mapper = mapper;
            wechatTemplateSendOptions = tOption.Value;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }

        /// <summary>
        /// 获取我的订单列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetOrderList([FromBody]GetOrderListRequretModel input)
        {
            var shop = db.Shops.Find(input.ShopId);
            int memberId = GetMemberId();
            var query = db.Query<ConglomerationOrder>()
               .Where(m => m.ShopId == input.ShopId)
               .Where(m => m.MemberId.Equals(memberId))
               //.Where(m => !m.Status.Equals(ShopOrderStatus.未处理))
               .AsNoTracking();

            if (input.Status != ShopOrderStatus.已完成)
            {
                query = query.Where(m => !m.Status.Equals(ShopOrderStatus.已完成) && !m.Status.Equals(ShopOrderStatus.未处理) && !m.Status.Equals(ShopOrderStatus.已退款));
            }
            else
            {
                query = query.Where(m => m.Status.Equals(ShopOrderStatus.已完成) || m.Status.Equals(ShopOrderStatus.已退款));

            }
            var query2 = query.OrderByDescending(m => m.CreateTime).ToPagedList(input.PageIndex, input.PageSize);
            var result = query2.ToList().Select(m =>
            {
                var resultItem = _mapper.Map<GetOrderListResultModel>(m);
                var conglomerationActivity = db.ConglomerationActivity.Find(m.ConglomerationActivityId);
                resultItem.ShopAddress = shop.Address;
                resultItem.CoverPortal = conglomerationActivity.CoverPortal;
                resultItem.ActivityName = conglomerationActivity.ActivityName;
                return resultItem;
            });

            return Success(new
            {
                query2.PageIndex,
                query2.PageSize,
                TotalCount = query2.TotalItemCount,
                Items = result
            });
        }
        /// <summary>
        /// 获取拼团订单详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetOrderDetails([FromBody]GetGetOrderStatusModel input)
        {
            var shop = db.Shops.Find(input.ShopId);
            var order = db.ConglomerationOrder.Find(input.OrderId);
            order.ConglomerationActivity = db.ConglomerationActivity.Find(order.ConglomerationActivityId);
            order.ConglomerationSetUp = db.ConglomerationSetUp.Find(order.ConglomerationSetUpId);
            var resultItem = _mapper.Map<GetOrderListResultModel>(order);
            if (order.Type == ConsignmentType.快递)
            {
                order.ConglomerationExpress = db.ConglomerationExpress.Find(order.ConglomerationExpressId);
            }
            resultItem.ShopAddress = shop.Address;
            resultItem.ConglomerationExpress = order.ConglomerationExpress;

            return Success(resultItem);

        }
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetOrderStatus([FromBody]GetGetOrderStatusModel input)
        {

            var memberTradeForRechanges = _financedb.MemberTradeForRechanges.FirstOrDefault(m => m.ConglomerationOrderId.Equals(input.OrderId) && (m.Status == MemberTradeForRechangeStatus.成功));
            if (memberTradeForRechanges != null)
            {
                return Success();
            }
            return Error("未支付");

        }

        /// <summary>
        /// 用户申请退款审批
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult ApplicationDrawback([FromBody]GetGetOrderStatusModel input)
        {
            var conglomerationOrder = db.ConglomerationOrder.Find(input.OrderId);
            if (conglomerationOrder == null)
            {
                return Error("订单不存在");
            }
            if (conglomerationOrder.Status != ShopOrderStatus.已完成)
            {
                return Error("订单未完成");
            }
            conglomerationOrder.Status = ShopOrderStatus.退款审批;
            db.SaveChanges();
            return Success("申请成功");
        }

        /// <summary>
        /// 获取提货码核销二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public ActionResult GetPickupCodeRQcode(int OrderId, int shopId)
        {
            //var memberId = GetMemberId();
            //var memberId = 212;
            var conglomerationOrder = db.ConglomerationOrder.FirstOrDefault(m => m.Id.Equals(OrderId) && m.ShopId.Equals(shopId) && !m.IsDel);
            if (conglomerationOrder == null)
            {
                return Content("订单不存在");
            }
            if (conglomerationOrder.Type != ConsignmentType.自提)
            {
                return Content("订单不存在提货吗");
            }


            var redictUrl = $"https://wxapi.91huichihuihe.com/api/ShopConglomerationOrderAPI/CncelPickupCode?orderId={OrderId}&pickupCode={conglomerationOrder.PickupCode}";
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(wechatTemplateSendOptions.AppId, redictUrl,
                    conglomerationOrder.ShopId.ToString(), Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);

            var bitmap = CodeHelper.CreateCodeEwmRuBitmap(result);
            return File(CodeHelper.BitmapToBytes(bitmap), "image/png");
        }


        /// <summary>
        /// 开始核销
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CncelPickupCode(int orderId, string pickupCode, string code, int state)
        {

            _logger.LogInformation($"=========================开始核销CncelPickupCode方法开始============================");

            _logger.LogInformation($"orderId ={orderId}  shopid={state}");
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(wechatTemplateSendOptions.AppId,
                   wechatTemplateSendOptions.AppSecret, code);
            Senparc.Weixin.MP.AdvancedAPIs.OAuth.OAuthUserInfo userInfo = null;
            if (result != null && !string.IsNullOrEmpty(result.openid))
            {
                string openId = result.openid;
                string access_token = result.access_token;
                userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(access_token, openId);
                if (userInfo != null && !string.IsNullOrEmpty(userInfo.openid))
                {
                    var shopServiceUserInfo = await db.ShopServiceUserInfo.FirstOrDefaultAsync(
                            m => m.Openid.Equals(userInfo.openid) &&
                            m.Unionid.Equals(userInfo.unionid) &&
                            m.ShopId.Equals(state) &&
                            !m.IsDel
                            );
                    _logger.LogInformation($"userInfo ={userInfo.nickname}");

                    if (shopServiceUserInfo == null)
                    {
                        _logger.LogInformation($"=========================开始核销CncelPickupCode方法结束============================");
                        return Json("无核销权限");
                    }
                    var isOk = await ShopConglomerationOrderOptions.TakeTheirFinishOrderAsync(db, orderId, pickupCode, _logger);
                    if (isOk)
                    {
                        _logger.LogInformation($"=========================开始核销CncelPickupCode方法结束============================");
                        return Json("核销成功");
                    }
                }
            }
            _logger.LogInformation($"=========================开始核销CncelPickupCode方法结束============================");

            return Json("核销失败");

        }



        //[HttpPost]
        //[Authorize]
        //public async System.Threading.Tasks.Task<APIResult> GetOAuthUrl([FromBody]GetOAuthQrcodeArgsModel args)
        //{
        //    if (!args.ShopId.HasValue) throw new ArgumentNullException("shopId");
        //    CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);
        //    var redictUrl = "http://manager.91huichihuihe.com/ShopWechatOpenOAuth/ShopReceiverOAuth";
        //    var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(wechatTemplateSendOptions.AppId, redictUrl,
        //            args.ShopId.Value.ToString(), Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);
        //    return Success(result);
        //}

    }
}
