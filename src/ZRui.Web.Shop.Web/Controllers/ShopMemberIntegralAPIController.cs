using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using ZRui.Web.ShopMemberAPIModels;
using System.Linq;
using System.Threading.Tasks;
using ZRui.Web.Models;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.ServerDto;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Common;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 会员卡
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class ShopMemberIntegralAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopMemberIntegralAPIController(
            IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }


        /// <summary>
        /// 用户消费添加积分
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetIntegral([FromBody]GetIntegralModel input)
        {
            if (input.SourceType != SourceType.拼团订单)
            {
                var order = await db.ShopOrders.FindAsync(input.OrderId);
                if (order.IsTakeOut)
                {
                    input.SourceType = SourceType.外卖订单;
                }
                if (order.ShopPartId.HasValue)
                {
                    input.SourceType = SourceType.扫码点餐订单;
                }
                if (order.ShopOrderSelfHelpId.HasValue)
                {
                    input.SourceType = SourceType.自助点餐订单;
                }
            }
            await ShopIntegralRechargeServer.GetOrderIntegral(db, input.OrderId, input.SourceType, _logger);
            return await Task.FromResult(Success());
        }


        /// <summary>
        /// 获取积分列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetIntegralList([FromBody]GetPagedListBaseModelB input)
        {
            input.MemberId = GetMemberId();
            var shopMember = await db.ShopMembers.FirstOrDefaultAsync(m => m.MemberId.Equals(input.MemberId) && !m.IsDel);
            if (shopMember == null)
            {
                return Error("未注册");
            }
            var shopIntegralRecharges = db.ShopIntegralRecharge.
                  Where(m => !m.IsDel && m.MemberId.Equals(input.MemberId) && m.ShopId.Equals(input.ShopId) && m.ShopMemberId.Equals(shopMember.Id))
                 .ToPagedList(input.PageIndex, input.PageSize)
                 .Select(m => new
                 {
                     m.Id,
                     IntegralCount = m.Count,
                     m.CodeStatut,
                     m.SourceRemark,
                     AddTime = m.AddTime.ToString("yyyy-MM-dd HH:mm:ss")
                 });
            return await Task.FromResult(Success(new
            {
                input.PageIndex,
                input.PageSize,
                shopIntegralRecharges = shopIntegralRecharges,
                shopMember.Credits
            }));
        }
    }
}
