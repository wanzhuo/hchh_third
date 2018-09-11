using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBrandSetAPIModels;
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
    public class ShopBrandSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        public APIResult GetShopBrands([FromBody]GetShopBrandsArgsModel args)
        {
            var memberId = GetMemberId();
            var query = db.Query<ShopBrandActor>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.MemberId == memberId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new GetShopBrandItem()
                {

                    ShopBrandId = m.ShopBrandId,
                    Name = m.ShopBrand.Name
                })
                .ToList();

            return Success(list);
        }


        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (!args.Id.HasValue) throw new ArgumentNullException("Id");
            CheckShopBrandActor(args.Id.Value, ShopBrandActorType.超级管理员);

            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            var model = db.Query<ShopBrand>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.Id.Value)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            model.Address = args.Address;
            model.Detail = args.Detail;
            model.Name = args.Name;
            model.Cover = args.Cover;
            model.Logo = args.Logo;

            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            //权限判定
            CheckShopBrandActor(args.Id, ShopBrandActorType.超级管理员);

            var viewModel = db.Query<Shop>()
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
                    Cover = m.Cover,
                    Logo = m.Logo
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");

            return Success(viewModel);
        }
    }
}
