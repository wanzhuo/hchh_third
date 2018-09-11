using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.BLL.ServerDto;

namespace ZRui.Web.Controllers
{

    /// <summary>
    /// 会员等级设置
    /// </summary>
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopMemberCardController : ShopManagerApiControllerBase
    {

        readonly IHostingEnvironment hostingEnvironment;
        private IMapper _mapper { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopMemberCardController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }



        /// <summary>
        /// 更新会员卡设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> UpdateMemberCardInfo([FromBody]ShopMemberCardInfoModel input)
        {
            input.AddIp = GetIp();
            var shopMemberCardInfo = await db.ShopMemberCardInfo.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);
            if (shopMemberCardInfo == null)
            {
                var addshopMemberCardInfo = _mapper.Map<ShopMemberCardInfo>(input);
                await db.ShopMemberCardInfo.AddAsync(addshopMemberCardInfo);
            }
            else
            {
                shopMemberCardInfo.IsValidityLong = input.IsValidityLong;
                shopMemberCardInfo.PrivilegeExplain = input.PrivilegeExplain;
                shopMemberCardInfo.UsedKnow = input.UsedKnow;
                shopMemberCardInfo.ValidityBeginTime = input.ValidityBeginTime;
                shopMemberCardInfo.ValidityEndTime = input.ValidityEndTime;
                shopMemberCardInfo.CardCover = input.CardCover;
                shopMemberCardInfo.ServePhone = input.ServePhone;
            }
            await db.SaveChangesAsync();
            return await Task.FromResult(Success());
        }


        /// <summary>
        /// 获取会员卡设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> GetMemberCardInfo([FromBody]GetPagedListBaseModel input)
        {
            var shopMemberCardInfo = await db.ShopMemberCardInfo.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);

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
            return await Task.FromResult(Success(shopMemberCardInfo));
        }
    }
}
