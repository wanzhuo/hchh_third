using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat.Open;
using Newtonsoft.Json;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Authorize]
        public APIResult<List<GetShopItem>> GetShops([FromBody]GetShopsArgsModel args)
        {
            var memberId = GetMemberId();
            var query = db.Query<ShopActor>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.MemberId == memberId)
                     .Where(m => !m.Shop.IsDel);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new GetShopItem()
                {
                    ShopId = m.ShopId,
                    Name = m.Shop.Name,
                    ShopFlag = m.Shop.Flag
                })
                .ToList();

            return Success(list);
        }


        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            if (!args.Id.HasValue) throw new ArgumentNullException("Id");
            CheckShopActor(args.Id.Value, ShopActorType.超级管理员);

            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("Name");
            var model = db.Query<Shop>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.Id.Value)
                .FirstOrDefault();
            if (model == null) throw new Exception("数据库记录不存在");

            model.Address = args.Address;
            model.AddressGuide = args.AddressGuide;
            model.Detail = args.Detail;
            model.Name = args.Name;
            model.OpenTime = args.OpenTime;
            model.Tel = args.Tel;
            model.UsePerUser = args.UsePerUser;
            model.Logo = args.Logo;
            model.Cover = args.Cover;
            model.Longitude = args.Longitude;
            model.Latitude = args.Latitude;
            model.AreaCode = args.AreaCode;
            model.AreaText = args.AreaText;
            List<BannerModel> bannermodel = new List<BannerModel>();
            string bannerjson = string.Empty;
            try
            {
                if (args.Banners != null && args.Banners != "")
                {
                    var banners = args.Banners.Split(",");
                    for (int i = 0; i < banners.Length; i++)
                    {
                        bannermodel.Add(new BannerModel() { Id = i + 1, Name = banners[i], Url = "http://91huichihuihe.oss-cn-shenzhen.aliyuncs.com/" + banners[i], Sorting = i + 1, IsShow = true });
                    }

                    bannerjson = JsonConvert.SerializeObject(bannermodel);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           
            model.Banners = bannerjson;
            db.SaveChanges();
            return Success();
        }
        [HttpPost]
        public APIResult TestBanner([FromBody] string json)
        {
            List<BannerModel> bannerModels = new List<BannerModel>() {

                new BannerModel(){
                     Id = 1, Name = "a", Link = "", Url = "", Sorting = 1001, IsShow = true
                },
                      new BannerModel(){
                     Id = 2, Name = "b", Link = "", Url = "", Sorting = 1002, IsShow = true
                },
                      new BannerModel(){
                     Id = 3, Name = "c", Link = "", Url = "", Sorting = 1003, IsShow = true
                },
            };
            return Success(bannerModels);
        }

        /// <summary>
        /// 设置是否营业中
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public APIResult SetIsOpen([FromBody] SetIsOpenArgsModels args)
        //{
        //    if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
        //    CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);
        //    var model = db.GetSingle<Shop>(args.ShopId.Value);
        //    if (model == null) throw new Exception("记录不存在");
        //    model.IsOpen = args.IsOpen;
        //    db.SaveChanges();
        //    return Success();
        //}

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            //权限判定
            CheckShopActor(args.Id, ShopActorType.超级管理员);

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
                    Logo = m.Logo,
                    Cover = m.Cover,
                    AreaText = m.AreaText,
                    AreaCode = m.AreaCode,
                    ShopBrandId = m.ShopBrandId,
                    Banners = m.Banners
                })
                .FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");

            return Success(viewModel);
        }



        [HttpPost]
        [Authorize]
        public APIResult GetShopTakeOutInfo([FromBody]GetShopTakeOutInfoArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");

            //权限判定
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var model = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();

            if (model == null)
            {
                DateTime now = DateTime.Now;
                model = new ShopTakeOutInfo()
                {
                    IsUseTakeOut = false,
                    BoxFee = 0,
                    DeliveryFee = 0,
                    IsOpen = false,
                    AddTime = now,
                    AutoPrint = true,
                    AddIp = GetIp(),
                    Area = 0,
                    AutoTakeOrdre = true,
                    MinAmount = 0,
                    StartTime = now,
                    EndTime = now.AddHours(3),
                    ShopId = args.ShopId.Value,
                    TakeDistributionType = args.TakeDistributionType,
                    IsDel = false
                };
                db.Add(model);
                db.SaveChanges();
            }
            string startTime = model.StartTime?.ToString("HH:mm:ss");
            string endTime = model.EndTime?.ToString("HH:mm:ss");
            return Success(new
            {
                model.AutoTakeOrdre,
                BoxFee = model.BoxFee / 100d,
                DeliveryFee = model.DeliveryFee / 100d,
                model.Id,
                model.IsOpen,
                model.ShopId,
                model.IsUseTakeOut,
                MinAmount = model.MinAmount / 100d,
                startTime,
                endTime,
                scope = model.Area,
                TakeDistributionType = model.TakeDistributionType
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult SetShopTakeOutInfo([FromBody]SetShopTakeOutInfoArgsModel args)
        {
            if (args.ShopId == 0) throw new Exception("ShopID不能为空");

            //权限判定
            CheckShopActor(args.ShopId, ShopActorType.超级管理员);

            var model = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId)
                .FirstOrDefault();

            if (model == null)
            {
                model = new ShopTakeOutInfo()
                {
                    ShopId = args.ShopId,
                    AutoPrint = args.AutoPrint,
                    AutoTakeOrdre = args.AutoTakeOrdre,
                    Area = args.scope,
                    BoxFee = (int)Math.Floor(args.BoxFee * 100),
                    DeliveryFee = (int)Math.Floor(args.DeliveryFee * 100),
                    MinAmount = (int)Math.Floor(args.MinAmount * 100),
                    AddIp = GetIp(),
                    StartTime = args.StartTime,
                    EndTime = args.EndTime,
                    AddTime = DateTime.Now,
                    TakeDistributionType = args.TakeDistributionType
                };
                db.AddTo(model);
            }
            else
            {
                model.AutoPrint = args.AutoPrint;
                model.AutoTakeOrdre = args.AutoTakeOrdre;
                model.Area = args.scope;
                model.BoxFee = (int)Math.Floor(args.BoxFee * 100);
                model.DeliveryFee = (int)Math.Floor(args.DeliveryFee * 100);
                model.MinAmount = (int)Math.Floor(args.MinAmount * 100);
                model.StartTime = args.StartTime;
                model.EndTime = args.EndTime;
                model.TakeDistributionType = args.TakeDistributionType;
            }
            db.SaveChanges();
            return Success(model);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetUsedShopTakeOut([FromBody]SetUsedShopTakeOutArgsModel args)
        {
            if (args.ShopId == 0) throw new Exception("ShopID不能为空");

            //权限判定
            CheckShopActor(args.ShopId, ShopActorType.超级管理员);

            var model = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId)
                .FirstOrDefault();

            if (model == null) throw new Exception("请先设置外卖信息");
            model.IsUseTakeOut = args.IsUsed;
            db.SaveChanges();
            return Success();
        }

        /// <summary>
        /// 获取自动点餐商户设置
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetShopSelfHelp([FromBody]ShopIdArgModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            bool isSelfHelp = db.GetSingle<Shop>(args.ShopId.Value).IsSelfHelp;
            ShopSelfHelpInfo shopSelfHelpInfo = db.Query<ShopSelfHelpInfo>()
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();
            if (shopSelfHelpInfo == null)
            {
                shopSelfHelpInfo = new ShopSelfHelpInfo()
                {
                    HasBoxFee = false,
                    BoxFee = 0,
                    ShopId = args.ShopId.Value,
                    HasTakeOut = true
                };
                db.Add(shopSelfHelpInfo);
                db.SaveChanges();
            }
            return Success(new
            {
                isSelfHelp,
                shopSelfHelpInfo.HasBoxFee,
                shopSelfHelpInfo.BoxFee
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult SetShopSelfHelpIsSelfHelp([FromBody]SetShopSelfHelpIsSelfHelpArgModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            var model = db.GetSingle<Shop>(args.ShopId.Value);
            model.IsSelfHelp = args.IsSelfHelp;
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetShopSelfHelpHasBoxFee([FromBody]SetShopSelfHelpHasBoxFeeArgModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            ShopSelfHelpInfo shopSelfHelpInfo = db.Query<ShopSelfHelpInfo>()
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();
            if (shopSelfHelpInfo == null) throw new Exception("记录不存在");
            shopSelfHelpInfo.HasBoxFee = args.HasBoxFee;
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetShopSelfHelpBoxFee([FromBody]SetShopSelfHelpBoxFeeArgModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            ShopSelfHelpInfo shopSelfHelpInfo = db.Query<ShopSelfHelpInfo>()
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();
            if (shopSelfHelpInfo == null) throw new Exception("记录不存在");
            shopSelfHelpInfo.BoxFee = args.BoxFee;
            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult SetOpenShopTakeOut([FromBody]SetOpenShopTakeOutArgsModel args)
        {
            if (args.ShopId == 0) throw new Exception("ShopID不能为空");

            //权限判定
            CheckShopActor(args.ShopId, ShopActorType.超级管理员);

            var model = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId)
                .FirstOrDefault();

            if (model == null) throw new Exception("记录不存在");
            model.IsOpen = args.IsOpen;
            db.SaveChanges();
            return Success();
        }

        /// <summary>
        /// 设置自助点餐
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetShopSelfHelpInfo([FromBody]SetShopSelfHelpInfoArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            //权限判定
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);
            var model = db.Query<ShopSelfHelpInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId)
                .FirstOrDefault();

            if (model == null)
            {
                model = new ShopSelfHelpInfo()
                {
                    ShopId = args.ShopId.Value,
                    BoxFee = args.BoxFee,
                    HasTakeOut = args.HasTakeOut,
                };
                db.Add(model);
            }
            else
            {
                model.BoxFee = args.BoxFee;
                model.HasTakeOut = args.HasTakeOut;
            }
            db.SaveChanges();
            return Success();
        }

    }
}
