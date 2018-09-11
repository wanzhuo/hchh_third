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
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Template;
using Microsoft.Extensions.Logging;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using ZRui.Web.BLL.Servers;
using Senparc.Weixin;
using System.DrawingCore;
using static System.Net.WebRequestMethods;
using System.IO;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 拼团活动API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopConglomerationActivityAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        private IMapper _mapper { get; set; }
        ILogger _logger;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopConglomerationActivityAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions
            , IMapper mapper
           , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
            _logger = loggerFactory.CreateLogger<ShopConglomerationOrderAPIController>();
            _mapper = mapper;
        }
        /// <summary>
        /// 添加活动
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        [Authorize]
        public async Task<APIResult> AddActivity([FromBody]ActivityModel input)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(input.Context);
            if (byteArray.Length > 1024 * 500)
            {
                return Error("拼团活动内容不能大于500kb");

            }

            if (input.DeliveryTakeTheirEndTimeMD < input.ActivityEndTime)
            {
                return Error("预计自提\\快递结束时间需要比活动时间长");
            }
            var authorizerAccessToken = GetAuthorizerAccessToken(input.ShopId);
            if (input.ActivityEndTime <= DateTime.Now)
            {
                return Error("结束时间月份不能小于当前时间月份");
            }
            #region 添加消息模板
            var shopTemplateMessageInfo = db.ShopTemplateMessageInfo.FirstOrDefault(m => m.ShopId.Equals(input.ShopId) && !m.IsDel && m.Title.Equals("拼团成功通知"));
            if (shopTemplateMessageInfo == null)
            {
                var addJsonResult = await TemplateApi.AddAsync(authorizerAccessToken, "AT0051", new int[] { 1, 3, 13, 16, 32, 34 }); //模板ID和关键字id都是固定的
                var list = await TemplateApi.ListAsync(authorizerAccessToken, 0, 20);
                var tesmp = list.list.FirstOrDefault(m => m.title.Equals("拼团成功通知"));
                if (tesmp != null)
                {
                    await db.ShopTemplateMessageInfo.AddAsync(new ShopTemplateMessageInfo() { Content = tesmp.content, CreateTime = DateTime.Now, Example = tesmp.example, ShopId = input.ShopId, TemplateId = tesmp.template_id, Title = tesmp.title });
                    await db.SaveChangesAsync();
                    _logger.LogInformation($"添加消息模板成功模板ID:{tesmp.template_id}");
                }

            }
            #endregion
            input.ConglomerationActivityStatut = ConglomerationActivityStatut.已发布;
            input.ShopId = input.ShopId;
            var conglomerationActivity = _mapper.Map<ConglomerationActivity>(input);
            conglomerationActivity = db.ConglomerationActivity.Add(conglomerationActivity).Entity;
            db.SaveChanges();
            input.ConglomerationActivityId = conglomerationActivity.Id;
            SetPicking(input, conglomerationActivity);
            return Success(conglomerationActivity);
        }


        /// <summary>
        /// 添加可选拼团类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult AddConglomerationType([FromBody]ConglomerationActivityTypeModel input)
        {
            if (input.ConglomerationMembers < 2)
            {
                return Error("拼团人数不能少于两个人");
            }
            input.TypeDescribe = "此字段不使用";
            var typeCount = db.ConglomerationActivityType.Where(m => m.ConglomerationActivityId.Equals(input.ConglomerationActivityId) && !m.IsDel).Count();
            if (typeCount >= 3)
            {
                return Error("最多添加3种类型");
            }
            var conglomerationActivityType = _mapper.Map<ConglomerationActivityType>(input);
            db.ConglomerationActivityType.Add(conglomerationActivityType);
            db.SaveChanges();
            return Success("添加成功");
        }


        /// <summary>
        /// 获取更新活动信息实体
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetUpdateActivityModel([FromBody]ActivityModel activityModel)
        {
            var actity = db.ConglomerationActivity.FirstOrDefault(m => !m.IsDel && m.Id.Equals(activityModel.Id));
            if (actity == null)
            {
                return Error("记录不存在");
            }

            var conglomerationActivityModel = _mapper.Map<ActivityModel>(actity);
            conglomerationActivityModel.Type = db.ConglomerationActivityPickingSetting.Where(m => m.ConglomerationActivityId.Equals(activityModel.Id) && !m.IsDel).Select(m => (int)m.Type).ToArray();
            return Success(conglomerationActivityModel);
        }


        /// <summary>
        ///发布活动
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult UpdateConglomerationActivityStatut([FromBody]ActivityModel activityModel)
        {
            var actity = db.ConglomerationActivity.FirstOrDefault(m => !m.IsDel && m.Id.Equals(activityModel.Id));
            if (actity == null)
            {
                return Error("记录不存在");
            }
            if (actity.ConglomerationActivityStatut == ConglomerationActivityStatut.已发布)
            {
                return Error("活动已经发布，不能修改");

            }

            var actityType = db.ConglomerationActivityType.Where(m => !m.IsDel && m.ConglomerationActivityId.Equals(activityModel.Id));
            if (actityType.Count() == 0)
            {
                return Error("拼团类型数量为0，请先添加活动类型");
            }
            actity.ConglomerationActivityStatut = ConglomerationActivityStatut.已发布;
            db.SaveChanges();
            return Success();

        }


        /// <summary>
        /// 更新活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult UpdateActivityModel([FromBody]ActivityModel input)
        {
            var actity = db.ConglomerationActivity.FirstOrDefault(m => !m.IsDel && m.Id.Equals(input.Id));
            if (actity == null)
            {
                return Error("记录不存在");
            }
            if (actity.ConglomerationActivityStatut == ConglomerationActivityStatut.已发布)
            {
                return Error("活动已经发布，不能修改");

            }
            var conglomerationActivity = _mapper.Map<ConglomerationActivity>(input);

            actity.ActivityName = conglomerationActivity.ActivityName;
            actity.Intro = conglomerationActivity.Intro;
            actity.ActivityBeginTime = conglomerationActivity.ActivityBeginTime;
            actity.ActivityEndTime = conglomerationActivity.ActivityEndTime;
            actity.ActivityEndTime = conglomerationActivity.ActivityEndTime;
            actity.ConglomerationCountdown = conglomerationActivity.ConglomerationCountdown;
            actity.CoverPortal = conglomerationActivity.CoverPortal;
            actity.Context = conglomerationActivity.Context;
            db.SaveChanges();
            input.ConglomerationActivityId = conglomerationActivity.Id;
            SetPicking(input, conglomerationActivity);
            return Success(conglomerationActivity);
        }

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public APIResult GetActivityPageList([FromBody]GetPagedListBaseModel input)
        {
            var query = db.ConglomerationActivity.Where(m =>
                !m.IsDel &&
                //m.ActivityEndTime > DateTime.Now &&
                m.ShopId.Equals(input.ShopId))
                .Include(m => m.ConglomerationActivityTypes)
                .AsNoTracking()
                .ToPagedList(input.PageIndex, input.PageSize);
            return Success(new
            {
                query.PageIndex,
                query.PageSize,
                TotalCount = query.TotalItemCount,
                Items = query.ToList()
            });

        }


        /// <summary>
        /// 获取活动详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetActivityDetails([FromBody]GetActivityDetailsModel input)
        {
            var conglomerationActivity = db.ConglomerationActivity.Find(input.Id);

            var activityModel = new
            {
                conglomerationActivity.Id,
                conglomerationActivity.Intro,
                conglomerationActivity.Context,
                MarketPrice = conglomerationActivity.MarketPrice / 100.00M,
                conglomerationActivity.ActivityEndTime,
                FaHuoQuhuoTime = $"{DateTime.Now.Year}/{conglomerationActivity.DeliveryTakeTheirBeginTimeMD.ToString("MM")}/{conglomerationActivity.DeliveryTakeTheirBeginTimeMD.ToString("dd")} {conglomerationActivity.DeliveryTakeTheirBeginTimeHM.ToString("HH")}:{conglomerationActivity.DeliveryTakeTheirBeginTimeHM.ToString("mm")} -  {DateTime.Now.Year}/{conglomerationActivity.DeliveryTakeTheirEndTimeMD.ToString("MM")}/{conglomerationActivity.DeliveryTakeTheirEndTimeMD.ToString("dd")} {conglomerationActivity.DeliveryTakeTheirEndTimeHM.ToString("HH")}:{conglomerationActivity.DeliveryTakeTheirEndTimeHM.ToString("mm")}",
                ConglomerationActivityPickingSettings = db.ConglomerationActivityPickingSetting.Where(m =>
         m.ConglomerationActivityId.Equals(input.Id) &&
         !m.IsDel).AsNoTracking().Select(m => m.PickingSettingName).ToArray()
            };
            return Success(activityModel);

        }


        [HttpGet]
        //[Authorize]
        public ActionResult GetOAuthUrl(int shopId)
        {
            //CheckShopActor(input.ShopId, ShopActorType.超级管理员);
            var redictUrl = "http://manager.91huichihuihe.com/api/ShopConglomerationActivityAPI/Manager/AddServerUserInfo";
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(wechatTemplateSendOptions.AppId, redictUrl,
                   shopId.ToString(), Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);
            var bitmap = CodeHelper.CreateCodeEwmRuBitmap(result);
            return File(CodeHelper.BitmapToBytes(bitmap), "image/png");
            //return await Task.FromResult(Success(result));
        }


        /// <summary>
        /// 授权后添加客服信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddServerUserInfo(string code, int state)
        {
            string message = "faile";
            try
            {
                _logger.LogInformation($"==================AddServerUserInfo方法开始==================");
                var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(wechatTemplateSendOptions.AppId,
                    wechatTemplateSendOptions.AppSecret, code);
                ShopOrderReceiver model = null;
                Senparc.Weixin.MP.AdvancedAPIs.OAuth.OAuthUserInfo userInfo = null;

                if (result != null && !string.IsNullOrEmpty(result.openid))
                {
                    string openId = result.openid;
                    string access_token = result.access_token;
                    userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(access_token, openId);
                    if (userInfo != null && !string.IsNullOrEmpty(userInfo.openid))
                    {
                        var shopServiceUserInfo = db.ShopServiceUserInfo.FirstOrDefault(
                            m => m.Openid.Equals(userInfo.openid) &&
                            m.Unionid.Equals(userInfo.unionid) &&
                            m.ShopId.Equals(state) &&
                            !m.IsDel
                            );
                        if (shopServiceUserInfo != null)
                        {
                            //已添加授权
                            shopServiceUserInfo.Headimgurl = userInfo.headimgurl;
                            shopServiceUserInfo.Nickname = userInfo.nickname;
                            db.SaveChanges();
                            message = "authored";
                        }
                        else
                        {
                            shopServiceUserInfo = new ShopServiceUserInfo();
                            shopServiceUserInfo.Headimgurl = userInfo.headimgurl;
                            shopServiceUserInfo.Nickname = userInfo.nickname;
                            shopServiceUserInfo.Openid = userInfo.openid;
                            shopServiceUserInfo.Sex = userInfo.sex == 1 ? "男" : "女";
                            shopServiceUserInfo.Province = userInfo.province;
                            shopServiceUserInfo.City = userInfo.city;
                            shopServiceUserInfo.Country = userInfo.country;
                            shopServiceUserInfo.Headimgurl = userInfo.headimgurl;
                            shopServiceUserInfo.Unionid = userInfo.unionid;
                            shopServiceUserInfo.AddTime = DateTime.Now;
                            shopServiceUserInfo.ShopId = state;
                            db.ShopServiceUserInfo.Add(shopServiceUserInfo);
                            db.SaveChanges();
                            message = "success";
                        }
                    }
                }
                _logger.LogInformation($"==================AddServerUserInfo方法结束==================");

            }
            catch (Exception e)
            {
                message = e.Message;
                _logger.LogError($"添加授权客服失败：{ e.Message}  |   { e.InnerException.Message}");
            }
            //ViewData["message"] = message;
            return Json(new { Message = message });
        }



        #region 删除
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public APIResult DelActivity([FromBody]ModelBase input)
        {
            var activity = db.ConglomerationActivity.Find(input.Id);
            activity.IsDel = true;
            db.SaveChanges();
            return Success("删除成功");
        }


        /// <summary>
        /// 删除活动类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public APIResult DelConglomerationType([FromBody]ModelBase input)
        {
            var type = db.ConglomerationActivityType.Find(input.Id);
            type.IsDel = true;
            db.SaveChanges();
            return Success("删除成功");
        }


        #endregion



        /// <summary>
        /// 获取拼团订单核销客服人员列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public async Task<APIResult> GetServerUserList([FromBody]GetPagedListBaseModel input)
        {
            var query = db.ShopServiceUserInfo.Where(m => m.ShopId.Equals(input.ShopId) && !m.IsDel)
                   .OrderByDescending(m => m.AddTime)
                   .AsNoTracking()
                   .ToPagedList(input.PageIndex, input.PageSize);
            return await Task.FromResult(Success(new
            {
                query.PageIndex,
                query.PageSize,
                TotalCount = query.TotalItemCount,
                Items = query.ToList()
            }));

        }

        /// <summary>
        /// 删除订单的客服信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public async Task<APIResult> DelServerUser([FromBody]ModelBase input)
        {
            var shopServiceUserInfo = await db.ShopServiceUserInfo.FirstOrDefaultAsync(m => m.Id.Equals(input.Id) && !m.IsDel);
            if (shopServiceUserInfo == null)
            {
                return await Task.FromResult(Error("未找到指定记录"));
            }
            shopServiceUserInfo.IsDel = true;
            await db.SaveChangesAsync();
            return await Task.FromResult(Success("删除成功"));

        }


        #region 内部函数
        /// <summary>
        /// 配置活动配送方式配送费，配送时间
        /// </summary>
        /// <returns></returns>
        void SetPicking(ActivityModel input, ConglomerationActivity conglomerationActivity)
        {
            if (conglomerationActivity.Id != 0)
            {
                var oldConglomerationActivityPickingSetting = db.ConglomerationActivityPickingSetting.Where(m => m.ConglomerationActivityId.Equals(conglomerationActivity.Id) && !m.IsDel);
                if (oldConglomerationActivityPickingSetting.Count() != 0)
                {
                    foreach (var ConglomerationActivityPickingSettingItem in oldConglomerationActivityPickingSetting)
                    {
                        ConglomerationActivityPickingSettingItem.IsDel = true;
                    }
                    db.SaveChanges();
                }

            }
            conglomerationActivity.DeliveryTakeTheirBeginTimeHM = input.DeliveryTakeTheirBeginTimeHM;
            conglomerationActivity.DeliveryTakeTheirBeginTimeMD = input.DeliveryTakeTheirBeginTimeMD;
            conglomerationActivity.DeliveryTakeTheirEndTimeHM = input.DeliveryTakeTheirEndTimeHM;
            conglomerationActivity.DeliveryTakeTheirEndTimeMD = input.DeliveryTakeTheirEndTimeMD;
            conglomerationActivity.ActivityDeliveryFee = (int)input.ActivityDeliveryFee;

            List<ConglomerationActivityPickingSetting> addList = new List<ConglomerationActivityPickingSetting>();
            foreach (var type in input.Type.Distinct())
            {
                ConglomerationActivityPickingSetting conglomerationActivityPickingSetting = new ConglomerationActivityPickingSetting()
                {
                    ConglomerationActivityId = input.ConglomerationActivityId,
                    Type = (ConsignmentType)type,
                    CreateTime = DateTime.Now

                };
                switch (conglomerationActivityPickingSetting.Type)
                {
                    case ConsignmentType.快递:
                        conglomerationActivityPickingSetting.PickingSettingName = "快递";
                        break;
                    case ConsignmentType.自提:
                        conglomerationActivityPickingSetting.PickingSettingName = "自提";
                        break;
                }

                addList.Add(conglomerationActivityPickingSetting);
            }

            db.ConglomerationActivityPickingSetting.AddRange(addList);
            db.SaveChanges();
        }
        #endregion

        #region 内部使用代码
        string GetAuthorizerAccessToken(int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId,
                    AuthorizerAccessToken = m.WechatOpenAuthorizer.AuthorizerAccessToken,
                    ExpiresTime = m.WechatOpenAuthorizer.ExpiresTime
                })
                .FirstOrDefault();

            if (model == null) throw new Exception("指定的纪录不存在");
            //if (model.ExpiresTime.AddMinutes(20) > DateTime.Now) return model.AuthorizerAccessToken;

            return GetAuthorizerAccessToken(model.AuthorizerAppId);
        }
        string GetAuthorizerAccessToken(string authorizerAppId)
        {
            //  var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken(wechatOpenOptions.AppId, authorizerAppId);
            //  return authorizerAccessToken;
            return ZRui.Web.BLL.AuthorizerHelper.GetAuthorizerAccessToken(authorizerAppId);
        }
        #endregion
    }
}
