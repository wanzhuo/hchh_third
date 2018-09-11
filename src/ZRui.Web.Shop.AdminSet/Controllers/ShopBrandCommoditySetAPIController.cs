using System;
using System.Linq;
using ZRui.Web.ShopBrandCommoditySetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopBrandCommoditySetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommoditySetAPIController(ICommunityService communityService
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
            var query = db.Query<ShopBrandCommodity>()
                     .Where(m => !m.IsDel);
            if (args.ShopBrandId.HasValue)
            {
                query = query.Where(m => m.ShopBrandId == args.ShopBrandId);
            }
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

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandCommodity>()
                     .Where(m => !m.IsDel);
            if (args.ShopBrandId.HasValue)
            {
                query = query.Where(m => m.ShopBrandId == args.ShopBrandId);
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
                    Unit = m.Unit,
                    Upvote = m.Upvote,
                    CategoryName = m.Category.Name,
                    CategoryId = m.CategoryId
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
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();

            var shopBrand = db.GetSingle<ShopBrand>(args.ShopBrandId);
            if (shopBrand == null) throw new Exception("店铺品牌纪录不存在");

            var category = db.GetSingle<ShopBrandCommodityCategory>(args.CategoryId);
            if (category == null) throw new Exception("店铺商品类别不存在");

            var model = new ShopBrandCommodity()
            {
                Flag = args.Flag,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Detail = args.Detail,
                Name = args.Name,
                Cover = args.Cover,
                IsRecommand = args.IsRecommand,
                Price = args.Price,
                SalesForMonth = args.SalesForMonth,
                ShopBrand = shopBrand,
                Summary = args.Summary,
                Unit = args.Unit,
                Upvote = args.Upvote,
                Category = category
            };

            db.Add<ShopBrandCommodity>(model);
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
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            var category = db.GetSingle<ShopBrandCommodityCategory>(args.CategoryId);
            if (category == null) throw new Exception("店铺商品类别不存在");

            model.Category = category;
            model.Detail = args.Detail;
            model.Name = args.Name;
            model.Cover = args.Cover;
            model.IsRecommand = args.IsRecommand;
            model.Price = args.Price;
            model.SalesForMonth = args.SalesForMonth;
            model.Summary = args.Summary;
            model.Unit = args.Unit;
            model.Upvote = args.Upvote;

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
                    SalesForMonth = m.SalesForMonth,
                    ShopBrandId = m.ShopBrandId,
                    Summary = m.Summary,
                    Unit = m.Unit,
                    Upvote = m.Upvote
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopBrandCommodity>(args.Id);
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
                .Select(m => new GetShopBrandsItem()
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
    }
}
