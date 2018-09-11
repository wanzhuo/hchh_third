using System;
using System.Linq;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using ZRui.Web.ShopOrderMoneyOffSetAPIModel;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopOrderMoneyOffSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopOrderMoneyOffSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgs args)
        {
            if (args.ShopId==0) throw new ArgumentNullException("ShopId");
            if (!args.StartDate.HasValue) throw new ArgumentNullException("StartDate");
            if (!args.EndDate.HasValue) throw new ArgumentNullException("EndDate");
            if (args.Items.Count == 0) throw new Exception("请输入具体规则");
            CheckShopActor(args.ShopId, ShopActorType.超级管理员);

            ShopOrderMoneyOff shopOrderMoneyOff = new ShopOrderMoneyOff()
            {
                Name = args.Name,
                StartDate = args.StartDate.Value.Date,
                EndDate = args.EndDate.Value.Date,
                ShopId = args.ShopId,
                IsScanCode = args.IsScanCode,
                IsSelfOrder = args.IsSelfOrder,
                IsTakeout = args.IsTakeout,
                IsEnable = true,
                IsDel = false
            };

            db.AddTo(shopOrderMoneyOff);
            db.SaveChanges();

            //若在优惠期间，则开始优惠
            DateTime now = DateTime.Now.Date;
            if (now >= shopOrderMoneyOff.StartDate.Value && now <= shopOrderMoneyOff.EndDate.Value)
            {
                ShopOrderMoneyOffCache cache = new ShopOrderMoneyOffCache()
                {
                    ShopId = shopOrderMoneyOff.ShopId,
                    MoneyOffId = shopOrderMoneyOff.Id
                };
                db.AddTo(cache);
            }


            foreach (var item in args.Items)
            {
                ShopOrderMoneyOffRule rule = new ShopOrderMoneyOffRule()
                {
                    MoneyOffId = shopOrderMoneyOff.Id,
                    FullAmount = (int)Math.Floor(item.FullAmount * 100),
                    Discount = (int)Math.Floor(item.Discount * 100),
                    IsDel = false
                };
                db.AddTo(rule);
            }
            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopOrderMoneyOff>(args.Id);
            if (model == null) throw new Exception("记录不存在");
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            model.IsDel = true;

            var caches = db.Query<ShopOrderMoneyOffCache>()
                .Where(m => !m.IsDel)
                .Where(m => m.MoneyOffId == model.Id)
                .ToList();

            foreach (var item in caches)
            {
                item.IsDel = true;
            }

            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var list = db.Query<ShopOrderMoneyOff>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult SetMoneyOffIsEnable([FromBody]SetMoneyOffIsEnableArgsModel args)
        {
            var model = db.GetSingle<ShopOrderMoneyOff>(args.Id);
            if (model == null) throw new Exception("记录不存在");
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            model.IsEnable = args.IsEnable;
            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult GetRuleList([FromBody]GetRuleListArgsModel args)
        {
            if (!args.id.HasValue) throw new ArgumentNullException("满减id");
            var moneyOff = db.GetSingle<ShopOrderMoneyOff>(args.id.Value);
            if (moneyOff == null) throw new Exception("记录不存在");

            CheckShopActor(moneyOff.ShopId, ShopActorType.超级管理员);


            var list = db.Query<ShopOrderMoneyOffRule>()
                .Where(m => !m.IsDel)
                .Where(m => m.MoneyOffId == moneyOff.Id)
                .Select(m => new RuleModel()
                {
                    Discount = m.Discount / 100m,
                    FullAmount = m.FullAmount / 100m
                })
                .ToList();

            return Success(new GetRuleListModel()
            {
                Items = list
            });
        }
    }
}
