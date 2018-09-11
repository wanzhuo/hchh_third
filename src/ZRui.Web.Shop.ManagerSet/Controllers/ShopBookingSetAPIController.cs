using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 预定设置API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopBookingSetAPIController : ShopManagerApiControllerBase
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
        public ShopBookingSetAPIController(ICommunityService communityService
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
        /// 获得指定ShopId的预约列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<GetListModel> GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var query = db.Query<ShopBooking>()
                     .Where(m => !m.IsDel);

            var list = query
                .Where(m => m.ShopId == args.ShopId.Value)
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddTime = m.AddTime,
                    RefuseReason = m.RefuseReason,
                    Remark = m.Remark,
                    Status = m.Status,
                    DinnerTime = m.DinnerTime,
                    Nickname = m.Nickname,
                    Phone = m.Phone,
                    Users = m.Users,
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
            var model = db.GetSingle<ShopBooking>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.Status = args.Status;
            model.RefuseReason = args.RefuseReason;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 设置未已使用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsUsed([FromBody]SetIsUsedArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopBooking>(args.Id);
            if (model == null) throw new Exception("数据库记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsUsed = args.IsUsed;
            db.SaveChanges();

            return Success();
        }
        /// <summary>
        /// 设置为删除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopBooking>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
