using System;
using System.Linq;
using ZRui.Web.CommercialDistrictShopSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CommercialDistrictShopSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public CommercialDistrictShopSetAPIController(ICommunityService communityService
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
            var query = db.Query<CommercialDistrictShop>()
                     .Where(m => !m.IsDel);
            if (args.CommercialDistrictId.HasValue)
            {
                query = query.Where(m => m.CommercialDistrictId == args.CommercialDistrictId);
            }
            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    ShopName = m.Shop.Name
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
            var query = db.Query<CommercialDistrictShop>()
                     .Where(m => !m.IsDel);
            if (args.CommercialDistrictId.HasValue)
            {
                query = query.Where(m => m.CommercialDistrictId == args.CommercialDistrictId);
            }
            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    ShopName = m.Shop.Name
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
            var commercialDistrict = db.GetSingle<CommercialDistrict>(args.CommercialDistrictId);
            if (commercialDistrict == null) throw new Exception("商圈纪录不存在");

            var shop = db.GetSingle<Shop>(args.ShopId);
            if (shop == null) throw new Exception("店铺纪录不存在");

            var isExit = db.Query<CommercialDistrictShop>()
                .Where(m => !m.IsDel)
                .Where(m => m.CommercialDistrictId == args.CommercialDistrictId && m.ShopId == args.ShopId)
                .Count() > 0;
            if (isExit) throw new Exception("商圈的店铺纪录已经存在");

            var model = new CommercialDistrictShop()
            {
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                CommercialDistrict = commercialDistrict,
                Shop = shop
            };

            db.Add<CommercialDistrictShop>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<CommercialDistrictShop>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult GetCommercialDistricts([FromBody]CommunityArgsModel args)
        {
            var query = db.Query<CommercialDistrict>()
                      .Where(m => !m.IsDel);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new GetCommercialDistrictsItem()
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToList();

            return Success(new GetCommercialDistrictsModel()
            {
                Items = list
            });
        }
    }
}
