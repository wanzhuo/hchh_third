using System;
using System.Linq;
using ZRui.Web.ShopAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using static ZRui.Web.Common.Geohash;
using ZRui.Web.Core.Wechat;
using ZRui.Web.BLL;
using System.IO;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取九个格子 顺序 本身 上、下、左、右、 左上、 右上、 左下、右下
        /// </summary>
        /// <param name="geohash"></param>
        /// <returns></returns>
        List<string> GetGeoHashExpand(String geohash)
        {
            String geohashTop = CalculateAdjacent(geohash, Direction.Top);//上
            String geohashBottom = CalculateAdjacent(geohash, Direction.Bottom);//下
            String geohashLeft = CalculateAdjacent(geohash, Direction.Left);//左
            String geohashRight = CalculateAdjacent(geohash, Direction.Right);//右
            String geohashTopLeft = CalculateAdjacent(geohashLeft, Direction.Top);//左上
            String geohashTopRight = CalculateAdjacent(geohashRight, Direction.Top);//右上
            String geohashBottomLeft = CalculateAdjacent(geohashLeft, Direction.Bottom);//左下
            String geohashBottomRight = CalculateAdjacent(geohashRight, Direction.Bottom);//右下
            return new List<string>() { geohash, geohashTop, geohashBottom, geohashLeft, geohashRight, geohashTopLeft, geohashTopRight, geohashBottomLeft, geohashBottomRight };
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<Shop>()
                     .Where(m => !m.IsDel);

            if (args.Latitude.HasValue && args.Longitude.HasValue)
            {
                //6 范围在±0.61KM
                var geohash = Geohash.Encode(args.Latitude.Value, args.Longitude.Value, 6);
                var areas = GetGeoHashExpand(geohash);
                query = query.Where(m => m.GeoHash.StartsWith(areas[0])
                || m.GeoHash.StartsWith(areas[1])
                || m.GeoHash.StartsWith(areas[2])
                || m.GeoHash.StartsWith(areas[3])
                || m.GeoHash.StartsWith(areas[4])
                || m.GeoHash.StartsWith(areas[5])
                || m.GeoHash.StartsWith(areas[6])
                || m.GeoHash.StartsWith(areas[7])
                );
            }

            var list = query
                .Select(m => new RowItem()
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
                    Cover = m.Cover,
                    Logo = m.Logo

                })
                .ToPagedList(args.PageIndex, args.PageSize);

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.Where(r => r.IsShowApplets == true).ToList()
            });
        }


        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetInfo([FromBody]GetSingleArgs args)
        {

            var shopModel = db.Query<Shop>()
                .Where(m => m.Flag == args.Flag)
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
                    Cover = m.Cover,
                    IsSelfHelp = m.IsSelfHelp,
                    Logo = m.Logo,

                })
                .FirstOrDefault();


            if (shopModel == null) throw new Exception("记录不存在");


            shopModel.CurrentVersion = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopModel.Id)
                .FirstOrDefault()?.CurrentTemplateUserVersion;

            if (shopModel.IsSelfHelp) //自助点餐
            {
                shopModel.DiningWay = 0;
                shopModel.HasSelfHelpTakeout = true;  //当前默认都有外带
                ShopSelfHelpInfo selfHelpInfo = db.Query<ShopSelfHelpInfo>()
                    .Where(m => !m.IsDel && m.ShopId == shopModel.Id)
                    .FirstOrDefault();
                if (selfHelpInfo != null && selfHelpInfo.HasBoxFee)
                {
                    shopModel.SelfHelpBoxFee = selfHelpInfo.BoxFee;
                }
                //shopModel.HasSelfHelpTakeout = db.Query<ShopSelfHelpInfo>()
                //    .Where(m => !m.IsDel && m.ShopId == shopModel.Id)
                //    .FirstOrDefault()?
                //    .HasTakeOut ?? false;
            }
            else
            {
                shopModel.DiningWay = 1;
            }
            var shopMemberSet = db.Query<ShopMemberSet>()
                .FirstOrDefault(m => !m.IsDel && m.ShopId == shopModel.Id);
            if (shopMemberSet == null)
                shopModel.IsTopUpDiscount = false;
            else
                shopModel.IsTopUpDiscount = shopMemberSet.IsTopUpDiscount;

            var viewModel = new GetInfoModel()
            {
                ShopInfo = shopModel,
                BannerModel = Utils.ExBannerModels.GetBannerList(db, args.Flag)
            };

            viewModel.TakeOutInfo = db.Query<ShopTakeOutInfo>()
                    .Where(m => !m.IsDel && m.ShopId == shopModel.Id)
                    .Select(m => new TakeOutInfo()
                    {
                        IsUseTakeOut = m.IsUseTakeOut,
                        BoxFee = m.BoxFee / 100m,
                        DeliveryFee = m.DeliveryFee / 100m,
                        IsOpen = m.IsOpen,
                        MinAmount = m.MinAmount / 100m,
                        StartTime = m.StartTime == null ? " " : m.StartTime.Value.ToString("HH:mm"),
                        EndTime = m.EndTime == null ? " " : m.EndTime.Value.ToString("HH:mm")
                    })
                    .FirstOrDefault();

            if (shopModel.Latitude.HasValue && shopModel.Longitude.HasValue)
            {
                var TxCoordinate = BaiduMapUtil.CoverCoordinateToTX(shopModel.Latitude.Value, shopModel.Longitude.Value);
                shopModel.Latitude = TxCoordinate.lati;
                shopModel.Longitude = TxCoordinate.logi;
            }

            //shopMember信息
            //try
            //{
            //    var memberId = GetMemberId();
            //    viewModel.ShopMember = db.Query<ShopMember>()
            //        .Where(m => !m.IsDel && m.MemberId == memberId && m.ShopId == shopModel.Id)
            //        .FirstOrDefault();
            //}
            //catch (Exception)
            //{

            //}

            return Success(viewModel);
        }


        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetSingle([FromBody]GetSingleArgs args)
        {
            var viewModel = db.Query<Shop>()
                .Where(m => m.Flag == args.Flag)
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
                    Cover = m.Cover,
                    Logo = m.Logo
                })
                .FirstOrDefault();

            if (viewModel == null) throw new Exception("记录不存在");

            if (viewModel.Latitude.HasValue && viewModel.Longitude.HasValue)
            {
                var TxCoordinate = BaiduMapUtil.CoverCoordinateToTX(viewModel.Latitude.Value, viewModel.Longitude.Value);
                viewModel.Latitude = TxCoordinate.lati;
                viewModel.Longitude = TxCoordinate.logi;
            }
            return Success(viewModel);
        }


        //[HttpGet]
        //public ActionResult Test()
        //{
        //    MemoryStream ms = new MemoryStream();
        //    var bim = CodeHelper.CreateCodeTxmRunBitmap("12312231",  300, 100);
        //    var bytes = CodeHelper.BitmapToArray(bim);
        //    return File(bytes, "image/jpeg");
        //}
    }
}
