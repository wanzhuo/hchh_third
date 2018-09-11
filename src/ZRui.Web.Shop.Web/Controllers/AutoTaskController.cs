using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.WxaAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ZRui.Web.BLL;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.Open;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/autotask/[action]")]
    public class AutoTaskController : ApiControllerBase
    {
        ShopDbContext db;
        WechatOptions wechatOptions;
        FinanceDbContext financeDb;

        readonly IHostingEnvironment hostingEnvironment;

        AUtoTask autotask;

        public AutoTaskController(ShopDbContext db, FinanceDbContext financeDb,
             IHostingEnvironment hostingEnvironment, IOptions<WechatOptions> wechatOptions, ILoggerFactory loggerFactory)
        {
            this.db = db;
            this.wechatOptions = wechatOptions.Value;// new WechatOptions() { AppId= "wx99dc6b0ea873ba0c", AppSecret= "d4f8f1222a94562ac6df4349861086b6" };
            this.hostingEnvironment = hostingEnvironment;
            this.financeDb = financeDb;

            this.autotask = new AUtoTask(db, hostingEnvironment);

        }

        [HttpGet]
        public APIResult AutoTaskTest()
        {
            return Success("ok");
        }

        /// <summary>
        /// 更新取号表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public APIResult UpdateCode()
        {
            db.Query<CodeForShopOrderSelfHelp>()
               .ToList()
               .ForEach(m =>
               {
                   m.CurrentNumber = 0;
               });

            db.SaveChanges();
            DbContextFactory.LogDbContext.AddTaskLog(new TaskLog() { AddTime = DateTime.Now, TaskName = "UpdateCode", ExeResult = "ok" });
            return Success("ok");
        }

        [HttpGet]
        public APIResult UpdateMoneyOffCache()
        {

            using (ShopDbContext db = DbContextFactory.ShopDb)
            {
                DateTime now = DateTime.Now.Date;
                var moneyOffList = db.Query<ShopOrderMoneyOff>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.EndDate >= now)
                    .Where(m => m.StartDate <= now)
                    .ToList();

                var moneyOffCacheList = db.Set<ShopOrderMoneyOffCache>()
                    .Where(m => !m.IsDel);

                var moneyOffFromCache = moneyOffCacheList.Select(m => m.ShopOrderMoneyOff).ToList();

                //新增部分
                List<ShopOrderMoneyOff> inCreased = moneyOffList.Except(moneyOffFromCache).ToList();
                foreach (var item in inCreased)
                {
                    ShopOrderMoneyOffCache model = new ShopOrderMoneyOffCache()
                    {
                        ShopId = item.ShopId,
                        MoneyOffId = item.Id,
                        IsDel = false
                    };
                    db.AddTo(model);
                }

                //减少部分
                var reduceIDs = moneyOffFromCache.Except(moneyOffList).Select(m => m.Id);
                var reduceCacheList = db.Query<ShopOrderMoneyOffCache>()
                    .Where(m => reduceIDs.Contains(m.MoneyOffId)).ToList();
                foreach (var item in reduceCacheList)
                {
                    item.IsDel = true;
                }
                db.SaveChanges();
            }

            DbContextFactory.LogDbContext.AddTaskLog(new TaskLog() { AddTime = DateTime.Now, TaskName = "UpdateMoneyOffCache", ExeResult = "ok" });

            return Success("ok");
        }

        [HttpGet]
        public APIResult SubmitAudit()
        {
            return autotask.SubmitAudit();
        }
        [HttpGet]
        public APIResult SubmitAuditShop(int shopId)
        {
            return autotask.SubmitAuditShop(shopId);
        }

        public APIResult AutoUpgrade(ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            return autotask.AutoUpgrade(item,tempInfo);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="authorizerAccessToken"></param>
        /// <param name="item"></param>
        /// <param name="tempInfo"></param>
        public APIResult SubmitVersion(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            return autotask.SubmitVersion(authorizerAccessToken, item, tempInfo);
        }

        public APIResult QuerySubmit(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            return autotask.QuerySubmit(authorizerAccessToken,item,tempInfo);
        }

        /// <summary>
        /// 发布已通过版本
        /// </summary>
        /// <param name="authorizerAccessToken"></param>
        /// <param name="shopId"></param>
        /// <param name="logDbContext"></param>
        public APIResult ReleaseVersion(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            return autotask.ReleaseVersion(authorizerAccessToken,item,tempInfo);
        }


        [HttpGet]
        /// <summary>
        /// 清理过期未支付的订单，设置为取消
        /// </summary>
        public void HandleOverdueOrder()
        {

            foreach (var order in db.Query<ShopOrder>().Where(p => !p.IsDel && p.Status == ShopOrderStatus.待支付 && p.AddTime < DateTime.Now.AddMinutes(-60)))
            {
                order.Status = ShopOrderStatus.已取消;
            }
            db.SaveChanges();


        }
        [HttpGet]
        public void HandleUpdateRefundStatus()
        {
            var refunds = financeDb.memberTradeForRefunds.ToList();
            foreach (var item in refunds)
            {
                if (item != null)
                {

                }
            }
        }



    }


    public class AUtoTask
    {
        public APIResult Success(string message)
        {
            return new APIResult() { Success= true, Message= message };
        }
        public APIResult Success()
        {
            return new APIResult() { Success = true};
        }

        public APIResult Error(string message)
        {
            return new APIResult() { Success = false, Message = message };
        }
        ShopDbContext db;
        IHostingEnvironment hostingEnvironment;
        public AUtoTask(ShopDbContext db, IHostingEnvironment hostingEnvironment)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }
        public APIResult SubmitAudit()
        {
            var targets = db.ShopWechatOpenAuthorizers.Include(p => p.WechatOpenAuthorizer).Include(p => p.Shop).Where(m => !m.IsDel).ToList();
            if (targets.Count == 0)
            {
                return   Success("not found ShopWechatOpenAuthorizer");
            }
            //取小程序模板列表
            var accessToken = AuthorizerHelper.GetComponentAccessToken();//  ComponentContainer.TryGetComponentAccessToken(wechatOptions.AppId, wechatOptions.AppSecret);
            var tempList = CodeTemplateApi.GetTemplateListAsync(accessToken).Result;
            var tempInfo = tempList.template_list[tempList.template_list.Count - 1];

            foreach (var item in targets)
            {
                AutoUpgrade(item, tempInfo);
            }
            return Success("ok");
        }
    
        public APIResult SubmitAuditShop(int shopId)
        {
            var target = db.ShopWechatOpenAuthorizers.Include(p => p.WechatOpenAuthorizer).Include(p => p.Shop).Where(m => m.ShopId == shopId && !m.IsDel).FirstOrDefault();
            if (target == null)
            {
                return Error("not found ShopWechatOpenAuthorizer by shopid");
            }
            //取小程序模板列表
            var accessToken = AuthorizerHelper.GetComponentAccessToken();
            var tempList = CodeTemplateApi.GetTemplateListAsync(accessToken).Result;
            var tempInfo = tempList.template_list[tempList.template_list.Count - 1];
            string newVersion = tempInfo.user_version; //最新版本                     
            AutoUpgrade(target, tempInfo);

            return Success("ok");
        }

        public APIResult AutoUpgrade(ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            var authorizerAccessToken = AuthorizerHelper.GetAuthorizerAccessToken(item.WechatOpenAuthorizer.AuthorizerAppId);

            //检查是否已创建开放平台和绑定，此功能目的是为了登录时获取到unionid
            var bindResult = AuthorizerHelper.CreateAndBindOpen(item.WechatOpenAuthorizer.AuthorizerAppId);

            //当前店铺不是最新版 提交审核
            if (item.ReleaseTemplateUserVersion != tempInfo.user_version)
            {
                return SubmitVersion(authorizerAccessToken, item, tempInfo);
            }
            else     //已提交审核则查询审核状态
            {
                if (item.CurrentTemplateUserVersion != item.ReleaseTemplateUserVersion)
                {
                    return QuerySubmit(authorizerAccessToken, item, tempInfo);
                }
            }
            return Success();
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="authorizerAccessToken"></param>
        /// <param name="item"></param>
        /// <param name="tempInfo"></param>
        public APIResult SubmitVersion(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            //先上传代码
            string extJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new { ext = new { shopFlag = item.Shop.Flag } });
            var commitResult = CodeApi.Commit(authorizerAccessToken, tempInfo.template_id, extJsonString, tempInfo.user_version, tempInfo.user_desc);
            //如果请求成功，则将请求的参数写入到数据库进行保存
            if (commitResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                return Error("上传代码失败，" + commitResult.errmsg);
            }

            //提交审核
            List<SubmitAuditPageInfo> list = new List<SubmitAuditPageInfo>();
            CategroyInfo category = AuthorizerHelper.GetCategory(authorizerAccessToken);
            list.Add(new SubmitAuditPageInfo()
            {
                first_id = category.first_id,
                second_id = category.second_id,
                first_class = category.first_class,
                second_class = category.second_class,
                third_class = category.third_class,
                third_id = category.third_id,
                title = "点餐",
                tag = category.first_class,
                address = "pages/home/home"
            });
            try
            {
                var submitAuditResult = CodeApi.SubmitAudit(authorizerAccessToken, list);

                if (submitAuditResult.errcode != ReturnCode.请求成功)
                {
                    return Error("提交审核失败，" + submitAuditResult.errmsg);
                }
                item.CurrentTemplateExtJson = extJsonString;
                item.CurrentAuditId = int.Parse(submitAuditResult.auditid);
                item.CurrentAuditStatus = 2; //审核状态，其中0为审核成功，1为审核失败，2为审核中
                item.ReleaseTemplateUserVersion = tempInfo.user_version;
                item.IsRelease = false;//将发布状态设置为未发布              
                db.SaveChanges();

                if (item.CurrentAuditStatus == 0) //审核通过则马上发布
                {
                    ReleaseVersion(authorizerAccessToken, item, tempInfo);
                }
            }
            catch (Exception ex)
            {
                return Error("提交审核失败，" + ex.Message);
            }

            return Success();
        }

        public APIResult QuerySubmit(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            string text = string.Format("https://api.weixin.qq.com/wxa/get_latest_auditstatus?access_token={0}", authorizerAccessToken);
            var queryResult = CommonJsonSend.Send<GetAuditStatusResultJson>(null, text, null, CommonJsonSendType.GET, 1000, false, null);

            if (queryResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                return Error("查询发布失败," + queryResult.errmsg);
            }
            //如果请求成功，则将请求的结果写入到数据库中

            item.CurrentAuditStatus = queryResult.status;//审核状态，其中0为审核成功，1为审核失败，2为审核中                      
            item.CurrentAuditFailReason = queryResult.reason;
            db.SaveChanges();
            if (queryResult.status == 0) //审核通过，马上发布
            {
                if (!item.IsRelease)
                {
                    var releaseResult = ReleaseVersion(authorizerAccessToken, item, tempInfo);
                    return releaseResult;
                }
                else
                {
                    item.CurrentTemplateUserVersion = item.ReleaseTemplateUserVersion;
                    db.SaveChanges();
                }
            }

            return Success();
        }

        /// <summary>
        /// 发布已通过版本
        /// </summary>
        /// <param name="authorizerAccessToken"></param>
        /// <param name="shopId"></param>
        /// <param name="logDbContext"></param>
        public APIResult ReleaseVersion(string authorizerAccessToken, ShopWechatOpenAuthorizer item, TemplateInfo tempInfo)
        {
            var releaseResult = CodeApi.Release(authorizerAccessToken);
            //如果发布成功，则将请求的结果写入到数据库中
            if (releaseResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                return Error("发布失败," + releaseResult.errmsg);
            }

            item.IsRelease = true;
            item.CurrentTemplateUserVersion = item.ReleaseTemplateUserVersion;
            item.CurrentTemplateId = tempInfo.template_id;
            item.CurrentTemplateUserDesc = tempInfo.user_desc;
            db.SaveChanges();
            //这里尝试添加二唯码规则       
            if (hostingEnvironment != null)
            {
                var qrCodeResult = CodeApiExt.QRCodeJumpAddPublish(item.ShopId, authorizerAccessToken, hostingEnvironment);
                if (qrCodeResult.errcode != ReturnCode.请求成功)
                {
                    return Error("添加二维码规则失败 ," + qrCodeResult.errmsg);
                }
            }
            return Success();

        }


    }

}
