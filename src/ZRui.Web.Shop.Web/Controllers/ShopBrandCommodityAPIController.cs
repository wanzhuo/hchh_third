using System;
using System.Linq;
using ZRui.Web.ShopCommodityAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopBrandCommodityAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommodityAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// 获取指定商铺的商铺列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetShopCategoryAndCommodities([FromBody]GetShopCategoryAndCommoditiesArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            var rtn = await Task.Run(() =>
            {
                ShopBrandCommodityServer server = new ShopBrandCommodityServer(db, args.ShopFlag, args.DiningWay);
                return server.GetCategoryAndCommodity();
            });
            return Success(rtn);
        }


        /// <summary>
        /// 获取指定商铺的商铺列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> GetShopCommoditiesAndCombos([FromBody]GetShopCategoryAndCommoditiesArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            var res = await Task.Run(() =>
            {
                ShopBrandCommodityServer server = new ShopBrandCommodityServer(db, args.ShopFlag, args.DiningWay);
                GetShopCommoditiesAndCombos rtn = new GetShopCommoditiesAndCombos()
                {
                    CategoryAndCommodity = server.GetCategoryAndCommodity(),
                    Commodity = server.GetComboList()
                };
                return rtn;
            });
            return Success(res);
        }


        /// <summary>
        /// 获取指定商铺的商铺列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetCommoditiesForMember([FromBody]GetShopCategoryAndCommoditiesArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            var res = await Task.Run(() =>
            {
                var shop = db.Query<Shop>()
                .Where(m => !m.IsDel)
                .Where(m => m.Flag == args.ShopFlag)
                .FirstOrDefault();
                var memberId = GetMemberId();
                var shopMember = ShopMemberServer.GetShopMember(db, shop.Id, memberId);
                ShopMemberLevel shopMemberLevel = null;
                if (shopMember != null)
                {
                    shopMemberLevel = db.GetSingle<ShopMemberLevel>(shopMember.ShopMemberLevelId);
                }
                ShopBrandCommodityServer server = new ShopBrandCommodityServer(db, shop, args.DiningWay);
                GetShopCommoditiesAndCombos rtn = new GetShopCommoditiesAndCombos()
                {
                    CategoryAndCommodity = server.GetCategoryAndCommodity(shopMemberLevel?.Discount),
                    Commodity = server.GetComboList()
                };
                return rtn;
            });
            return Success(res);
        }



        /// <summary>
        /// 获取指定商铺的商铺列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            var query = db.Query<ShopCommodityStock>()
                 .Where(m => !m.IsDel)
                 .Where(m => m.Shop.Flag == args.ShopFlag);

            if (!string.IsNullOrEmpty(args.CategoryFlag))
            {
                query = query.Where(m => m.Sku.Commodity.Category.Flag == args.CategoryFlag);
            }

            var items = query
                 .Select(m => new
                 {
                     Name = m.Sku.Commodity.Name,
                     CommodityId = m.Sku.CommodityId,
                     SalesForMonth = m.Sku.Commodity.SalesForMonth,
                     Upvote = m.Sku.Commodity.Upvote,
                     Cover = m.Sku.Commodity.Cover,
                     SalePrice = m.SalePrice,
                     MarketPrice = m.MarketPrice,
                     Detail = m.Sku.Commodity.Detail
                 })
                 .GroupBy(m => m.CommodityId)
                 .Select(m => new CommodityRowitem
                 {
                     Name = m.First().Name,
                     SalesForMonth = m.First().SalesForMonth,
                     Upvote = m.First().Upvote,
                     SalePrice = m.First().SalePrice,
                     Cover = m.First().Cover,
                     MarketPrice = m.First().MarketPrice,
                     Detail = m.First().Detail,
                 })
                 .ToList();

            return Success(new GetListModel()
            {
                Items = items
            });
        }

        /// <summary>
        /// 获取指定商铺的商品列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<GetCommoditysModel> GetCommoditys([FromBody]GetListArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");
            bool isFb = args.CategoryFlag == "fb"; //火爆分类
            var query = db.Query<ShopCommodityStock>()
                 .Where(m => !m.IsDel)
                 .Where(m => m.Shop.Flag == args.ShopFlag);
            if(isFb)   //火爆分类
            {
                query = query.Where(m => m.Sku.Commodity.CategoryId != 0);
            }
            else if (!string.IsNullOrEmpty(args.CategoryFlag))
            {
                query = query.Where(m => m.Sku.Commodity.Category.Flag == args.CategoryFlag);
            }
            //这里获得指定店铺的库存及价格及关联的Sku
            var stockItems = query
                 .Select(m => new StockRowItem
                 {
                     SkuId = m.Sku.Id,
                     SkuFlag = m.Sku.Flag,
                     CommodityId = m.Sku.CommodityId,
                     SalePrice = m.SalePrice,
                     //MarketPrice = m.MarketPrice,
                     Stock = m.Stock
                 })
                 .ToList();

            var viewModel = new GetCommoditysModel();
            //获取库存关联的商铺信息，主要是一些描述
            var commodityIds = stockItems.Select(m => m.CommodityId).Distinct().ToList();
            var commodities = db.Query<ShopBrandCommodity>().Where(m => !m.IsDel)
                .Where(m => commodityIds.Contains(m.Id));
            if (args.DiningWay.HasValue)
                commodities = commodities.Where(m => m.DiningWay == DiningWay.所有 || m.DiningWay == args.DiningWay);
            if(isFb)
                commodities = commodities.OrderByDescending(m => m.SalesForMonth).Take(10);
            viewModel.Items = commodities.Select(m => new CommodityRowItem()
            {
                Id = m.Id,
                Cover = m.Cover,
                Detail = m.Detail,
                Name = m.Name,
                SalesForMonth = m.SalesForMonth,
                Upvote = m.Upvote
            }).ToList();
            //获取库存关联的规格的规格项
            var skuIds = stockItems.Select(m => m.SkuId).Distinct().ToList();
            var skuItems = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .OrderByDescending(m => m.Id)
                .Select(m => new SkuItem()
                {
                    Id = m.Id,
                    SkuId = m.SkuId,
                    Value = m.ParameterValue.Value,
                    ParameterValueId = m.ParameterValueId,
                    ParameterId = m.ParameterId,
                    ParameterName = m.Parameter.Name
                })
                .ToList();

            //循环每一个商品
            
            foreach (var item in viewModel.Items)
            {
                //获得商品的sku id列表，用于下面获取商品的skuItems
                var commoditySkuIds = stockItems.Where(m => m.CommodityId == item.Id).Select(m => m.SkuId).Distinct().ToList();
                //
                var stock = stockItems.Where(m => m.CommodityId == item.Id).First();
                item.Skus = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId)).GroupBy(m => m.SkuId).Select(m => new CommoditySku(m.Key, m.ToList(), stockItems.Where(x => x.CommodityId == item.Id && x.SkuId == m.Key).FirstOrDefault())).ToList();

                item.Parameters = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId)).GroupBy(m => m.ParameterId).Select(m => new CommodityParameter(m.ToList())).ToList();
            }

            return Success(viewModel);
        }
        /// <summary>
        /// 获取指定商铺的商铺类别树
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<GetCategoryTreeModel> GetCategoryTree([FromBody]GetCategoryTreeArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ShopFlag)) throw new Exception("ShopFlag不能为空");

            var shopBrandId = db.Query<Shop>()
                .Where(m => !m.IsDel)
                .Where(m => m.Flag == args.ShopFlag)
                .Select(m => m.ShopBrandId)
                .FirstOrDefault();
            if (shopBrandId <= 0) throw new Exception("纪录不存在");
            var query = db.Query<ShopBrandCommodityCategory>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == shopBrandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new CategoryItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    PId = m.PId,
                    Flag = m.Flag,
                    Ico = m.Ico,
                })
                .ToList();

            return Success(new GetCategoryTreeModel(list));
        }
        /// <summary>
        /// 通过商品标识获得商品的Sku信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetSkuItems([FromBody]GetSkuItemsArgsModel args)
        {
            var query = db.Query<ShopBrandCommoditySkuItem>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.Sku.Commodity.Flag == args.CommodityFlag)
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
