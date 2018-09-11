using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using ZRui.Web.ShopManager.ShopCommodityComboSetAPIModels;
using System.Linq;
using ZRui.Web.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopCommodityComboSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommodityComboSetAPIController(ICommunityService communityService
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
        /// 添加套餐
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult AddFixCombo([FromBody]ComboModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");

            var shop = db.GetSingle<Shop>(args.ShopId.Value);
            if (shop == null) throw new Exception("商铺记录不存在");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            ShopBrandCombo shopBrandCombo = new ShopBrandCombo()
            {
                AddIp = GetIp(),
                IsTakeout = args.IsTakeout,
                IsScanCode = args.IsScanCode,
                IsSelfOrder = args.IsSelfOrder,
                Price = args.Price,
                Name = args.Name,
                ShopBrandId = shop.ShopBrandId,
                Cover = args.Cover,
                Flag = Guid.NewGuid().ToString(),
                ComboType = ComboType.固定套餐,
                AddUser = GetUsername(),
                AddTime = DateTime.Now,
                Unit = args.Unit
            };
            db.Add(shopBrandCombo);
            db.SaveChanges();

            foreach (var group in args.Groups)
            {
                foreach (var item in group.Items)
                {
                    ShopBrandFixComboItem comboItem = new ShopBrandFixComboItem()
                    {
                        ComboId = shopBrandCombo.Id,
                        CommodityName = item.CommodityName,
                        Count = item.Count,
                        SalePrice = item.SalePrice,
                        Sku = item.Sku
                    };
                    db.Add(comboItem);
                }
            }
            db.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 获取套餐设置列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public APIResult GetComboList([FromBody]GetComboArgsModels args)
        //{


        //}

        /// <summary>
        /// 获取指定商铺的商铺列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]                    
        public async Task<APIResult> GetShopCommodities([FromBody]IdArgsModel args)
        {
            if (args.Id == 0) throw new Exception("ShopId不能为空");
            var res = await Task.Run(() =>
            {
                Shop shop = db.GetSingle<Shop>(args.Id);
                if (shop == null) throw new Exception("商铺记录不存在");
                ShopBrandCommodityServer server = new ShopBrandCommodityServer(db, shop, DiningWay.所有);
                CategoryAndCommodityModel rtn = server.GetCategoryAndCommodity();
                return rtn;
            });
            return Success(res);
        }


        [HttpPost]
        [Authorize]
        public APIResult GetAllCommodity([FromBody] GetAllCommodityArgsModels args)
        {
            var list = db.Query<ShopBrandCommodity>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopBrandId == args.ShopBrandId)
                .Where(m => m.CategoryId != 0)
                .Select(m => new
                {
                    key = m.Id,
                    label = m.Name
                })
                .ToList();

            return Success(list);
        }


        [HttpPost]
        [Authorize]
        public APIResult SetComboIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopBrandCombo>(args.Id);
            if (model == null) throw new Exception("记录不存在");
            //获取到订单后判断是否拥有指定的店铺的权限
            CheckShopBrandActor(model.ShopBrandId, ShopBrandActorType.超级管理员);
            model.IsDel = true;
            db.SaveChanges();
            return Success();
        }
    }
}
