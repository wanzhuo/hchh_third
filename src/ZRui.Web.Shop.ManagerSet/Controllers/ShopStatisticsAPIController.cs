using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopStatisticsAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using System.Collections.Generic;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopStatisticsAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopStatisticsAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        public APIResult GetTotal([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);
            var brandId = db.Query<Shop>()
                .Where(m => m.Id == args.ShopId.Value)
                .Select(m => m.ShopBrandId)
                .FirstOrDefault();

            var viewModel = new GetTotalModel()
            {

            };

            viewModel.OrderCount = db.Query<ShopOrder>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Count();

            viewModel.CommodityCount = db.Query<ShopBrandCommodity>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == brandId)
                     .Count();

            //客户数量，这里读取的是已经下过单的客户数量
            viewModel.MemberCount = db.Query<ShopOrder>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Select(m => m.MemberId)
                     .Distinct()
                     .Count();

            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult GetOrderCountForDay([FromBody]GetOrderCountForDayArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            if (!args.StartDate.HasValue) args.StartDate = DateTime.Now.Date.AddDays(-7);
            if (!args.EndDate.HasValue) args.EndDate = DateTime.Now.Date;

            args.EndDate = args.EndDate.Value.AddDays(1);

            var brandId = db.Query<Shop>()
                .Where(m => m.Id == args.ShopId.Value)
                .Select(m => m.ShopBrandId)
                .FirstOrDefault();

            var query = db.Query<ShopOrder>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value);

            query = query.Where(m => m.AddTime > args.StartDate.Value);
            query = query.Where(m => m.AddTime < args.EndDate.Value);

            var items = query
                .Select(m => m.AddTime)
                .ToList()
                .GroupBy(m => m.Date)
                .Select(m => new RowItem()
                {
                    Date = m.Key.Date,
                    Count = m.Count()
                })
                .ToList();

            return Success(new GetOrderCountForDayModel()
            {
                Items = items
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetCallingSuccessTotal([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var viewModel = new GetCallingQueueModel();

            viewModel.CallingSuccessTotal = db.Query<ShopCallingQueue>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Where(m => m.IsUsed)
                     .Count();

            //客户数量
            viewModel.MemberCount = db.Query<ShopCallingQueue>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Where(m => m.IsUsed)
                     .Select(m => m.MemberId)
                     .Distinct()
                     .Count();

            return Success(viewModel);
        }


        [HttpPost]
        [Authorize]
        public APIResult GetBookTotal([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var viewModel = new GetBookingModel();

            viewModel.BookTotal = db.Query<ShopBooking>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Where(m => m.Status == ShopBookingStatus.确认成功)
                     .Count();

            viewModel.BookAndUse = db.Query<ShopBooking>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Where(m => m.IsUsed)
                     .Count();

            //客户数量
            viewModel.MemberCount = db.Query<ShopBooking>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId.Value)
                     .Where(m => m.IsUsed)
                     .Select(m => m.MemberId)
                     .Distinct()
                     .Count();

            return Success(viewModel);
        }

    }
}
