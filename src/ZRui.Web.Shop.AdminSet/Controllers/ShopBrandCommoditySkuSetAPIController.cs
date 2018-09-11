using System;
using System.Linq;
using ZRui.Web.ShopBrandCommoditySkuSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopBrandCommoditySkuSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommoditySkuSetAPIController(ICommunityService communityService
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
            var skuIds = db.Query<ShopBrandCommoditySku>()
                .Where(m => !m.IsDel)
                .Where(m => m.CommodityId == args.CommodityId)
                .Select(m => m.Id)
                .ToList();

            var list = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
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
            var commodity = db.GetSingle<ShopBrandCommodity>(args.CommodityId);
            if (commodity == null) throw new Exception("商品纪录不存在");

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

            var oldParameterIds = skuItems.Select(m => m.ParameterId).ToList();
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

            var parameterIds = args.ParameterIds;

            var paramterCount = db.Query<ShopBrandCommodityParameter>()
                .Where(m => parameterIds.Contains(m.Id))
                .Count();
            if (parameterIds.Count != paramterCount) throw new Exception("参数有误，请刷新重试");

            var paramterValues = db.Query<ShopBrandCommodityParameterValue>()
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
                    Flag = string.Join('_', newSkuItems),
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

        [HttpPost]
        [Authorize]
        public APIResult GetParameters([FromBody]GetParametersArgsModel args)
        {
            var query = db.Query<ShopBrandCommodityParameter>()
                     .Where(m => !m.IsDel);
            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new GetParameterItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                })
                .ToList();

            return Success(list);
        }
    }
}
