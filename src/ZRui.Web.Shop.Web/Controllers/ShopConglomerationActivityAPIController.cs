using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using ZRui.Web.ShopCommodityComboAPIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ZRui.Web.Models;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.ServerDto;
using ZRui.Web.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopConglomerationActivityAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ShopConglomerationOrderOptions _shopConglomerationOrderServer;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment hostingEnvironment;
        ILogger _logger;
        public ShopConglomerationActivityAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , IOptions<ShopConglomerationOrderOptions> shopConglomerationOrderServer
            , ShopDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IMapper mapper
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            _mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;
            _shopConglomerationOrderServer = shopConglomerationOrderServer.Value;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }


        /// <summary>
        /// 获取拼团类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetConglomerationTypeDetails([FromBody] ModelBase input)
        {
            var conglomerationActivityTypes = db.ConglomerationActivityType.Where(m => !m.IsDel && m.ConglomerationActivityId.Equals(input.Id)).AsNoTracking().ToList();
            var conglomerationActivityTypeModel = _mapper.Map<List<ConglomerationActivityTypeModel>>(conglomerationActivityTypes);
            return Success(conglomerationActivityTypeModel);
        }

        /// <summary>
        /// 获取活动详情（不含拼团类型）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetConglomerationActivityDetails([FromBody] ModelBase input)
        {
            var conglomerationActivity = db.ConglomerationActivity.Find(input.Id);
            conglomerationActivity.BrowseNumber = conglomerationActivity.BrowseNumber + 1;
            db.SaveChanges();
            conglomerationActivity.ConglomerationActivityTypes = db.ConglomerationActivityType.Where(m => m.ConglomerationActivityId.Equals(conglomerationActivity.Id)).AsNoTracking().ToList();
            if (conglomerationActivity.ConglomerationActivityTypes.Count == 0)
            {
                return Error("活动无参团类型");
            }
            var conglomerationActivityPickingSettings = db.ConglomerationActivityPickingSetting.Where(m =>
            m.ConglomerationActivityId.Equals(input.Id) &&
            !m.IsDel).AsNoTracking().ToDictionary(m => m.PickingSettingName, n => n.Type);
            if (conglomerationActivity == null || conglomerationActivity.IsDel)
            {
                return Error("记录不存在");
            }
            var activityModel = _mapper.Map<ActivityModel>(conglomerationActivity);
            activityModel.ConglomerationActivityPickingSettings = conglomerationActivityPickingSettings;


            return Success(activityModel);
        }


        /// <summary>
        /// 获取发起拼团订单详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetInitiateConglomerationOrderDetails([FromBody] ModelBase input)
        {
            var conglomerationActivityType = db.ConglomerationActivityType.Find(input.Id);
            if (conglomerationActivityType.IsDel)
            {
                return Error("记录不存在");
            }
            var conglomerationActivity = db.ConglomerationActivity.Find(conglomerationActivityType.ConglomerationActivityId);
            var activityPickingSettings = db.ConglomerationActivityPickingSetting.Where(m =>
            m.ConglomerationActivityId.Equals(conglomerationActivityType.ConglomerationActivityId) &&
            !m.IsDel);

            var getInitiateConglomerationOrderDetailsResultModel = new GetInitiateConglomerationOrderDetailsResultModel();

            getInitiateConglomerationOrderDetailsResultModel.CoverPortal = conglomerationActivity.CoverPortal;
            getInitiateConglomerationOrderDetailsResultModel.CoverPortal = conglomerationActivity.CoverPortal;
            getInitiateConglomerationOrderDetailsResultModel.ActivityName = conglomerationActivity.ActivityName;
            getInitiateConglomerationOrderDetailsResultModel.ConglomerationPriceM = conglomerationActivityType.ConglomerationPrice / 100.00M;
            getInitiateConglomerationOrderDetailsResultModel.PayPrice = getInitiateConglomerationOrderDetailsResultModel.ConglomerationPriceM;
            getInitiateConglomerationOrderDetailsResultModel.DeliveryTakeTheirBeginTimeMD = conglomerationActivity.DeliveryTakeTheirBeginTimeMD;
            getInitiateConglomerationOrderDetailsResultModel.DeliveryTakeTheirEndTimeMD = conglomerationActivity.DeliveryTakeTheirEndTimeMD;
            getInitiateConglomerationOrderDetailsResultModel.DeliveryTakeTheirBeginTimeHM = conglomerationActivity.DeliveryTakeTheirBeginTimeHM;
            getInitiateConglomerationOrderDetailsResultModel.DeliveryTakeTheirEndTimeHM = conglomerationActivity.DeliveryTakeTheirEndTimeHM;
            getInitiateConglomerationOrderDetailsResultModel.PickingSetting = activityPickingSettings.ToDictionary(key => key.Type, value => value.PickingSettingName);
            getInitiateConglomerationOrderDetailsResultModel.ActivityDeliveryFeeM = conglomerationActivity.ActivityDeliveryFee / 100.00M;

            return Success(getInitiateConglomerationOrderDetailsResultModel);

        }

        /// <summary>
        /// 创建拼团
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult CreateConglomeration([FromBody] ConglomerationSetUpModel input)
        {
            var conglomerationActivityType = db.ConglomerationActivityType.Find(input.ConglomerationActivityTypeId);
            var conglomerationActivity = db.ConglomerationActivity.Find(input.ConglomerationActivityId);
            if (conglomerationActivityType.IsDel || conglomerationActivity.IsDel)
            {
                return Error("记录不存在");
            }

            if (conglomerationActivity.ActivityEndTime <= DateTime.Now)
            {
                return Error("活动已经结束");
            }
            var nowDate = DateTime.Now;
            var strat = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 22, 00, 00);
            var end = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 00, 00, 00);
            if (!(nowDate < strat && nowDate > end))
            {
                return Error("参团时间必须为0点-22点");
            }
            input.CurrentMemberNumber = 0;
            input.MemberNumber = conglomerationActivityType.ConglomerationMembers;
            input.Status = ConglomerationSetUpStatus.未支付;
            input.EndTime = DateTime.Now.AddMinutes(conglomerationActivity.ConglomerationCountdown);
            input.MemberId = GetMemberId();
            //input.MemberId = 70;

            var conglomerationSetUp = _mapper.Map<ConglomerationSetUp>(input);
            conglomerationSetUp = db.ConglomerationSetUp.Add(conglomerationSetUp).Entity;
            db.SaveChanges();


            //生成订单
            ShopConglomerationOrderDto shopConglomerationOrderDto = new ShopConglomerationOrderDto()
            {
                AddIp = GetIp(),
                ConglomerationSetUpId = conglomerationSetUp.Id,
                ConglomerationSetUp = conglomerationSetUp,
                Delivery = input.Delivery,
                MemberAddressId = input.MemberAddressId,
                //MemberId = 70,
                MemberId = GetMemberId(),
                ShopId = input.ShopId,
                Type = input.Type,
                FormId = input.FormId

            };
            var order = _shopConglomerationOrderServer.Create(db, shopConglomerationOrderDto);

            return Success(new
            {
                ConglomerationOrderId = order.Id,
                ConglomerationSetUpId = conglomerationSetUp.Id
            });
        }


        /// <summary>
        /// 记录曝光量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult RecordBrowseNumber([FromBody] ModelBase input)
        {
            var conglomerationActivity = db.ConglomerationActivity.Find(input.Id);
            conglomerationActivity.BrowseNumber = conglomerationActivity.BrowseNumber + 1;
            db.SaveChanges();
            return Success();
        }


        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public APIResult GetActivityPageList([FromBody]ActivityPageList input)
        {
            var query = db.ConglomerationActivity.Where(m =>
                !m.IsDel &&
                m.ConglomerationActivityStatut == ConglomerationActivityStatut.已发布 &&
                //m.ActivityEndTime > DateTime.Now &&
                m.ShopId.Equals(input.ShopId)).AsNoTracking()
                .ToPagedList(input.PageIndex, input.PageSize);

            List<GetActivityPageListResultModel> result = new List<GetActivityPageListResultModel>();
            var ActivityPageListREsult = query.ToList().Select(m => _mapper.Map<GetActivityPageListResultModel>(m))
              ;
            foreach (var item in ActivityPageListREsult)
            {

                var conglomerationActivityType = db.ConglomerationActivityType.Where(m => m.ConglomerationActivityId.Equals(item.Id));
                if (conglomerationActivityType.Count() != 0)
                {
                    item.ConglomerationLowestPrice = conglomerationActivityType.Min(m => m.ConglomerationPrice) / 100.00M;
                    item.ConglomerationToptPrice = conglomerationActivityType.Max(m => m.ConglomerationPrice) / 100.00M;
                    item.Participants = db.ConglomerationParticipation.Where(m => !m.IsDel && m.ConglomerationActivityId.Equals(item.Id)).Count();

                }
                result.Add(item);
            }

            //排序
            List<GetActivityPageListResultModel> result2 = result.Where(m => DateTime.Now > m.ActivityEndTime).ToList();
            List<GetActivityPageListResultModel> result3 = result.Where(m => DateTime.Now < m.ActivityEndTime).OrderByDescending(m => m.ActivityBeginTime).ToList();
            result3.AddRange(result2);
            return Success(new
            {
                query.PageIndex,
                query.PageSize,
                TotalCount = query.TotalItemCount,
                Items = result3
            });

        }


        /// <summary>
        /// 参加拼团
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Participation([FromBody]ParticipationModel input)
        {

            var conglomerationSetUp = db.ConglomerationSetUp.Find(input.ConglomerationSetUpId);
            if (conglomerationSetUp == null || conglomerationSetUp.IsDel)
            {
                return Error("记录不存在");
            }
            if (conglomerationSetUp.MemberId.Equals(GetMemberId()))
            {
                return Error("不能参加自己的拼团");
            }

            var conglomerationActivity = db.ConglomerationActivity.Find(conglomerationSetUp.ConglomerationActivityId);
            if (conglomerationActivity.ActivityEndTime <= DateTime.Now)
            {
                return Error("活动已经结束");
            }


            //开始执行逻辑
            if (conglomerationSetUp.EndTime <= DateTime.Now ||
                conglomerationSetUp.CurrentMemberNumber.Equals(conglomerationSetUp.MemberNumber) ||
                conglomerationSetUp.Status.Equals(ConglomerationSetUpStatus.已经成团))
            {
                return Error("该拼团已经结束");
            }
            var conglomerationParticipations = db.ConglomerationParticipation.Where(m => !m.IsDel && m.ConglomerationSetUpId.Equals(input.ConglomerationSetUpId));
            if (conglomerationParticipations.Count() >= conglomerationSetUp.MemberNumber)
            {
                return Error("该拼团已经结束");
            }
            //获取请求集合
            var list = ShopConglomerationActivityOptions.GetSetupIdAndMemberId(conglomerationSetUp.Id, _logger);
            if ((conglomerationSetUp.MemberNumber - conglomerationSetUp.CurrentMemberNumber) <= list.Count())
            {
                //判断是否还可以加入集合，如果不可以则提示稍后重试
                return Error("请稍后再试");
            }
            //否则加入集合
            ShopConglomerationActivityOptions.AddList(list, new ShopConglomerationActivityOptions.RequestModel() { CreateTime = DateTime.Now, MmeberId = GetMemberId() });
            _logger.LogInformation($"==============================添加到请求集合用户ID{GetMemberId()}===============");

            var conglomerationActivityType = db.ConglomerationActivityType.Find(conglomerationSetUp.ConglomerationActivityTypeId);

            //生成订单
            var memberId = GetMemberId();
            var memberAddress = db.Query<MemberAddress>().FirstOrDefault(m => m.Id.Equals(input.MemberAddressId));
            if (memberAddress == null)
            {
                memberAddress = db.Query<MemberAddress>().FirstOrDefault(m => m.IsUsed);
            }
            ShopConglomerationOrderDto shopConglomerationOrderDto = new ShopConglomerationOrderDto()
            {
                AddIp = GetIp(),
                ConglomerationSetUpId = conglomerationSetUp.Id,
                ConglomerationSetUp = conglomerationSetUp,
                MemberAddressId = input.MemberAddressId,
                //MemberId = 70,
                MemberId = memberId,
                ShopId = input.ShopId,
                Type = input.Type,
                Name = memberAddress.Name,
                Phone = memberAddress.Phone,
                Delivery = input.Delivery,
                FormId = input.FormId

            };
            var order = _shopConglomerationOrderServer.Create(db, shopConglomerationOrderDto);

            return Success(new { ConglomerationOrderId = order.Id });
        }


        /// <summary>
        /// 获取可参与拼团列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>s
        [HttpPost]
        public APIResult GetConglomerationSetUp([FromBody]  GetConglomerationSetUpModel input)
        {

            var query = db.ConglomerationSetUp
                  .Where(m => m.ConglomerationActivityId.Equals(input.ConglomerationActivityId))
                  .Where(m => m.EndTime > DateTime.Now && m.Status.Equals(ConglomerationSetUpStatus.未成团))

                  .Include(m => m.ConglomerationParticipations)
                  .Include(m => m.ConglomerationActivityType)
                  .AsNoTracking()
                  .ToPagedList(input.PageIndex, input.PageSize);

            return Success(new
            {
                query.PageIndex,
                query.PageSize,
                TotalCount = query.TotalItemCount,
                Items = _mapper.Map<List<GetConglomerationSetUpResultModel>>(query.ToList())
            });

        }


        /// <summary>
        /// 获取已参与拼团的详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetConglomerationSetUpDetails([FromBody]  GetConglomerationSetUpDetailsRequetModel input)
        {
            _logger.LogInformation($"=====================获取已参与拼团的详细信息开始======================");
            try
            {
                var order = db.ConglomerationOrder.Find(input.OrderId);
                if (order == null)
                {
                    return Error("记录不存在");
                }
                var conglomerationSetUp = db.ConglomerationSetUp.Find(order.ConglomerationSetUpId);
                var conglomerationParticipations = db.ConglomerationParticipation
                    .Where(m => !m.IsDel && m.ConglomerationSetUpId.Equals(order.ConglomerationSetUpId)).ToList();
                conglomerationSetUp.ConglomerationParticipations = conglomerationParticipations;
                conglomerationSetUp.ConglomerationActivity = db.ConglomerationActivity.Find(conglomerationSetUp.ConglomerationActivityId);

                var result = _mapper.Map<GetConglomerationSetUpDetailsResultModel>(conglomerationSetUp);

                result.ConglomerationPriceM = db.ConglomerationActivityType.Find(conglomerationSetUp.ConglomerationActivityTypeId).ConglomerationPrice / 100.00M;
                result.ConglomerationOrderId = input.OrderId;
                result.ConglomerationActivityTypeId = conglomerationSetUp.ConglomerationActivityTypeId;
                _logger.LogInformation($"=====================获取已参与拼团的详细信息结束======================");

                return Success(result);

            }
            catch (Exception e)
            {
                _logger.LogInformation($"获取已参与拼团的详细信息:{e}");
                _logger.LogInformation($"=====================获取已参与拼团的详细信息结束======================");
                return Success(e);
            }
        }

        /// <summary>
        /// 取消支付
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult CancelThePayment([FromBody]CancelThePayment input)
        {
            ShopConglomerationActivityOptions.NotifyOkRemoveList(input.ConglomerationOrderId, db, _logger);
            return Success();
        }

        /// <summary>
        /// 检查已发起的拼团
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult CheckConglomerationSetUp([FromBody]ParticipationModel input)
        {
            var conglomerationSetUp = db.ConglomerationSetUp.Find(input.ConglomerationSetUpId);
            if (conglomerationSetUp == null)
            {
                return Error("记录不存在");
            }
            if (conglomerationSetUp.EndTime < DateTime.Now)
            {
                return Error("该拼团已结束");
            }
            var conglomerationActivity = db.ConglomerationActivity.Find(conglomerationSetUp.ConglomerationActivityId);
            if (conglomerationActivity.ActivityEndTime <= DateTime.Now)
            {
                return Error("活动已经结束");
            }

            return Success();
        }



        /// <summary>
        /// 获取小程序分享二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWxaCodeUnlimit(int shopId, int conglomerationActivityId)
        {
            var token = ShopConglomerationOrderOptions.GetAuthorizerAccessToken(db, shopId);
            var dirPath = hostingEnvironment.ContentRootPath + "\\wwwroot\\GetWxaCodeUnlimit\\";
            var path = Path.Combine(dirPath, $"{shopId}_{conglomerationActivityId}.jpg");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (!System.IO.File.Exists(path))
            {
                var jasonresult = WxAppApi.GetWxaCodeUnlimit(token, path, $"{conglomerationActivityId}", "pages/home/OrderDetailsinfo", 210);
            }
            return File($"\\GetWxaCodeUnlimit\\{shopId}_{conglomerationActivityId}.jpg", "image/png");

        }



        /// <summary>
        /// 获取小程序已成团分享二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWxaCodeSetUp(int shopId, int conglomerationOrderId)
        {
            var token = ShopConglomerationOrderOptions.GetAuthorizerAccessToken(db, shopId);
            var dirPath = hostingEnvironment.ContentRootPath + "\\wwwroot\\GetWxaCodeSetUp\\";
            var path = Path.Combine(dirPath, $"{shopId}_{conglomerationOrderId}.jpg");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (!System.IO.File.Exists(path))
            {
                var jasonresult = WxAppApi.GetWxaCodeUnlimit(token, path, $"{conglomerationOrderId}", "pages/home/payinfo", 210);
            }
            return File($"\\GetWxaCodeSetUp\\{shopId}_{conglomerationOrderId}.jpg", "image/png");

        }



        /// <summary>
        /// 下载小程序分享二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DownloadWxaCodeUnlimit(int shopId, int conglomerationActivityId)
        {
            var token = ShopConglomerationOrderOptions.GetAuthorizerAccessToken(db, shopId);
            var dirPath = hostingEnvironment.ContentRootPath + "\\wwwroot\\WxopenCode\\";
            var path = Path.Combine(dirPath, $"{shopId}_{conglomerationActivityId}.jpg");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (!System.IO.File.Exists(path))
            {
                var jasonresult = WxAppApi.GetWxaCodeUnlimit(token, path, $"{conglomerationActivityId}", "pages/home/OrderDetailsinfo", 210);
            }
            return File($"\\WxopenCode\\{shopId}_{conglomerationActivityId}.jpg", "image/png", "qr-async.jpg");

        }
    }
}
