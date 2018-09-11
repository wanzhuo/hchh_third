using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopCommodityStockSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopCommodityStockSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommodityStockSetAPIController(ICommunityService communityService
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
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopCommodityStock>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
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
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            //判定商品中是否已经指定sku的库存
            var query = db.Query<ShopCommodityStock>()
                    .Where(m => !m.IsDel);
            var isExit = query.Where(m => m.ShopId == args.ShopId && m.SkuId == args.SkuId).Count() > 0;
            if (isExit) throw new Exception("指定的规格已经添加到库存");

            var sku = db.GetSingle<ShopBrandCommoditySku>(args.SkuId);
            if (sku == null || sku.CommodityId != args.CommodityId) throw new Exception("指定的规格不存在");

            var shop = db.GetSingle<Shop>(args.ShopId.Value);
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

            //获取纪录进行权限判定
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

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

            //获取纪录进行权限判定
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAllIsDelete([FromBody]SetAllIsDeleteArgsModel args)
        {
            if (!args.shopId.HasValue) throw new ArgumentNullException("shopid");
            if (!args.commodityId.HasValue) throw new ArgumentNullException("commodityId");
            int shopId = args.shopId.Value;
            //获取纪录进行权限判定
            CheckShopActor(shopId, ShopActorType.超级管理员);

            var skuIds = db.Query<ShopBrandCommoditySku>()
                .Where(m => !m.IsDel)
                .Where(m => m.CommodityId == args.commodityId.Value)
                .Select(m => m.Id);

            db.Query<ShopCommodityStock>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Where(m => skuIds.Contains(m.SkuId))
                .ToList()
                .ForEach(m => m.IsDel = true);

            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSkuItems([FromBody]GetSkuItemsArgsModel args)
        {
            if (!args.CommodityId.HasValue) throw new ArgumentNullException("CommodityId");
            var commodity = db.Query<ShopBrandCommodity>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.CommodityId.Value)
                .Select(m => new
                {
                    Id = m.Id,
                    ShopBrandId = m.ShopBrandId
                })
                .FirstOrDefault();
            CheckShopBrandActor(commodity.ShopBrandId, ShopBrandActorType.超级管理员);

            var query = db.Query<ShopBrandCommoditySkuItem>()
                     .Where(m => !m.IsDel)
                     .Where(m => !m.ParameterValue.IsDel);

            var list = query
                .Where(m => m.Sku.CommodityId == args.CommodityId.Value)
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
    }
}
