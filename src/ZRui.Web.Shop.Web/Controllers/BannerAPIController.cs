using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Core.Wechat;
using ZRui.Web.ShopAPIModels;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 广告图
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class BannerAPIController : WechatApiControllerBase
    {
        ShopDbContext _db;
        public BannerAPIController(IOptions<MemberAPIOptions> options, ShopDbContext db, MemberDbContext memberDb, WechatCoreDbContext wechatCoreDb) : base(options, memberDb, wechatCoreDb)
        {
            this._db = db;
        }
        /// <summary>
        /// banner列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes ="jwt")]
        public APIResult BannerList([FromBody]GetSingleArgs args)
        {
            var shopbanners = _db.Query<Shop>().FirstOrDefault(r => r.Flag == args.Flag).Banners;
            var list = JsonConvert.DeserializeObject<List<BannerModel>>(shopbanners)
            .Where(r => r.IsShow == true)
            .OrderByDescending(r => r.Sorting);
            return Success(list);
        }
    }
}
