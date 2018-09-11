using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBrandCommodityParameterSetAPIModels;
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
    public class ShopBrandCommodityParameterSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommodityParameterSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
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

            var query = db.Query<ShopBrandCommodityParameter>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == args.ShopBrandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Flag = m.Flag,
                    ShopBrandId = m.ShopBrandId
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

            if (!args.ShopBrandId.HasValue) throw new ArgumentNullException("ShopBrandId");
            var brandId = args.ShopBrandId.Value;
            CheckShopBrandActor(brandId, ShopBrandActorType.超级管理员);

            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandCommodityParameter>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == args.ShopBrandId);

            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Flag = m.Flag,
                    ShopBrandId = m.ShopBrandId
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

            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();

            var shopBrand = db.GetSingle<ShopBrand>(brandId);
            if (shopBrand == null) throw new Exception("店铺品牌纪录不存在");

            var model = new ShopBrandCommodityParameter()
            {
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Flag = args.Flag,
                Name = args.Name,
                ShopBrand = shopBrand
            };

            db.Add<ShopBrandCommodityParameter>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();
            var model = db.Query<ShopBrandCommodityParameter>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            model.Name = args.Name;

            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var model = db.Query<ShopBrandCommodityParameter>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Flag = m.Flag,
                    ShopBrandId = m.ShopBrandId
                })
                .FirstOrDefault();
            if (model == null) throw new Exception("记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            return Success(model);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopBrandCommodityParameter>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);

            var records = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => m.ParameterId == model.Id)
                .Count();

            if (records > 0)
            {
                throw new Exception("不能删除此规格,尚有使用它的商品");
            }
            else
            {
                model.IsDel = true;
                db.SaveChanges();
            }

            return Success();
        }
    }
}
