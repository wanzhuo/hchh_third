using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopCallingQueueProductSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 排队叫号产品设置API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopCallingQueueProductSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopCallingQueueProductSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 获取指定商铺的排队叫号产品列表
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopCallingQueueProduct>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Detail = m.Detail,
                    Status = m.Status,
                    Title = m.Title,
                    Name = m.Name,
                    Id = m.Id,
                    ShopId = m.ShopId
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        /// <summary>
        /// 添加餐桌
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            //获取并验证店铺是否存在
            var shop = db.GetSingle<Shop>(args.ShopId.Value);
            if (shop == null) throw new Exception("指定的商铺不存在");

            //这里只是添加一个库存纪录，库存的参数在编辑处修改
            var model = new ShopCallingQueueProduct()
            {
                Shop = shop,
                Status = ShopCallingQueueProductStatus.正常,
                Title = args.Title,
                Name = args.Name,
                Detail = args.Detail
            };
            db.Add<ShopCallingQueueProduct>(model);
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            var model = db.Query<ShopCallingQueueProduct>()
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.Detail = args.Detail;
            model.Title = args.Title;
            model.Name = args.Name;
            model.Status = args.Status;

            db.SaveChanges();
            return Success();
        }
        /// <summary>
        /// 设置为已删除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopCallingQueueProduct>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 获取商铺是否开放的状态
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetShopOpenStatus([FromBody]IdArgsModel args)
        {
            CheckShopActor(args.Id, ShopActorType.超级管理员);

            var flag = ShopCallingQueue.GetShopOpenStatusFlag(args.Id);
            var v = db.GetSettingValue<bool>(flag);
            return Success(v);
        }
        /// <summary>
        /// 设置商铺的开放状态
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> SetShopOpenStatus([FromBody]SetShopOpenStatusArgsModel args)
        {
            CheckShopActor(args.Id, ShopActorType.超级管理员);

            var flag = ShopCallingQueue.GetShopOpenStatusFlag(args.Id);
            db.SetSettingValue(flag, args.IsOpen.ToString());
            await db.SaveChangesAsync();
            return Success();
        }
    }
}
