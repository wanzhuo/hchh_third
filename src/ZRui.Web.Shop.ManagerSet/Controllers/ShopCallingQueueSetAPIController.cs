using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopCallingQueueSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 叫号排队处理API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopCallingQueueSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="db"></param>
        /// <param name="memberDb"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopCallingQueueSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取指定商铺的叫号排队列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopCallingQueue>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
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
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetStatus([FromBody]SetStatusArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.Status = args.Status;
            model.RefuseReason = args.RefuseReason;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 设置是否已经使用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsUsed([FromBody]SetIsUsedArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsUsed = args.IsUsed;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 设置排队的次序
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetQueueIndex([FromBody]SetQueueIndexArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.QueueIndex = args.QueueIndex;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 是指是否删除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
