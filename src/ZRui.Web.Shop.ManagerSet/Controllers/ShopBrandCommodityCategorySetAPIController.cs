using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBrandCommodityCategorySetAPIModels;
using ZRui.Web.Common;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZRui.Web.Controllers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopBrandCommodityCategorySetAPIController : ShopManagerApiControllerBase
    {
        public ShopBrandCommodityCategorySetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db)
            : base(options, db, memberDb)
        {
        }

        [HttpPost]
        [Authorize]
        public APIResult GetTree([FromBody]GetTreeArgsModel args)
        {
            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(args.ShopBrandId.Value, ShopBrandActorType.超级管理员);

            var query = db.Query<ShopBrandCommodityCategory>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == brandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    Detail = m.Detail,
                    PId = m.PId,
                    ParentName = m.Parent.Name,
                    Flag = m.Flag,
                    IsDel = m.IsDel,
                    Ico = m.Ico,
                    Keywords = m.Keywords,
                    Description = m.Description,
                    Tags = m.Tags
                })
                .ToList();

            return Success(new GetTreeModel(list));
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            var query = db.Query<ShopBrandCommodityCategory>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == brandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    Detail = m.Detail,
                    ParentName = m.Parent.Name,
                    PId = m.PId,
                    Flag = m.Flag,
                    Ico = m.Ico,
                    IsDel = m.IsDel,
                    Keywords = m.Keywords,
                    Description = m.Description,
                })
                .ToList();

            list.Add(new RowItem()
            {
                Name = "套餐",
                Id = 0
            });

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandCommodityCategory>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == brandId);

            switch (args.OrderName.ToLower())
            {
                case "name":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Name);
                    else
                        query = query.OrderByDescending(m => m.Name);
                    break;
                case "orderWeight":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.OrderWeight);
                    else
                        query = query.OrderByDescending(m => m.OrderWeight);
                    break;
                case "detail":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Detail);
                    else
                        query = query.OrderByDescending(m => m.Detail);
                    break;
                case "pId":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.PId);
                    else
                        query = query.OrderByDescending(m => m.PId);
                    break;
                case "flag":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Flag);
                    else
                        query = query.OrderByDescending(m => m.Flag);
                    break;
                case "isDel":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.IsDel);
                    else
                        query = query.OrderByDescending(m => m.IsDel);
                    break;
                case "keywords":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Keywords);
                    else
                        query = query.OrderByDescending(m => m.Keywords);
                    break;
                case "description":
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Description);
                    else
                        query = query.OrderByDescending(m => m.Description);
                    break;
                default:
                    if (args.OrderType == "asc")
                        query = query.OrderBy(m => m.Id);
                    else
                        query = query.OrderByDescending(m => m.Id);
                    break;
            }

            var list = query
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    Detail = m.Detail,
                    ParentName = m.Parent.Name,
                    PId = m.PId,
                    Ico = m.Ico,
                    Flag = m.Flag,
                    IsDel = m.IsDel,
                    Keywords = m.Keywords,
                    Description = m.Description,
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.ToList()
            });

        }


        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("name");

            //如果没有传，则自动生成
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();

            var shopBrand = db.GetSingle<ShopBrand>(brandId);
            if (shopBrand == null) throw new Exception("店铺品牌纪录不存在");

            var model = new ShopBrandCommodityCategory()
            {
                Name = args.Name,
                OrderWeight = args.OrderWeight,
                Detail = args.Detail,
                PId = args.PId,
                Flag = args.Flag,
                Ico = args.Ico,
                IsDel = false,
                Keywords = args.Keywords,
                Description = args.Description,
                ShopBrand = shopBrand
            };
            db.Add<ShopBrandCommodityCategory>(model);
            db.SaveChanges();
            return Success<int>(model.Id);
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var model = db.Query<ShopBrandCommodityCategory>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            if (model.Id == args.PId) throw new Exception("自己的父类不能是自己");
            if (model.PId != args.PId && args.PId.HasValue)
            {//说明父类有变更,并且不是在根下，那么需要判定一下旧的类别是否是当前新的类别的父类
                List<int> ids = new List<int>();
                Action<int> getChildrenIds = null;
                getChildrenIds = id =>
                {
                    var chidrenIds = db.Query<ShopBrandCommodityCategory>().Where(m => m.PId == id).Select(m => m.Id).ToList();
                    ids.AddRange(chidrenIds);
                    foreach (var item in chidrenIds)
                    {
                        getChildrenIds(item);
                    }
                };
                getChildrenIds(model.Id);
                if (ids.Contains(args.PId.Value)) throw new Exception("不能放在自己的子目录下面");
            }
            model.PId = args.PId;
            model.Name = args.Name;
            model.OrderWeight = args.OrderWeight;
            model.Detail = args.Detail;
            //有值才修改，没有值不修改
            if (!string.IsNullOrEmpty(model.Flag))
            {
                model.Flag = args.Flag;
            }
            model.Ico = args.Ico;
            model.Tags = args.Tags;
            model.Keywords = args.Keywords;
            model.Description = args.Description;
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var viewModel = db.Query<ShopBrandCommodityCategory>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    Detail = m.Detail,
                    PId = m.PId,
                    Flag = m.Flag,
                    Ico = m.Ico,
                    Tags = m.Tags,
                    Keywords = m.Keywords,
                    Description = m.Description,
                    ShopBrandId = m.ShopBrandId
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(viewModel.ShopBrandId, ShopBrandActorType.超级管理员);

            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.Query<ShopBrandCommodityCategory>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("记录不存在");
            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            if (db.Query<ShopBrandCommodityCategory>().Where(m => m.PId == model.Id && !m.IsDel).Count() > 0) throw new Exception("存在下级分类不能删除");
            if (db.Query<ShopBrandCommodity>().Where(m => m.CategoryId == model.Id && !m.IsDel).Count() > 0) throw new Exception("存在商品不能删除");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
