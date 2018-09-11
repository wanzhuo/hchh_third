using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Extension;
using ZRui.Web.Models;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopDayOpenReportAPIController : ShopManagerApiControllerBase
    {
        ShopDbContext db;
        FinanceDbContext financeDb;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopDayOpenReportAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , FinanceDbContext financeDb
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            _mapper = mapper;
            this.financeDb = financeDb;
            this._hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public APIResult ShopDayOpenReport([FromBody]ShopDayOpenReportAPIArgModels args)
        {
            if (args.StartTime.Year < 1900 || args.EndTime.Year < 1900)
            {
                throw new Exception("请输入查询时间");
            }

            var shop = db.Set<Shop>().FirstOrDefault(r => r.Id == args.ShopId && !r.IsDel);
            if (shop == null) throw new Exception("店铺不存在");
            var memberId = GetMemberId();
            var member = memberDb.Set<Member>().FirstOrDefault(r => r.Id == memberId && !r.IsDel);
            if (member == null) throw new Exception("操作员不存在");
            return Success(db.ExShopDayOpenReportSumAmount(args, member, shop));
        }
    }
}
