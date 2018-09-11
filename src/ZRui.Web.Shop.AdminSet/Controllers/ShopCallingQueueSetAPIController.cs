using System;
using System.Linq;
using ZRui.Web.ShopCallingQueueSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCallingQueueSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCallingQueueSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
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
            var query = db.Query<ShopCallingQueue>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddTime = m.AddTime,
                    CanShareTable = m.CanShareTable,
                    ProductId = m.ProductId,
                    QueueIndex = m.QueueIndex,
                    QueueNumber = m.QueueNumber,
                    RefuseReason = m.RefuseReason,
                    Remark = m.Remark,
                    Status = m.Status,
                    Title = m.Title,
                    Id = m.Id,
                    MemberId = m.MemberId,
                    ShopId = m.ShopId,
                    IsUsed = m.IsUsed
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult SetStatus([FromBody]SetStatusArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            model.Status = args.Status;
            model.RefuseReason = args.RefuseReason;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsUsed([FromBody]SetIsUsedArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            model.IsUsed = args.IsUsed;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetQueueIndex([FromBody]SetQueueIndexArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            model.QueueIndex = args.QueueIndex;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
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
    }
}
