using System;
using System.Linq;
using ZRui.Web.CommercialDistrictSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CommercialDistrictSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public CommercialDistrictSetAPIController(ICommunityService communityService
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
            var query = db.Query<CommercialDistrict>()
                     .Where(m => !m.IsDel);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Address = m.Address,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Status = m.Status,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
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
            var query = db.Query<CommercialDistrict>()
                     .Where(m => !m.IsDel);

            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Address = m.Address,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Status = m.Status,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
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
            if (!args.Status.HasValue) args.Status = CommercialDistrictStatus.正常;

            var model = new CommercialDistrict()
            {
                Flag = args.Flag,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Address = args.Address,
                Name = args.Name,
                Status = args.Status.Value,
                Detail = args.Detail,
                Longitude = args.Longitude,
                Latitude = args.Latitude
            };
            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                model.GeoHash = Geohash.Encode(model.Latitude.Value, model.Longitude.Value);
            }
            else
            {
                model.GeoHash = string.Empty;
            }
            db.Add<CommercialDistrict>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();
            if (!args.Status.HasValue) args.Status = CommercialDistrictStatus.正常;

            var model = db.Query<CommercialDistrict>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");
            model.Flag = args.Flag;
            model.Name = args.Name;
            model.Status = args.Status.Value;
            model.Detail = args.Detail;
            model.Address = args.Address;
            model.Longitude = args.Longitude;
            model.Latitude = args.Latitude;
            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                model.GeoHash = Geohash.Encode(model.Latitude.Value, model.Longitude.Value);
            }
            else
            {
                model.GeoHash = string.Empty;
            }
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var viewModel = db.Query<CommercialDistrict>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Id = m.Id,
                    Address = m.Address,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Name = m.Name,
                    Status = m.Status,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<CommercialDistrict>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
