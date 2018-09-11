using System;
using System.Linq;
using ZRui.Web.CommercialDistrictAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using static ZRui.Web.Common.Geohash;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CommercialDistrictAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public CommercialDistrictAPIController(ICommunityService communityService
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
            return new List<string>() { geohash, geohashTop, geohashBottom, geohashLeft, geohashRight,geohashTopLeft, geohashTopRight, geohashBottomLeft, geohashBottomRight };
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<CommercialDistrict>()
                     .Where(m => !m.IsDel);

            if (args.Latitude.HasValue && args.Longitude.HasValue)
            {
                //6 范围在±0.61KM
                if (!args.Precision.HasValue) args.Precision = 6;
                var geohash = Geohash.Encode(args.Latitude.Value, args.Longitude.Value, args.Precision.Value);
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
                    Detail = m.Detail,
                    IsDel = m.IsDel,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Name = m.Name
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
    }
}
