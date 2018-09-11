using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;
using ZRui.Web.Common;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Models;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 第三方配送
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class ThirdAPIController : WechatApiControllerBase
    {
        ShopDbContext _db;
        ThirdConfig thirdConfig;
        // ThirdServer thirdServer;
        private IMapper _mapper { get; set; }
        public ThirdAPIController(IOptions<MemberAPIOptions> options, IOptions<ThirdConfig> poptions, ShopDbContext db, MemberDbContext memberDb, WechatCoreDbContext wechatCoreDb, IMapper mapper) : base(options, memberDb, wechatCoreDb)
        {
            this._db = db;
            //this.thirdServer = thirdServer;
            this._mapper = mapper;
            this.thirdConfig = poptions.Value;
        }

        /// <summary>
        /// 第三方配送订单查询（达达）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> ThirdOrderQuery([FromBody] COrderInfoQueryModel model)
        {
            ThirdServer thirdServer = new ThirdServer(_db,thirdConfig);
            var thirdorder = await thirdServer.ThirdOrderQuery(model);
            return Success(thirdServer);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult ThirdOrderList([FromBody] GetPagedListBaseModel model)
        {

            var list = _db.ThirdOrder
                .Where(r => r.ShopId == model.ShopId)
                .ToPagedList(model.PageIndex, model.PageSize);
            var result = _mapper.Map<PagedList<CThirdOrdersModel>>(list);
            result.PageIndex = list.PageIndex;
            result.PageSize = list.PageSize;
            result.TotalItemCount = list.TotalItemCount;

            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = result.ToList()
            });

        }

        /// <summary>
        /// 第三方配送订单取消（达达）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> ThirdFormalCancel([FromBody] CFormalCancel model)
        {
            ThirdServer thirdServer = new ThirdServer(_db,thirdConfig);
            var result = await thirdServer.ThirdFormalCancel(model);
            return Success(result);
        }

        /// <summary>
        /// 取消订单原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> ThirdFormalCancel()
        {
            ThirdServer thirdServer = new ThirdServer(_db,thirdConfig);
            var result = await thirdServer.ThirdFormalCancel();
            return Success(result);

        }
    }
}
