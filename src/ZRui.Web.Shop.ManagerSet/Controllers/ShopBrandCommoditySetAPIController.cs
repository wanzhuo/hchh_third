using ZRui.Web.ShopManager.ShopBrandCommoditySetAPIModels;
using ZRui.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using ZRui.Web.Common;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopBrandCommoditySetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommoditySetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            var query = db.Query<ShopBrandCommodity>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == args.ShopBrandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Cover = m.Cover,
                    IsRecommand = m.IsRecommand,
                    Price = m.Price,
                    SalesForMonth = m.SalesForMonth,
                    ShopBrandId = m.ShopBrandId,
                    Summary = m.Summary,
                    Unit = m.Unit,
                    Upvote = m.Upvote,
                    CategoryName = m.Category.Name,
                    CategoryId = m.CategoryId
                })                      
                .ToList();

            return Success(new ShopBrandCommoditySetAPIModels.GetListModel()
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

            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandCommodity>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == args.ShopBrandId);

            //商品名称搜索
            if(!string.IsNullOrEmpty(args.SearchName))
            {
                query = query.Where(m => m.Name.Contains(args.SearchName));
            }
            if (query.Count()<10) {
                args.PageIndex = 1;
            }
            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Cover = m.Cover,
                    IsRecommand = m.IsRecommand,
                    Price = m.Price,
                    SalesForMonth = m.SalesForMonth,
                    ShopBrandId = m.ShopBrandId,
                    Summary = m.Summary,
                    IsScanCode = m.IsScanCode,
                    IsSelfOrder = m.IsSelfOrder,
                    IsTakeout = m.IsTakeout,
                    UseMemberPrice = m.UseMemberPrice,
                    Unit = m.Unit,
                    Upvote = m.Upvote,
                    DiningWay = m.DiningWay,
                    CategoryId = m.CategoryId
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            var resultList = list.ToList();
            foreach (var item in resultList)
            {
                if (item.CategoryId == 0)
                {
                    item.CategoryName = "套餐";
                    item.CommodityIds = db.Query<ShopOrderComboItem>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.Pid == item.Id)
                        .Select(m => m.CommodityId.ToString())
                        .Distinct()
                        .ToList();
                }
                else
                {
                    item.CategoryName = db.GetSingle<ShopBrandCommodityCategory>(item.CategoryId)?.Name;
                }
            }

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = resultList
            });
        }


        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();

            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            var shopBrand = db.GetSingle<ShopBrand>(brandId);
            if (shopBrand == null) throw new Exception("店铺品牌纪录不存在");

            ShopBrandCommodityCategory category;
            if (args.CategoryId == 0)
            {
                category = null;
            }
            else
            {
                category = db.GetSingle<ShopBrandCommodityCategory>(args.CategoryId);
                if (category == null) throw new Exception("店铺商品类别不存在");
            }

            var model = new ShopBrandCommodity()
            {
                Flag = args.Flag,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Detail = args.Detail,
                DiningWay = args.DiningWay,
                Name = args.Name,
                Cover = args.Cover,
                IsRecommand = args.IsRecommand,
                Price = args.Price,
                SalesForMonth = args.SalesForMonth,
                ShopBrand = shopBrand,
                Summary = args.Summary,
                Unit = args.Unit,
                Upvote = args.Upvote,
                IsSelfOrder = args.IsSelfOrder,
                IsScanCode = args.IsScanCode,
                IsTakeout = args.IsTakeout,
                Category = category,
                UseMemberPrice = args.UseMemberPrice
            };

            db.Add<ShopBrandCommodity>(model);

            if (args.CategoryId == 0) //套餐
            {
                foreach (var commodityId in args.CommodityIds)
                {
                    ShopOrderComboItem comboItem = new ShopOrderComboItem()
                    {
                        ParentCommodity = model,
                        CommodityId = commodityId
                    };
                    db.Add(comboItem);
                }
            }

            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();
            var model = db.Query<ShopBrandCommodity>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            ShopBrandCommodityCategory category;
            if (args.CategoryId == 0)
            {
                category = null;
            }
            else
            {
                category = db.GetSingle<ShopBrandCommodityCategory>(args.CategoryId);
                if (category == null) throw new Exception("店铺商品类别不存在");
            }

            model.Category = category;
            model.Detail = args.Detail;
            model.Name = args.Name;
            model.Cover = args.Cover;
            model.IsRecommand = args.IsRecommand;
            model.Price = args.Price;
            model.SalesForMonth = args.SalesForMonth;
            model.Summary = args.Summary;
            model.Unit = args.Unit;
            model.IsTakeout = args.IsTakeout;
            model.IsScanCode = args.IsScanCode;
            model.IsSelfOrder = args.IsSelfOrder;
            model.Upvote = args.Upvote;
            model.UseMemberPrice = args.UseMemberPrice;
            //model.DiningWay = args.DiningWay;

            if (args.CategoryId == 0) //套餐
            {
                foreach (var commodityId in args.CommodityIds)
                {
                    db.Query<ShopOrderComboItem>()
                        .Where(m => !m.IsDel && m.Pid == model.Id)
                        .ToList()
                        .ForEach(m => m.IsDel = true);
                    ShopOrderComboItem comboItem = new ShopOrderComboItem()
                    {
                        ParentCommodity = model,
                        CommodityId = commodityId
                    };
                    db.Add(comboItem);
                }
            }

            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var viewModel = db.Query<ShopBrandCommodity>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Cover = m.Cover,
                    IsRecommand = m.IsRecommand,
                    Price = m.Price,
                    IsTakeout = m.IsTakeout,
                    IsScanCode = m.IsScanCode,
                    IsSelfOrder = m.IsSelfOrder,
                    SalesForMonth = m.SalesForMonth,
                    ShopBrandId = m.ShopBrandId,
                    Summary = m.Summary,
                    Unit = m.Unit,
                    Upvote = m.Upvote
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
            var model = db.GetSingle<ShopBrandCommodity>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            model.IsDel = true;

            var skus = db.Query<ShopBrandCommoditySku>()
                .Where(m => !m.IsDel)
                .Where(m => m.CommodityId == model.Id);

            var skuIds = skus.Select(m => m.Id);

            db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .ToList()
                .ForEach(m => m.IsDel = true);

            skus.ToList().ForEach(m => m.IsDel = true);

            db.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public APIResult AddCombo([FromBody] AddComboArgsModel args)
        {
            if (args.ShopId == 0) throw new Exception("ShopId不能为空");
            Shop shop = db.GetSingle<Shop>(args.ShopId);
            CheckShopBrandActor(shop.ShopBrandId, ShopBrandActorType.超级管理员);

            ShopBrandCommodity combo = new ShopBrandCommodity()
            {
                AddIp = GetIp(),
                SalesForMonth = 0,
                ShopBrandId = shop.ShopBrandId,
                AddTime = DateTime.Now,
                IsSelfOrder = args.IsSelfOrder,
                IsScanCode = args.IsScanCode,
                IsTakeout = args.IsTakeout,
                Name = args.Name,
                Price = args.Price,
                Unit = args.Unit,
                CategoryId = 0,
                Cover = args.Cover,
                Flag = Guid.NewGuid().ToString()
            };
            db.Add(combo);
            db.SaveChanges();
            foreach (var item in args.Items)
            {
                ShopOrderComboItem comboItem = new ShopOrderComboItem()
                {
                    Pid = combo.Id,
                    CommodityName = item.CommodityName,
                    Count = item.Count,
                    SalePrice = item.Count * item.SalePrice,
                    Sku = item.Sku
                };
                db.Add(comboItem);
            }
            db.SaveChanges();
            return Success();
        }

        /// <summary>
        /// 获取套餐信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public APIResult GetComboList([FromBody] IdArgsModel args)
        {
            var shop = db.GetSingle<Shop>(args.Id);
            if (shop == null) throw new Exception("商铺记录不存在");
            CheckShopBrandActor(shop.ShopBrandId, ShopBrandActorType.超级管理员);

            var comboList = db.Query<ShopBrandCommodity>()
                .Where(m => !m.IsDel && m.ShopBrandId == shop.ShopBrandId)
                .Select(m => new GetComboListDto()
                {
                    Name = m.Name,
                    Price = m.Price,
                    Unit = m.Unit
                }).ToList();

            return Success(comboList);
        }


        /// <summary>
        /// 获取套餐信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public APIResult GetComboSingle([FromBody] IdArgsModel args)
        {
            ShopBrandCommodity combo = db.GetSingle<ShopBrandCommodity>(args.Id);
            if (combo == null || combo.CategoryId != 0) throw new Exception("套餐记录不存在");

            CheckShopBrandActor(combo.ShopBrandId, ShopBrandActorType.超级管理员);

            GetComboSingleDto rtn = new GetComboSingleDto()
            {
                Unit = combo.Unit,
                Price = combo.Price,
                Name = combo.Name,
                Cover = combo.Cover,
                IsScanCode = combo.IsScanCode,
                IsSelfOrder = combo.IsSelfOrder,
                IsTakeout = combo.IsTakeout
            };

            rtn.Items = db.Query<ShopOrderComboItem>()
                .Where(m => !m.IsDel && m.Pid == combo.Id)
                .Select(m => new ComboItem()
                {
                    CommodityName = m.CommodityName,
                    SalePrice = m.SalePrice,
                    Count = m.Count,
                    Sku = m.Sku
                }).ToList();

            return Success(rtn);
        }

        


    }
}
