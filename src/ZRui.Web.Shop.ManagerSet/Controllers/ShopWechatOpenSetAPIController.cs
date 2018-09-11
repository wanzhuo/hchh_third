using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopWechatOpenSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using System.Collections.Generic;
using Senparc.Weixin.Open.Containers;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.Open;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Exceptions;
using System.IO;
using System.Text;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.WxaAPIs;
using Microsoft.Extensions.Logging;
using Senparc.Weixin;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Entities;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Template;
using System.Threading.Tasks;
using ZRui.Web.BLL;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    /// <summary>
    /// 商铺第三方小程序设置API
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class ShopWechatOpenSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        WechatOpenOptions wechatOpenOptions;
        WechatTemplateSendOptions wechatTemplateSendOptions;
        ILogger logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">MemberAPIOptions</param>
        /// <param name="db">商铺DbContext</param>
        /// <param name="wechatOpenOptions">wechatOpenOptions</param>
        /// <param name="hostingEnvironment">主机环境</param>
        public ShopWechatOpenSetAPIController(IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IOptions<WechatOpenOptions> wechatOpenOptions
            , IOptions<WechatTemplateSendOptions> wechatTemplateSendOptions
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.wechatOpenOptions = wechatOpenOptions.Value;
            this.wechatTemplateSendOptions = wechatTemplateSendOptions.Value;
            this.logger = loggerFactory.CreateLogger<ShopWechatOpenSetAPIController>();
        }


        ///// <summary>
        ///// 绑定小程序拼团推送模板
        ///// </summary>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //[HttpPost]
        ////[Authorize]
        //public async Task<APIResult> AddTemplate([FromBody]ShopIdArgsModel args)
        //{
        //    logger.LogInformation($"==================AddTemplate开始==================");
        //    if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
        //    var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
        //    //var libraryGetJsonResult = LibraryGet(authorizerAccessToken, "AT0051");
        //    //var addJsonResult = await TemplateApi.AddAsync(authorizerAccessToken, "AT0051", new int[] { 1, 3, 13, 16, 32, 34 });
        //    var list = await TemplateApi.ListAsync(authorizerAccessToken, 0, 20);
        //    //K-RBO0yV9s0f5qWI5Zisc-5nDXH4TcmcE0XGYJJfzE4
        //    //1533263821185
        //    var data = new
        //    {
        //        keyword1 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //        keyword2 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //        keyword3 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //        keyword4 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //        keyword5 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //        keyword6 = new
        //        {
        //            value = "测试",
        //            color = "#173177"
        //        },
        //    };
        //    var sendTemplateMessage = await TemplateApi.SendTemplateMessageAsync(authorizerAccessToken, "oFjUE5kslkBdABu9uml9gC9C-JRM", "K-RBO0yV9s0f5qWI5Zisc-5nDXH4TcmcE0XGYJJfzE4", data, "ed98cab315c83daded19b95f6e4fc9f9", "pages/home/statusinfo");

        //    logger.LogInformation($"==================sendTemplateMessage:{sendTemplateMessage}==================");

        //    logger.LogInformation($"==================AddTemplate结束==================");


        //    return Success(sendTemplateMessage);
        //    //await TemplateApi.AddAsync(authorizerAccessToken, "AT0051",)
        //}

        /// <summary>
        /// 获取模板库某个模板标题下关键词库
        /// </summary>
        /// <param name="accessToken">接口调用凭证</param>
        /// <param name="id">模板标题id，可通过接口获取，也可登录小程序后台查看获取</param>
        /// <param name="timeOut">请求超时时间</param>
        /// <returns></returns>
        public static LibraryGetJsonResult LibraryGet(string accessToken, string id, int timeOut = Config.TIME_OUT)
        {
            string urlFormat = Config.ApiMpHost + "/cgi-bin/wxopen/template/library/get?access_token={0}";
            var data = new
            {
                id = id
            };
            return CommonJsonSend.Send<LibraryGetJsonResult>(accessToken, urlFormat, data, timeOut: timeOut);
        }
        /// <summary>
        /// 判定是否已经绑定小程序
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<bool> GetIsBind([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var isBind = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .Count() > 0;

            return Success(isBind);
        }

        /// <summary>
        /// 获得某个缓存的信息
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public APIResult GetCacheItem([FromBody]GetCacheItemArgsModel args)
        //{
        //    var model = AuthorizerContainer.TryGetItem(args.ShortKey);
        //    return Success(model);
        //}
        /// <summary>
        /// 绑定微信用户为小程序体验者
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> BindTester([FromBody]TesterArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
            var bindResult = await TesterApi.BindTesterSync(authorizerAccessToken.authorizer_access_token, args.WechatId);
            if (bindResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                throw new Exception($"请求失败：{bindResult.errcode}-{bindResult.errmsg}");
            }
            return Success();
        }
        /// <summary>
        /// 解绑微信用户为小程序体验者
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> UnBindTester([FromBody]TesterArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
            var bindResult = await TesterApi.UnBindTesterSync(authorizerAccessToken.authorizer_access_token, args.WechatId);
            if (bindResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                throw new Exception($"请求失败：{bindResult.errcode}-{bindResult.errmsg}");
            }
            return Success();
        }

        /// <summary>
        /// 解绑微信用户为小程序体验者
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> GetTesters([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
            var result = TesterApiExt.GetList(authorizerAccessToken.authorizer_access_token);
            if (result.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                throw new Exception($"请求失败：{result.errcode}-{result.errmsg}");
            }
            else
            {
                return Success(result);
            }
        }

        /// <summary>
        /// 获得绑定的小程序信息
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetAuthorizerInfo([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);


            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
                })
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");
            //  var authorizerInfoResult = AuthorizerContainer.GetAuthorizerInfoResult(wechatOpenOptions.AppId,
            //         model.AuthorizerAppId);
            AuthorizerInfo auth = GetAuthorizationInfo(model.AuthorizerAppId);
            return Success(auth);
        }


        private AuthorizerInfo GetAuthorizationInfo(string authorizerAppId)
        {
            string token = ZRui.Web.BLL.AuthorizerHelper.GetComponentAccessToken();
            GetAuthorizerInfoResult authorizerInfoResult = ComponentApi.GetAuthorizerInfo(token, wechatOpenOptions.AppId, authorizerAppId, 0x2710);
            return authorizerInfoResult.authorizer_info;
        }
        /// <summary>
        /// 获得绑定的小程序信息
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult UpdateAuthorizerInfo([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .Select(m => m.WechatOpenAuthorizer)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");

            var authorizerInfoResult = AuthorizerContainer.GetAuthorizerInfoResult(wechatOpenOptions.AppId,
                    model.AuthorizerAppId);

            if (authorizerInfoResult.errcode != Senparc.Weixin.ReturnCode.请求成功) throw new Exception("请求失败：" + authorizerInfoResult.errcode);

            model.AuthorizerNickname = authorizerInfoResult.authorizer_info.nick_name;
            model.AuthorizerUsername = authorizerInfoResult.authorizer_info.user_name;
            db.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 刷新绑定的小程序的Token
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult RefreshToken([FromBody]ShopIdArgsModel args)
        {
            //if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            //CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            //var shopWechatOpenAuthorizer = db.Query<ShopWechatOpenAuthorizer>()
            //    .Where(m => !m.IsDel)
            //    .Where(m => m.ShopId == args.ShopId.Value)
            //    .Select(m => new
            //    {
            //        AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId
            //    })
            //    .FirstOrDefault();
            //if (shopWechatOpenAuthorizer == null) throw new Exception("指定的纪录不存在");
            //Senparc.Weixin.Open.Containers.AuthorizerContainer.RemoveFromCache(shopWechatOpenAuthorizer.AuthorizerAppId);
            //Senparc.Weixin.Open.Containers.AuthorizerContainer.TryGetAuthorizerAccessToken(wechatOpenOptions.AppId, shopWechatOpenAuthorizer.AuthorizerAppId, true);



            return Success();
        }

        /// <summary>
        /// 修改可请求的Domain
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ModifyDomain([FromBody]ModifyDomainArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            //var model = db.Query<ShopWechatOpenAuthorizer>()
            //    .Where(m => !m.IsDel)
            //    .Where(m => m.ShopId == args.ShopId.Value)
            //    .Select(m => new
            //    {
            //        AuthorizerAccessToken = m.WechatOpenAuthorizer.AuthorizerAccessToken
            //    })
            //    .FirstOrDefault();
            //if (model == null) throw new Exception("指定的纪录不存在");

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            args.RequestDomains.Add("https://wxapi.91huichihuihe.com");

            var result = Senparc.Weixin.Open.WxaAPIs.ModifyDomainApi.ModifyDomain(authorizerAccessToken.authorizer_access_token, Senparc.Weixin.Open.ModifyDomainAction.set, args.RequestDomains, args.WsRequestDomains, args.UploadDomains, args.DownloadDomains);

            return Success(result);
        }

        /// <summary>
        /// 获取当前设置的可请求Domain
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetDomain([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            //注意参数 ModifyDomainAction.get 表示获取的意思
            var result = Senparc.Weixin.Open.WxaAPIs.ModifyDomainApi.ModifyDomain(authorizerAccessToken.authorizer_access_token, Senparc.Weixin.Open.ModifyDomainAction.get, null, null, null, null);

            return Success(result);
        }

        /// <summary>
        /// 获取小程序的第三方提交代码的页面配置
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetPage([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = Senparc.Weixin.Open.WxaAPIs.CodeApi.GetPage(authorizerAccessToken.authorizer_access_token);
            return Success(result);
        }

        /// <summary>
        /// 获取授权小程序帐号的可选类目
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        //  [Authorize]
        public APIResult GetCategory([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = Senparc.Weixin.Open.WxaAPIs.CodeApi.GetCategory(authorizerAccessToken.authorizer_access_token);
            return Success(result);
        }

        /// <summary>
        /// 将第三方提交的代码包提交审核
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult<GetAuditStatusResultJson>> SubmitAudit([FromBody]SubmitAuditArgsModel args)
        {
            try
            {
                if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
                CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

                var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

                var result = await CodeApi.SubmitAuditAsync(authorizerAccessToken.authorizer_access_token, args.Items);
                //logger.LogTrace("SubmitAuditAsync result：{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));
                if (result.errcode == ReturnCode.请求成功)
                {
                    var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopId == args.ShopId.Value)
                        .FirstOrDefault();
                    if (authorizer != null)
                    {
                        authorizer.CurrentAuditId = int.Parse(result.auditid);
                        authorizer.CurrentAuditStatus = 2; //审核状态，其中0为审核成功，1为审核失败，2为审核中
                        authorizer.CurrentTemplateUserVersion = args.user_version;
                        authorizer.IsRelease = false;//将发布状态设置为未发布
                        await db.SaveChangesAsync();
                    }
                }
                return Success(result);
            }
            catch (Exception ex)
            {

                throw new Exception($"提交失败，注意：需要先提交体验版后再提交代码包审核。");
            }

        }

        /// <summary>
        /// 查询某个指定版本的审核状态
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult<GetAuditStatusResultJson>> GetAuditStatus([FromBody]GetAuditStatusArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = await Senparc.Weixin.Open.WxaAPIs.CodeApi.GetAuditStatusAsync(authorizerAccessToken.authorizer_access_token, args.AuditId);
            return Success(result);
        }

        /// <summary>
        /// 查询某个指定版本的审核状态
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult<GetAuditStatusResultJson>> GetLatestAuditStatus([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = await Senparc.Weixin.Open.WxaAPIs.CodeApi.GetLatestAuditStatusAsync(authorizerAccessToken.authorizer_access_token);

            //如果请求成功，则将请求的结果写入到数据库中
            if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == args.ShopId.Value)
                    .FirstOrDefault();
                if (authorizer.CurrentAuditId == int.Parse(result.auditid))
                {
                    authorizer.CurrentAuditStatus = result.status;
                    if (result.status == 1)//审核状态，其中0为审核成功，1为审核失败，2为审核中
                    {
                        authorizer.CurrentAuditFailReason = result.reason;
                    }
                    await db.SaveChangesAsync();
                }
            }

            return Success(result);
        }

        /// <summary>
        /// 发布已通过审核的小程序
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult<CodeResultJson>> ReleaseAsync([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);
            var result = await CodeApi.ReleaseAsync(authorizerAccessToken.authorizer_access_token);


            //如果请求成功，则将请求的结果写入到数据库中
            if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == args.ShopId.Value)
                    .FirstOrDefault();

                authorizer.IsRelease = true;
                await db.SaveChangesAsync();
                //这里尝试添加二唯码规则
                try
                {
                    CodeApiExt.QRCodeJumpAddPublish(args.ShopId.Value, authorizerAccessToken.authorizer_access_token, hostingEnvironment);
                }
                catch (Exception)
                {
                }
            }
            return Success(result);
        }

        /// <summary>
        /// 修改小程序线上代码的可见状态
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ChangeVisitStatus([FromBody]ChangeVisitStatusArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = Senparc.Weixin.Open.WxaAPIs.CodeApi.ChangeVisitStatus(authorizerAccessToken.authorizer_access_token, args.Action);
            return Success(result);
        }

        /// <summary>
        /// 设置小程序“扫普通链接二维码打开小程序”能力 之 增加或修改二维码规则
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QRCodeJumpAdd([FromBody]QRCodeJumpAddArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Path)) throw new ArgumentNullException("Path");
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            //先下载验证文件
            var prefix = new Uri(args.Prefix);



            var fileInfo = CodeApiExt.QRCodeJumpDownload(authorizerAccessToken.authorizer_access_token);

            var filePath = hostingEnvironment.MapWebPath(Path.Combine(prefix.PathAndQuery, fileInfo.file_name));
            Common.FileUtils.CreateDirectory(filePath);
            //System.IO.File.Create(filePath);
            System.IO.File.WriteAllText(filePath, fileInfo.file_content);

            if (args.DebugUrl == null) args.DebugUrl = new List<string>();
            if (string.IsNullOrEmpty(args.PermitSubRule)) args.PermitSubRule = "1";

            var result = CodeApiExt.QRCodeJumpAdd(authorizerAccessToken.authorizer_access_token, args.Prefix, args.PermitSubRule, args.Path, args.OpenVersion, args.DebugUrl.ToArray(), args.IsEdit);
            return Success(result);
        }

        /// <summary>
        /// 设置小程序“扫普通链接二维码打开小程序”能力 之 获取
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QRCodeJumpGet([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = CodeApiExt.QRCodeJumpGet(authorizerAccessToken.authorizer_access_token);
            return Success(result);
        }

        /// <summary>
        /// 设置小程序“扫普通链接二维码打开小程序”能力 之 删除
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QRCodeJumpDelete([FromBody]QRCodeJumpPrefixArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = CodeApiExt.QRCodeJumpDelete(authorizerAccessToken.authorizer_access_token, args.Prefix);
            return Success(result);
        }

        /// <summary>
        /// 设置小程序“扫普通链接二维码打开小程序”能力 之 发布
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QRCodeJumpPublish([FromBody]QRCodeJumpPrefixArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.ShopId.Value);

            var result = CodeApiExt.QRCodeJumpPublish(authorizerAccessToken.authorizer_access_token, args.Prefix);
            return Success(result);
        }


        /// <summary>
        /// 添加、发布二维码跳转规则
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QRCodeJumpAddPublish([FromBody]IdArgsModel args)
        {
            CheckShopActor(args.Id, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.Id);

            var result = CodeApiExt.QRCodeJumpAddPublish(args.Id, authorizerAccessToken.authorizer_access_token, hostingEnvironment);
            return Success(result);
        }



        /// <summary>
        /// 初始化微信商铺功能
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult InitShop([FromBody]IdArgsModel args)
        {
            CheckShopActor(args.Id, ShopActorType.超级管理员);

            var authorizerAccessToken = GetAuthorizerAccessToken(args.Id);

            try
            {

                var result = CodeApiExt.QRCodeJumpAddPublish(args.Id, authorizerAccessToken.authorizer_access_token, hostingEnvironment);
                AuthorizerHelper.CreateAndBindOpen(authorizerAccessToken.authorizer_appid);
                if (result.errcode == ReturnCode.请求成功)
                {
                    return Success("设置成功");
                }
                else
                {
                    return Error(result.errmsg);
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("初始化商店失败。错误路径：{0}", ex.StackTrace);
                return Error(ex.Message);
            }

        }


        /// <summary>
        /// 使用指定的模板
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> UseTemplate([FromBody]UseTemplateArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .Select(m => new
                {
                    ShopFlag = m.Shop.Flag,
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId,
                    CurrentTemplateId = m.CurrentTemplateId
                })
                .FirstOrDefault();
            if (model == null) throw new Exception("指定的纪录不存在");
            if (!model.CurrentTemplateId.HasValue || model.CurrentTemplateId.Value != args.TemplateId)
            {
                var authorizerAccessToken = GetAuthorizerAccessToken(model.AuthorizerAppId);
                var extJson = Newtonsoft.Json.JsonConvert.DeserializeXNode(args.ExtJson);
                var ext = extJson.Element("ext");
                if (ext == null)
                {
                    ext = new System.Xml.Linq.XElement("ext");
                    extJson.Add(ext);
                }

                if (ext.Elements("shopFlag").Count() <= 0)
                {
                    ext.Add(new System.Xml.Linq.XElement("shopFlag", model.ShopFlag));
                }

                var extJsonString = Newtonsoft.Json.JsonConvert.SerializeXNode(extJson);
                var result = Senparc.Weixin.Open.WxaAPIs.CodeApi.Commit(authorizerAccessToken.authorizer_access_token, args.TemplateId, extJsonString, args.UserVersion, args.UserDesc);
                //如果请求成功，则将请求的参数写入到数据库进行保存
                if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    var authorizer = db.Query<ShopWechatOpenAuthorizer>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.ShopId == args.ShopId.Value)
                        .FirstOrDefault();
                    authorizer.CurrentTemplateId = args.TemplateId;
                    authorizer.CurrentTemplateExtJson = extJsonString;
                    //authorizer.CurrentTemplateUserVersion = args.UserVersion;
                    authorizer.CurrentTemplateUserDesc = args.UserDesc;
                    await db.SaveChangesAsync();
                }
                return Success(result);
            }
            else
            {
                return Success();
            }
        }

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> GetTemplateList()
        {
            TemplateInfo temp = null;
            var accessToken = ZRui.Web.BLL.AuthorizerHelper.GetComponentAccessToken();//  ComponentContainer.TryGetComponentAccessToken(wechatOpenOptions.AppId, wechatOpenOptions.AppSecret);

            var result = await CodeTemplateApi.GetTemplateListAsync(accessToken);
            if (result.template_list != null && result.template_list.Count > 0)
            {
                temp = result.template_list[result.template_list.Count - 1];
                if (!string.IsNullOrWhiteSpace(temp.user_version))
                {
                    var version = db.Query<SystemVersion>()
                       .FirstOrDefault(p => p.VersionCode== temp.user_version);
                    if (version != null)
                    {
                        temp.user_desc = version.VersionDesc;
                    }
                }
            }
            return Success(temp);
        }

        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> GetOAuthUrl([FromBody]GetOAuthQrcodeArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("shopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);
            var redictUrl = "http://manager.91huichihuihe.com/ShopWechatOpenOAuth/ShopReceiverOAuth";
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(wechatTemplateSendOptions.AppId, redictUrl,
                    args.ShopId.Value.ToString(), Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);
            return Success(result);
        }

        [HttpPost]
        [Authorize]
        public APIResult GetShopOAuthList([FromBody]ShopIdArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new ArgumentNullException("shopId");
            CheckShopActor(args.ShopId.Value, ShopActorType.超级管理员);

            var list = db.Query<ShopOrderReceiver>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId.Value)
                .ToList();
            return Success(list);
        }

        [HttpPost]
        [Authorize]
        public APIResult DelShopOAuth([FromBody]IdArgsModel args)
        {
            var model = db.Set<ShopOrderReceiver>().Find(args.Id);
            if (model == null) throw new Exception("该记录不存在");
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            model.IsDel = true;
            db.SaveChanges();
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult SetShopOAuthIsUsed([FromBody]SetShopOAuthIsUsedArgsModel args)
        {
            var model = db.GetSingle<ShopOrderReceiver>(args.ID);
            if (model == null) throw new Exception("该记录不存在");
            CheckShopActor(model.ShopId, ShopActorType.超级管理员);
            if (args.IsUsed)
            {
                bool canUsed = db.Query<ShopOrderReceiver>()
                                .Where(m => !m.IsDel)
                                .Where(m => m.ShopId == model.ShopId)
                                .Where(m => m.IsUsed)
                                .Count() < 5;
                if (!canUsed) throw new Exception("只能同时启用五个接收者");
            }
            model.IsUsed = args.IsUsed;
            db.SaveChanges();
            return Success();
        }

        AuthorizationInfo GetAuthorizerAccessToken(int shopId)
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
            return GetAuthorizerAccessToken(model.AuthorizerAppId);
        }

        AuthorizationInfo GetAuthorizerAccessToken(string authorizerAppId)
        {           
            string accessToken=   ZRui.Web.BLL.AuthorizerHelper.GetAuthorizerAccessToken(authorizerAppId);
            return new AuthorizationInfo() { authorizer_appid = authorizerAppId, authorizer_access_token = accessToken };
        }
    }
}
