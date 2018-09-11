using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBrandCommodityParameterValueSetAPIModels;
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
    public class ShopBrandCommodityParameterValueSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandCommodityParameterValueSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        private void CheckShopBrandActorByParameterId(int parameterId, params ShopBrandActorType[] actorTypes)
        {
            var parameter = db.Query<ShopBrandCommodityParameter>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == parameterId)
                .Select(m => new
                {
                    Id = m.Id,
                    ShopBrandId = m.ShopBrandId
                })
                .FirstOrDefault();
            if (parameter == null) throw new Exception("关联的商铺参数纪录不存在");
            //获取到参数关联的品牌才能进行判断
            CheckShopBrandActor(parameter.ShopBrandId, actorTypes);
        }

        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ParameterId.HasValue) throw new ArgumentNullException("ParameterId");
            CheckShopBrandActorByParameterId(args.ParameterId.Value, ShopBrandActorType.超级管理员);

            var query = db.Query<ShopBrandCommodityParameterValue>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ParameterId == args.ParameterId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    ParameterId = m.ParameterId,
                    Value = m.Value

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
            if (!args.ParameterId.HasValue) throw new ArgumentNullException("ParameterId");
            CheckShopBrandActorByParameterId(args.ParameterId.Value, ShopBrandActorType.超级管理员);

            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandCommodityParameterValue>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ParameterId == args.ParameterId);

            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    ParameterId = m.ParameterId,
                    Value = m.Value
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
            if (string.IsNullOrEmpty(args.Value)) throw new ArgumentNullException("Name");

            var parameter = db.GetSingle<ShopBrandCommodityParameter>(args.ParameterId);
            if (parameter == null) throw new Exception("关联的属性不存在");

            CheckShopBrandActor(parameter.ShopBrandId, ShopBrandActorType.超级管理员);


            var model = new ShopBrandCommodityParameterValue()
            {
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                Parameter = parameter,
                Value = args.Value
            };

            db.Add<ShopBrandCommodityParameterValue>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var viewModel = db.Query<ShopBrandCommodityParameterValue>()
                .Where(m => m.Id == args.Id)
                .Select(m => new GetSingleModel()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    IsDel = m.IsDel,
                    ParameterId = m.ParameterId,
                    Value = m.Value
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActorByParameterId(viewModel.ParameterId, ShopBrandActorType.超级管理员);


            return Success(viewModel);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopBrandCommodityParameterValue>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //在获取后检查是否拥有管理权限
            CheckShopBrandActorByParameterId(model.ParameterId, ShopBrandActorType.超级管理员);

            var records = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => m.ParameterValueId == model.Id)
                .Count();

            if (records > 0)
            {
                throw new Exception("不能删除此规格属性,尚有使用它的商品");
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
