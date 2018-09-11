using System;
using System.Linq;
using ZRui.Web.ShopCallingQueueAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCallingQueueAPIController : WechatApiControllerBase
    {
        static object lockAddObject = new object();
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCallingQueueAPIController(ICommunityService communityService
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


        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetProducts([FromBody]GetProductsArgsModel args)
        {
            var query = db.Query<ShopCallingQueueProduct>()
                 .Where(m => !m.IsDel);

            var items = query
                .Where(m => m.ShopId == args.ShopId)
                .Where(m => m.Status == ShopCallingQueueProductStatus.正常)
                .Select(m => new GetProductsRowItem()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Detail = m.Detail
                })
                .ToList();

            return Success(new GetProductsModel()
            {
                Items = items
            });
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
            var query = db.Query<ShopCallingQueue>()
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
                     IsUsed = m.IsUsed,
                     Id = m.Id,
                     RefuseReason = m.RefuseReason,
                     Remark = m.Remark,
                     Status = m.Status,
                     ShopName = m.Shop.Name,
                     CanShareTable = m.CanShareTable,
                     QueueIndex = m.QueueIndex,
                     QueueNumber = m.QueueNumber,
                     Title = m.Title
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
            //需要判定是否已经开放叫号
            var flag = ShopCallingQueue.GetShopOpenStatusFlag(args.ShopId);
            var isOpen = db.GetSettingValue<bool>(flag);
            if (!isOpen) throw new Exception("还未开放叫号，请等待");

            ShopCallingQueueProduct product = null;
            if (args.ProductId.HasValue)
            {
                product = db.Query<ShopCallingQueueProduct>()
                 .Where(m => !m.IsDel)
                 .Where(m => m.Status == ShopCallingQueueProductStatus.正常)
                 .Where(m => m.Id == args.ProductId && m.ShopId == args.ShopId)
                 .FirstOrDefault();

                if (product == null) throw new Exception("指定的人数设定不存在或者未开放");
                args.Title = product.Title;
            }

            var memberId = GetMemberId();
            var isExit = db.Query<ShopCallingQueue>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.ShopId == args.ShopId)
                .Where(m => !m.IsUsed)
                .Where(m => m.Status != ShopCallingQueueStatus.取消)
                .Where(m => m.Status != ShopCallingQueueStatus.确认失败)
                .Where(m => DateTime.Now.Date.Equals(m.AddTime.Date))
                .Count() > 0;
            if (isExit) throw new Exception("你已经在排队中，如果想重新排队，需要先取消");

            var startTime = DateTime.Today;
            var endTime = startTime.AddDays(1);

            var model = new ShopCallingQueue()
            {
                AddTime = DateTime.Now,
                CanShareTable = args.CanShareTable,
                MemberId = memberId,
                ProductId = args.ProductId,
                ShopId = args.ShopId,
                Title = args.Title,
                Status = product == null ? ShopCallingQueueStatus.待确认 : ShopCallingQueueStatus.确认成功
            };

            lock (lockAddObject)
            {//锁定，用于保证每次生成的QueueNumber都是唯一的
                var count = db.Query<ShopCallingQueue>()
                    .Where(m => m.AddTime >= startTime && m.AddTime < endTime)
                    .Count();
                count++; //增加1，否则开始是从0开始
                model.QueueIndex = count;
                model.QueueNumber = count;

                db.AddTo<ShopCallingQueue>(model);
                db.SaveChanges();
            }
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopCallingQueue>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopCallingQueue>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetCancel([FromBody]IdArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopCallingQueue>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定纪录不存在");

            model.Status = ShopCallingQueueStatus.取消;
            db.SaveChanges();
            return Success(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetRemark([FromBody]SetRemarkArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopCallingQueue>()
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
