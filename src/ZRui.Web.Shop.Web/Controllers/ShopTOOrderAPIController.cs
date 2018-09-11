using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using ZRui.Web.ShopTOOrderAPIModels;
using ZRui.Web.BLL.ShopTakeOutInfoExtension;
using ZRui.Web.BLL.MemberAddressExtension;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopTOOrderAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopTOOrderAPIController(
            IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , ILoggerFactory loggerFactory
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }


        /// <summary>
        /// 是否营业中
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        //public APIResult GetShopIsOpen([FromBody] HasTakeOutArgsModels args)
        //{
        //    if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");

        //    var model = db.GetSingle<Shop>(args.ShopId.Value);
        //    if (model == null) throw new Exception("记录不存在");
        //    return Success(model.IsOpen);
        //}



        /// <summary>
        /// 是否有外卖功能
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult HasTakeOut([FromBody] HasTakeOutArgsModels args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopID不能为空");
            bool result = false;

            var toInfo = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();

            if (toInfo != null)
                result = true;

            return Success(result);
        }


        /// <summary>
        /// 添加配送地址
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult AddAddress([FromBody]AddAddressArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopId不能为空");
            if (string.IsNullOrEmpty(args.Name)) throw new Exception("姓名不能为空");
            if (string.IsNullOrEmpty(args.Detail)) throw new Exception("详细地址不能为空");
            if (string.IsNullOrEmpty(args.Phone)) throw new Exception("手机号码不能为空");
            int memberId = GetMemberId();
            var model = new MemberAddress()
            {
                Name = args.Name,
                MemberId = memberId,
                Detail = args.Detail,
                Sex = args.Sex,
                Province = args.Province,
                City = args.City,
                Area = args.Area,
                IsUsed = true,
                Phone = args.Phone,
                AddIp = GetIp(),
                AddTime = DateTime.Now
            };


            CheckIsInScope(model, args.ShopId.Value, args.IsConglomeration.Equals(2));

            var query = db.Query<MemberAddress>()
                 .Where(m => m.MemberId == memberId)
                 .Where(m => !m.IsDel && m.IsUsed)
                 .FirstOrDefault();
            if (query != null) query.IsUsed = false;

            db.AddTo(model);
            db.SaveChanges();
            return Success();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult ReGeocoder([FromBody]ReGeocoderArgsModel args)
        {
            if (!args.lat.HasValue) throw new Exception("lat不能为空");
            if (!args.lng.HasValue) throw new Exception("lng不能为空");
            var latlng = BLL.BaiduMapUtil.CoverCoordinateToBaidu(args.lat.Value, args.lng.Value);
            var result = BLL.BaiduMapUtil.GetReGeoCoder(latlng.lat, latlng.lng);
            return Success(result);
        }


        /// <summary>
        /// 修改配送地址
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetAddress([FromBody]AddAddressArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopId不能为空");
            if (string.IsNullOrEmpty(args.Name)) throw new Exception("姓名不能为空");
            if (string.IsNullOrEmpty(args.Detail)) throw new Exception("详细地址不能为空");
            if (string.IsNullOrEmpty(args.Phone)) throw new Exception("手机号码不能为空");
            int memberId = GetMemberId();

            var model = db.GetSingle<MemberAddress>(args.Id);

            if (model == null) throw new Exception("记录不存在");

            model.Name = args.Name;
            model.Detail = args.Detail;
            model.Phone = args.Phone;
            model.Sex = args.Sex;
            model.Province = args.Province;
            model.City = args.City;
            model.Area = args.Area;

            CheckIsInScope(model, args.ShopId.Value, args.IsConglomeration.Equals(2));

            db.SaveChanges();
            return Success();
        }

        /// <summary>
        /// 检查是否在配送范围
        /// </summary>
        /// <param name="model"></param>
        /// <param name="shopId"></param>
        /// <param name="IsConglomeration">是否为拼团添加地址</param>
        void CheckIsInScope(MemberAddress model, int shopId, bool IsConglomeration)
        {
            string address = model.GetAddress();
            if (model.GetCoordinates())
            {
                if (!IsConglomeration)
                {
                    model.CheckIsInScope(db, shopId);
                }
            }
            else
            {
                throw new Exception("当前地址无法获取相应的坐标");
            }
        }

        /// <summary>
        /// 获取默认配送地址
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetUsingAddress()
        {
            int memberId = GetMemberId();

            var model = db.Query<MemberAddress>()
                .Where(m => m.MemberId == memberId)
                .Where(m => !m.IsDel && m.IsUsed)
                .FirstOrDefault();

            return Success(model);
        }

        /// <summary>
        /// 设置使用中
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetAddressIsUsed([FromBody] SetIsUsedArgs args)
        {
            if (!args.shopId.HasValue) throw new Exception("ShopId不能为空");
            int memberId = GetMemberId();
            var model = db.Query<MemberAddress>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.id)
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();
            if (model == null) throw new Exception("记录不存在");
            if (!model.SetAddressIsUsed(db, args.shopId.Value, memberId, args.IsConglomeration.Equals(1)))
                throw new Exception("当前地址不在配送范围内");
            return Success();
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetAddressIsDelete([FromBody] SetIsUsedArgs args)
        {
            int memberId = GetMemberId();

            var model = db.Query<MemberAddress>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == args.id)
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();

            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }


        /// <summary>
        /// 商铺外卖信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetTakeOutInfo([FromBody] HasTakeOutArgsModels args)
        {
            if (!args.ShopId.HasValue) throw new Exception("shopId为空");

            var model = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .FirstOrDefault();

            return Success(new GetTakeOutInfoModel()
            {
                IsUseTakeOut = model.IsUseTakeOut,
                BoxFee = model.BoxFee / 100d,
                DeliveryFee = model.DeliveryFee / 100d,
                IsOpen = model.IsOpen,
                MinAmount = model.MinAmount / 100d
            });
        }


        /// <summary>
        /// 配送地址列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetAddressList()
        {
            int memberId = GetMemberId();

            var list = db.Query<MemberAddress>()
                .Where(m => m.MemberId == memberId)
                .Where(m => !m.IsDel)
                .ToList();

            return Success(list);
        }

        /// <summary>
        /// 是否在配送范围之内
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult IsInScope([FromBody]IsInScopeArgsModels args)
        {
            if (!args.shopId.HasValue) throw new Exception("shopid不能为空");
            if (!args.lat.HasValue) throw new Exception("纬度不能为空");
            if (!args.lng.HasValue) throw new Exception("经度不能为空");

            var shop = db.GetSingle<Shop>(args.shopId.Value);

            var toInfo = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.shopId.Value)
                .FirstOrDefault();
            if (toInfo == null) return Success(false);
            return Success(toInfo.IsInScope(shop, args.lat.Value, args.lng.Value));
        }


        /// <summary>
        /// 是否在配送范围之内
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<ShopTakeOutStatus> CanUsed([FromBody]CanUsedArgsModels args)
        {
            if (!args.shopId.HasValue) throw new Exception("shopid不能为空");

            var shop = db.GetSingle<Shop>(args.shopId.Value);

            var toInfo = db.Query<ShopTakeOutInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.shopId.Value)
                .FirstOrDefault();
            if (toInfo == null) throw new Exception("抱歉，商家未启动外卖功能");
            return Success(toInfo.CanUsed(shop));
        }


        //[HttpPost]
        //public APIResult AddMenu(string rt)
        //{
        //    string accessToken = AccessTokenContainer.GetAccessToken("wx8bd82e194d0f3d52");
        //    CommonApi.DeleteMenu(accessToken);
        //    ButtonGroup bg = new ButtonGroup();
        //    bg.button.Add(new SingleViewButton()
        //    {
        //        name = "最新订单",
        //        type = ButtonType.view.ToString(),
        //        url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx8bd82e194d0f3d52&redirect_uri=http://manager.91huichihuihe.com/api/ShopOrderSetAPI/Manager/LastShopOrderView?response_type={rt}&scope=snsapi_base#wechat_redirect"
        //    });
        //    var result = CommonApi.CreateMenu(accessToken, bg);
        //    return Success(result);
        //}

    }
}
