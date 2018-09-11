using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBrandCommoditySkuSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Controllers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopBrandCommoditySkuSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommoditySkuSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        private void CheckShopBrandActorByCommodityId(int commodityId, params ShopBrandActorType[] actorTypes)
        {
            var parameter = db.Query<ShopBrandCommodity>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == commodityId)
                .Select(m => new
                {
                    Id = m.Id,
                    ShopBrandId = m.ShopBrandId
                })
                .FirstOrDefault();
            if (parameter == null) throw new Exception("关联的品牌纪录不存在");
            //获取到参数关联的品牌才能进行判断
            CheckShopBrandActor(parameter.ShopBrandId, actorTypes);
        }


        [HttpPost]
        [Authorize]
        public APIResult<GetListModel> GetList([FromBody]GetListArgsModel args)
        {
            if (!args.CommodityId.HasValue) throw new ArgumentNullException("CommodityId");
            CheckShopBrandActorByCommodityId(args.CommodityId.Value, ShopBrandActorType.超级管理员);

            var skuIds = db.Query<ShopBrandCommoditySku>()
                .Where(m => !m.IsDel)
                .Where(m => m.CommodityId == args.CommodityId.Value)
                .Select(m => m.Id)
                .ToList();

            var list = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => !m.ParameterValue.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    ParameterName = m.Parameter.Name,
                    ParameterId = m.ParameterId,
                    ParameterValue = m.ParameterValue.Value,
                    ParameterValueId = m.ParameterValueId
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var commodity = db.GetSingle<ShopBrandCommodity>(args.CommodityId.Value);
            if (commodity == null) throw new Exception("商品纪录不存在");
            var parameterIds = args.ParameterIds;
            //套餐商品只能添加一个规格
            if (commodity.CategoryId == 0)
            {
                if (args.ParameterIds.Count > 1)
                    throw new Exception("套餐只能添加单个规格");
                if (args.ParameterIds.Count != 0)
                {
                    var pvListCount = db.Query<ShopBrandCommodityParameterValue>()
                    .Where(m => !m.IsDel)
                    .Where(m => parameterIds.Contains(m.ParameterId))
                    .Count();
                    if (pvListCount != 1) throw new Exception("套餐只能添加单个规格");
                }
            }

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(commodity.ShopBrandId, ShopBrandActorType.超级管理员);

            //获取到旧的skuParameter
            var skus = db.Query<ShopBrandCommoditySku>()
               .Where(m => !m.IsDel)
               .Where(m => m.CommodityId == args.CommodityId)
               .ToList();
            //删掉原来的
            foreach (var sku in skus)
            {
                sku.IsDel = true;
            }
            var skuIds = skus.Select(m => m.Id).ToList();

            //删掉原来的
            var skuItems = db.Query<ShopBrandCommoditySkuItem>()
                     .Where(m => !m.IsDel)
                     .Where(m => skuIds.Contains(m.SkuId))
                     .ToList();
            foreach (var skuItem in skuItems)
            {
                skuItem.IsDel = true;
            }

            var oldParameterIds = skuItems.Select(m => m.ParameterId).Distinct().ToList();
            //判定新的跟旧的是否一样，如果一样，则不需要修改
            //判定的方法是获得并集，如果两个集的长度相等并且合并后长度还是等于新的长度，则表示是一样的
            if (oldParameterIds.Count == args.ParameterIds.Count && oldParameterIds.Union(args.ParameterIds).Count() == args.ParameterIds.Count)
            {
                throw new Exception("当前数据跟提交数据一样，不需要修改");
            }
            //需要判定一下是否sku已经在使用，如果在使用，则不能删除
            var usedSkuCount = db.Query<ShopCommodityStock>()
                .Where(m => !m.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .Count();
            if (usedSkuCount > 0) throw new Exception("原规格还在使用中，不能修改");


            var paramterCount = db.Query<ShopBrandCommodityParameter>()
                .Where(m => !m.IsDel)
                .Where(m => parameterIds.Contains(m.Id))
                .Count();
            if (parameterIds.Count != paramterCount) throw new Exception("参数有误，请刷新重试");

            var paramterValues = db.Query<ShopBrandCommodityParameterValue>()
                .Where(m => !m.IsDel)
                .Where(m => parameterIds.Contains(m.ParameterId))
                .Select(m => new
                {
                    ParameterId = m.ParameterId,
                    Id = m.Id,
                    ParameterName = m.Parameter.Name,
                    ParameterValue = m.Value
                })
                .OrderBy(m => m.Id)
                .ToList();

            //得到新的sku列表
            var newSkus = new List<List<int>>();
            newSkus.Add(new List<int> { commodity.Id });
            foreach (var parameterId in parameterIds)
            {
                var values = paramterValues.Where(m => m.ParameterId == parameterId).Select(m => m.Id).ToList();
                newSkus = CombineArray(newSkus, values);
            }

            foreach (var newSkuItems in newSkus)
            {
                var sku = new ShopBrandCommoditySku()
                {
                    CommodityId = commodity.Id,
                    Flag = string.Join('_', newSkuItems.AsQueryable().OrderBy(m => m)),
                    Summary = string.Empty
                };

                db.Add<ShopBrandCommoditySku>(sku);

                for (int i = 1; i < newSkuItems.Count; i++)
                {//0位为parameterId，这里从1开始
                    var parameterValueId = newSkuItems[i];
                    var paramValue = paramterValues.Where(m => m.Id == parameterValueId).First();
                    db.Add(new ShopBrandCommoditySkuItem()
                    {
                        ParameterId = paramValue.ParameterId,
                        ParameterValueId = parameterValueId,
                        Sku = sku
                    });
                    sku.Summary += $"{paramValue.ParameterName}:{paramValue.ParameterValue},";
                }

                sku.Summary = sku.Summary.TrimEnd(',');
            }
            db.SaveChanges();

            return Success();
        }

        private List<List<int>> CombineArray(List<List<int>> targets, IList<int> newArray)
        {
            var results = new List<List<int>>();
            foreach (var item in targets)
            {
                foreach (var newItem in newArray)
                {
                    var abc = new List<int>(item);
                    abc.Add(newItem);
                    results.Add(abc);
                }
            }

            return results;
        }
    }
}
