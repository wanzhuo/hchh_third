using System;
using System.Linq;
using ZRui.Web.ShopCallingQueueProductSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCallingQueueProductSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCallingQueueProductSetAPIController(ICommunityService communityService
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
            var query = db.Query<ShopCallingQueueProduct>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Detail = m.Detail,
                    Status = m.Status,
                    Title = m.Title,
                    Id = m.Id,
                    ShopId = m.ShopId
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
            //获取并验证店铺是否存在
            var shop = db.GetSingle<Shop>(args.ShopId);
            if (shop == null) throw new Exception("指定的商铺不存在");

            //这里只是添加一个库存纪录，库存的参数在编辑处修改
            var model = new ShopCallingQueueProduct()
            {
                Shop = shop,
                Status = ShopCallingQueueProductStatus.正常,
                Title = args.Title,
                Detail = args.Detail
            };
            db.Add<ShopCallingQueueProduct>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var model = db.Query<ShopCallingQueueProduct>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            model.Detail = args.Detail;
            model.Title = args.Title;
            model.Status = args.Status;

            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopCallingQueueProduct>(args.Id);
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

        [HttpPost]
        [Authorize]
        public APIResult GetShopOpenStatus([FromBody]IdArgsModel args)
        {
            var flag = ShopCallingQueue.GetShopOpenStatusFlag(args.Id);
            var v = db.GetSettingValue<bool>(flag);
            return Success(v);
        }

        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> SetShopOpenStatus([FromBody]SetShopOpenStatusArgsModel args)
        {
            var flag = ShopCallingQueue.GetShopOpenStatusFlag(args.Id);
            db.SetSettingValue(flag, args.IsOpen.ToString());
            await db.SaveChangesAsync();
            return Success();
        }
    }
}
