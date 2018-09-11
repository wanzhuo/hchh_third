using System;
using System.Linq;
using ZRui.Web.ShopPayInfoSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopPayInfoSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopPayInfoSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]GetSingleArgsModel args)
        {
            var model = db.Query<ShopPayInfo>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.id)
                     .FirstOrDefault();

            if (model == null) return Success(null);

            bool HasSecretKey = (model.SecretKey != null && model.SecretKey.Length > 0);
            return Success(new
            {
                model.Id,
                model.ShopId,
                model.PayWay,
                model.MchId,
                HasSecretKey
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult SelectShopPayInfoGetSingleByPayWay([FromBody]GetPayWayArgsModel args)
        {
            var model = db.ShopPayInfo.FirstOrDefault(r => r.ShopId == args.id && r.PayWay == args.payway);
            if (model != null && args.payway == PayWay.Swiftpass)
            {
                var swiftpasskey = db.SwiftpassKey.FirstOrDefault(r => r.ShopFlag == model.ShopFlag);
                if (swiftpasskey != null)
                {
                    return Success(new SetPayInfoSwiftpassArgsModel()
                    {
                        isenable = swiftpasskey.IsEnable,
                        mchid = model.MchId,
                        notify = swiftpasskey.Notify,
                        prviateKey = swiftpasskey.PrviateKey,
                        publicKey = swiftpasskey.PublicKey,
                        reqUrl = swiftpasskey.ReqUrl,
                        flage = "swiftpasskey"
                    });
                }
            }
            else if (model != null && args.payway == PayWay.Wechat)
            {
                return Success(new SetPayInfoSwiftpassArgsModel()
                {
                    isenable = model.IsEnable,
                    mchid = model.MchId,
                    secretkey = model.SecretKey,
                    flage = "wechat"
                });
            }
            return Success(model);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetPayInfo([FromBody]SetPayInfoArgsModel args)
        {
            if (!args.shopid.HasValue) throw new ArgumentNullException("shopid");
            var model = db.Query<ShopPayInfo>()
               .Where(m => !m.IsDel)
               .Where(m => m.ShopId == args.shopid.Value && m.PayWay == args.payway)
               .FirstOrDefault();

            var isenablecount = db.Query<ShopPayInfo>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.shopid.Value && m.IsEnable == true).FirstOrDefault();
            //PayWay payWay = Enum.Parse<PayWay>(args.payway.ToString());

            if (isenablecount != null)
            {
                if (isenablecount.PayWay != args.payway && args.isenable)
                {
                    throw new Exception("一个商户同时只能开启一个支付通道！");
                }
            }
            //微信支付
            if (args.payway == PayWay.Wechat)
            {

                if (string.IsNullOrEmpty(args.mchid)) throw new ArgumentNullException("商户号");
                if (string.IsNullOrEmpty(args.secretkey)) throw new ArgumentNullException("密钥");
                ExPayInfo(args, model);

            }
            else if (args.payway == PayWay.Swiftpass)
            {
                ExPayInfo(args, model);
                model = db.Query<ShopPayInfo>()
              .Where(m => !m.IsDel)
              .Where(m => m.ShopId == args.shopid.Value && m.PayWay == args.payway)
              .FirstOrDefault();
                //中信支付
                var swifpasskey = db.SwiftpassKey.FirstOrDefault(r => r.ShopFlag == model.ShopFlag);
                if (swifpasskey == null)
                {
                    SwiftpassKey key = new SwiftpassKey()
                    {
                        ShopFlag = model.ShopFlag,
                        ReqUrl = args.reqUrl,
                        PublicKey = args.publicKey,
                        PrviateKey = args.prviateKey,
                        Notify = args.notify,
                        IsEnable = args.isenable
                    };
                    db.Add(key);
                    db.SaveChanges();
                }
                else
                {
                    if (string.IsNullOrEmpty(args.reqUrl) || string.IsNullOrEmpty(args.publicKey)
                        || string.IsNullOrEmpty(args.prviateKey) || string.IsNullOrEmpty(args.notify)
                        )
                    {
                        throw new Exception("配置信息必须全部配置");
                    }

                    swifpasskey.ShopFlag = model.ShopFlag;
                    swifpasskey.ReqUrl = args.reqUrl;
                    swifpasskey.PublicKey = args.publicKey;
                    swifpasskey.PrviateKey = args.prviateKey;
                    swifpasskey.Notify = args.notify;
                    swifpasskey.IsEnable = args.isenable;
                    model.IsEnable = args.isenable;
                    db.Update(swifpasskey);
                }
            }

            db.SaveChanges();
            return Success();
        }

        private void ExPayInfo(SetPayInfoArgsModel args, ShopPayInfo model)
        {
            if (model == null)
            {

                ShopPayInfo shopPayInfo = new ShopPayInfo()
                {
                    ShopId = args.shopid.Value,
                    MchId = args.mchid,
                    SecretKey = args.secretkey,
                    PayWay = args.payway,
                    IsEnable = args.isenable
                };
                string shopFlag = db.Query<Shop>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Id == args.shopid.Value)
                    .Select(m => m.Flag)
                    .FirstOrDefault();

                string appid = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == args.shopid.Value)
                    .Select(m => m.WechatOpenAuthorizer)
                    .Select(m => m.AuthorizerAppId)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(appid))
                    throw new Exception("请先在商户后台进行授权");

                shopPayInfo.ShopFlag = shopFlag;
                shopPayInfo.AppId = appid;
                shopPayInfo.AddTime = DateTime.Now;
                db.AddTo<ShopPayInfo>(shopPayInfo);
                db.SaveChanges();
            }
            else
            {
                model.PayWay = args.payway;
                model.MchId = args.mchid;
                model.IsEnable = args.isenable;        //args.isenable;
                string appid = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == args.shopid.Value)
                    .Select(m => m.WechatOpenAuthorizer)
                    .Select(m => m.AuthorizerAppId)
                    .FirstOrDefault();
                model.AppId = appid;
                Regex regex = new Regex(@"^\*+$");
                if (!regex.IsMatch(args.secretkey))
                {
                    model.SecretKey = args.secretkey;
                }
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            //TODO:这里还需要判定是否可以操作当前店铺
            var model = db.GetSingle<ShopPayInfo>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }


    }
}
