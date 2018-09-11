using System;
using System.Linq;
using ZRui.Web.ShopBookingAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopBookingAPIController : WechatApiControllerBase
    {
        static object lockAddObject = new object();
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBookingAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取我的预定列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetListForMe([FromBody]GetListForMeArgsModel args)
        {
            var memberId = GetMemberId();
            var query = db.Query<ShopBooking>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId);

            if (!string.IsNullOrEmpty(args.ShopFlag))
            {
                query = query.Where(m => m.Shop.Flag == args.ShopFlag);
            }

            if (args.Status.HasValue)
            {
                query = query.Where(m => m.Status == args.Status.Value);
            }

            if (args.IsUsed.HasValue)
            {
                query = query.Where(m => m.IsUsed == args.IsUsed.Value);
            }

            var items = query
                 .Select(m => new RowItem
                 {
                     AddTime = m.AddTime,
                     DinnerTime = m.DinnerTime,
                     IsUsed = m.IsUsed,
                     Id = m.Id,
                     Nickname = m.Nickname,
                     Phone = m.Phone,
                     RefuseReason = m.RefuseReason,
                     Remark = m.Remark,
                     Status = m.Status,
                     Users = m.Users,
                     ShopName = m.Shop.Name
                 })
                 .ToList();

            return Success(new GetListForMeModel()
            {
                Items = items
            });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            var memberId = GetMemberId();
            var isExit = db.Query<ShopBooking>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Status == ShopBookingStatus.待确认 || m.Status == ShopBookingStatus.确认成功)
                .Where(m => !m.IsUsed)
                .Count() > 0;
            if (isExit) throw new Exception("你已经预定过，如果想变更，需要先取消再重新预定");

            var startTime = DateTime.Today;
            var endTime = startTime.AddDays(1);

            var model = new ShopBooking()
            {
                AddTime = DateTime.Now,
                MemberId = memberId,
                ShopId = args.ShopId,
                Users = args.Users,
                DinnerTime = args.DinnerTime,
                Nickname = args.Nickname,
                Phone = args.Phone,
                Remark = args.Remark,
                Status = ShopBookingStatus.待确认
            };

            db.AddTo<ShopBooking>(model);
            db.SaveChanges();

            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopBooking>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetCancel([FromBody]IdArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopBooking>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            model.Status = ShopBookingStatus.取消;
            db.SaveChanges();
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopBooking>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetRemark([FromBody]SetRemarkArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopBooking>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            model.Remark = args.Remark;
            db.SaveChanges();
            return Success(model);
        }
    }
}
