using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using ZRui.Web.BLL.ServerDto;
using System.Collections.Generic;
using ZRui.Web.ShopMemberTopUpAPIModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 充值设置
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class ShopMemberTopUpAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ILogger _logger;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment hostingEnvironment;
        public ShopMemberTopUpAPIController(
            IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }





        /// <summary>
        /// 获取固定充值设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> GetTopUp([FromBody]ShopIdModel input)
        {
            var shopTopUpSets = db.ShopTopUpSet.Where(m => m.ShopId.Equals(input.ShopId) && !m.IsDel).ToList();
            var addshopMemberCardInfo = _mapper.Map<List<GetTopUpModel>>(shopTopUpSets);

            return await Task.FromResult(Success(addshopMemberCardInfo));
        }

        /// <summary>
        /// 获取固定充值设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> GetShopCustomTopUpSet([FromBody]ShopIdModel input)
        {
            var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => !m.IsDel && m.ShopId.Equals(input.ShopId));
            var shopCustomTopUpSet = await db.ShopCustomTopUpSet.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);
            var getCustomTopUpModel = _mapper.Map<GetCustomTopUpModel>(shopCustomTopUpSet);
            getCustomTopUpModel.IsShowCustomTopUpSet = shopMemberSet.IsShowCustomTopUpSet;
            return await Task.FromResult(Success(getCustomTopUpModel));
        }

    }
}
