using System;
using System.Linq;
using ZRui.Web.ShopOrderSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopOrderSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopOrderSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            var query = db.Query<ShopOrder>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId)
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
                    AddIp = m.AddIp,
                    AddUser = m.AddUser,
                    Amount = m.Amount,
                    FinishTime = m.FinishTime,
                    Flag = m.Flag,
                    PayTime = m.PayTime
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }


        [HttpPost]
        [Authorize]
        public APIResult GetOrderItems([FromBody]GetOrderItemsArgsModel args)
        {
            var query = db.Query<ShopOrderItem>()
                     .Where(m => !m.IsDel);

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

            return Success(list);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetStatus([FromBody]SetStatusArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopOrder>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            model.Status = args.Status;

            if (model.Status == ShopOrderStatus.已完成)
            {
                model.FinishTime = DateTime.Now;
            }

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

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetShopBrands([FromBody]CommunityArgsModel args)
        {
            var query = db.Query<ShopBrand>()
                      .Where(m => !m.IsDel);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new ShopBrandsItem()
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToList();

            return Success(new GetShopBrandsModel()
            {
                Items = list
            });
        }


        [HttpPost]
        [Authorize]
        public APIResult GetShops([FromBody]GetShopsArgsModel args)
        {
            var query = db.Query<Shop>()
                      .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopBrandId == args.ShopBrandId)
                .OrderByDescending(m => m.Id)
                .Select(m => new ShopItem()
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToList();

            return Success(new GetShopsModel()
            {
                Items = list
            });
        }
    }
}
