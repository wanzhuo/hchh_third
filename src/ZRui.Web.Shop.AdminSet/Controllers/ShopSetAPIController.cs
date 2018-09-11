using System;
using System.Linq;
using ZRui.Web.ShopSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopSetAPIController(ICommunityService communityService
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
            var query = db.Query<Shop>()
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
                    Logo = m.Logo,
                    Id = m.Id,
                    Address = m.Address,
                    AddressGuide = m.AddressGuide,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Name = m.Name,
                    OpenTime = m.OpenTime,
                    ScoreValue = m.ScoreValue,
                    Tel = m.Tel,
                    UsePerUser = m.UsePerUser,
                    ShopBrandId = m.ShopBrandId,
                    IsShowApplets = m.IsShowApplets
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
            if (args.PageSize <= 0) args.PageSize = 100;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<Shop>()
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
                    Logo = m.Logo,
                    Id = m.Id,
                    Address = m.Address,
                    AddressGuide = m.AddressGuide,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Name = m.Name,
                    OpenTime = m.OpenTime,
                    ScoreValue = m.ScoreValue,
                    Tel = m.Tel,
                    UsePerUser = m.UsePerUser,
                    ShopBrandId = m.ShopBrandId,
                    IsShowApplets = m.IsShowApplets
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            list.ForEach(r =>
            {
                var model = db.Query<ShopPayInfo>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == r.Id)
                     .FirstOrDefault();

                if (model != null)
                {
                    bool HasSecretKey = (model.SecretKey != null && model.SecretKey.Length > 0);
                    r.MchId = model.MchId;
                    r.PayWay = ((int)model.PayWay).ToString();
                    r.SecretKey = HasSecretKey ? "********" : "";
                }

            });

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
            var brand = db.GetSingle<ShopBrand>(args.ShopBrandId);
            if (brand == null) throw new Exception("指定的商铺品牌不存在");

            var model = new Shop()
            {
                Flag = args.Flag,
                ShopBrand = brand,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Address = args.Address,
                AddressGuide = args.AddressGuide,
                Detail = args.Detail,
                Latitude = args.Latitude,
                Longitude = args.Longitude,
                Name = args.Name,
                OpenTime = args.OpenTime,
                ScoreValue = args.ScoreValue,
                Tel = args.Tel,
                UsePerUser = args.UsePerUser,
                Logo = args.Logo,
                IsShowApplets = args.IsShowApplets


            };

            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                model.GeoHash = Geohash.Encode(model.Latitude.Value, model.Longitude.Value);
            }
            else
            {
                model.GeoHash = string.Empty;
            }
            db.Add<Shop>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();
            var model = db.Query<Shop>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            var brand = db.GetSingle<ShopBrand>(args.ShopBrandId);
            if (brand == null) throw new Exception("指定的商铺品牌不存在");
            model.ShopBrand = brand;

            model.Flag = args.Flag;
            model.Address = args.Address;
            model.AddressGuide = args.AddressGuide;
            model.Detail = args.Detail;
            model.Latitude = args.Latitude;
            model.Longitude = args.Longitude;
            model.Name = args.Name;
            model.OpenTime = args.OpenTime;
            model.ScoreValue = args.ScoreValue;
            model.Tel = args.Tel;
            model.UsePerUser = args.UsePerUser;
            model.Logo = args.Logo;
            model.IsShowApplets = args.IsShowApplets;
            model.Phone = args.Phone;
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
            var viewModel = db.Query<Shop>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Flag = m.Flag,
                    Logo = m.Logo,
                    Id = m.Id,
                    Address = m.Address,
                    AddressGuide = m.AddressGuide,
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Name = m.Name,
                    OpenTime = m.OpenTime,
                    ScoreValue = m.ScoreValue,
                    Tel = m.Tel,
                    UsePerUser = m.UsePerUser
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<Shop>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
