using System;
using System.Linq;
using ZRui.Web.ShopCommodityStockSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCommodityStockSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommodityStockSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options,memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }




        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            var query = db.Query<ShopCommodityStock>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId)
                .Where(m => m.Sku.CommodityId == args.CommodityId)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    CostPrice = m.CostPrice,
                    Id = m.Id,
                    SalePrice = m.SalePrice,
                    MarketPrice = m.MarketPrice,
                    SkuId = m.SkuId,
                    Stock = m.Stock
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            //判定商品中是否已经指定sku的库存
            var query = db.Query<ShopCommodityStock>()
                    .Where(m => !m.IsDel);
            var isExit = query.Where(m => m.ShopId == args.ShopId && m.SkuId == args.SkuId).Count() > 0;
            if (isExit) throw new Exception("指定的规格已经添加到库存");

            var sku = db.GetSingle<ShopBrandCommoditySku>(args.SkuId);
            if (sku == null || sku.CommodityId != args.CommodityId) throw new Exception("指定的规格不存在");

            var shop = db.GetSingle<Shop>(args.ShopId);
            if (shop == null) throw new Exception("指定的商铺不存在");

            //这里只是添加一个库存纪录，库存的参数在编辑处修改
            var model = new ShopCommodityStock()
            {
                CostPrice = 0,
                SalePrice = 0,
                MarketPrice = 0,
                Shop = shop,
                Sku = sku,
                Stock = 0
            };

            db.Add<ShopCommodityStock>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var model = db.Query<ShopCommodityStock>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            model.Stock = args.Stock;
            model.CostPrice = args.CostPrice;
            model.SalePrice = args.SalePrice;
            model.MarketPrice = args.MarketPrice;

            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopCommodityStock>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSkuItems([FromBody]GetSkuItemsArgsModel args)
        {
            var query = db.Query<ShopBrandCommoditySkuItem>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.Sku.CommodityId == args.CommodityId)
                .OrderByDescending(m => m.Id)
                .Select(m => new SkuItem()
                {
                    Id = m.Id,
                    SkuId = m.SkuId,
                    Value = m.ParameterValue.Value
                })
                .ToList();

            return Success(new GetSkuItemsModel()
            {
                Items = list
            });
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
