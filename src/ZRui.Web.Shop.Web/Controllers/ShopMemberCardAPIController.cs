using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using ZRui.Web.ShopMemberAPIModels;
using System.Linq;
using System.Threading.Tasks;
using ZRui.Web.Models;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.ServerDto;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 会员卡
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class ShopMemberCardAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopMemberCardAPIController(
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
        /// 获取会员卡信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> GetMemberCardInfo([FromBody]ShopIdModel input)
        {
            var shopMemberCardInfo = await db.ShopMemberCardInfo.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);
            if (shopMemberCardInfo == null)
            {
                if (shopMemberCardInfo == null)
                {
                    shopMemberCardInfo = new ShopMemberCardInfo()
                    {
                        AddIp = GetIp(),
                        AddTime = DateTime.Now,
                        CardCover = "http://91huichihuihe.oss-cn-shenzhen.aliyuncs.com/DmHsyw_1536202883673.png",
                        IsValidityLong = true,
                        PrivilegeExplain = "本店会员根据会员级别可在本店享有专属会员折扣，对应福利如下： 普通会员——**折 银卡会员——**折 金卡会员——**折 钻卡会员——**折",
                        ShopId = input.ShopId,
                        ServePhone = "",
                        UsedKnow = "1、每个微信用户仅能申请一张会员卡；2、申请会员卡需要绑定手机号，且一个微信号对应一个手机号，即同一个微信或同一个手机号仅能绑定一张会员卡；3、会员折扣仅限通过本小程序点餐支付方可享受相应折扣；4、会员卡可在本店享受对应会员服务，不能在其他门店享受会员服务5、本店在法律允许范围内保留最终解释权",
                        ValidityBeginTime = DateTime.Now,
                        ValidityEndTime = DateTime.Now

                    };
                    await db.ShopMemberCardInfo.AddAsync(shopMemberCardInfo);
                    await db.SaveChangesAsync();
                    //return await Task.FromResult(Error("未找到记录"));
                }
            }
            return  await Task.FromResult(Success(shopMemberCardInfo));
        }

    }
}
