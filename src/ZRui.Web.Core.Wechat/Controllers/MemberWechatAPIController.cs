using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZRui.Web.MemberSetAPIModels;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using ZRui.Web.Core;
using Microsoft.Extensions.Options;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.MemberWechatAPIModels;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.Entities;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 管理用微信接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class MemberWechatAPIController : CommunityApiControllerBase
    {
        WechatCoreDbContext wechatCoreDb;
        WechatOptions wechatOptions;
        private readonly ILogger _logger;

        string GetAccessToken()
        {
            return AccessTokenContainer.TryGetAccessToken(wechatOptions.AppId, wechatOptions.AppSecret);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wechatOptions"></param>
        /// <param name="wechatCoreDb"></param>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="loggerFactory"></param>
        public MemberWechatAPIController(IOptions<WechatOptions> wechatOptions
            , WechatCoreDbContext wechatCoreDb
            , ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            this.wechatCoreDb = wechatCoreDb;
            _logger = loggerFactory.CreateLogger<MemberWechatAPIController>();
            this.wechatOptions = wechatOptions.Value;
        }
        /// <summary>
        /// 获取登陆用的QRCodeUrl
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult<string> GetLoginQRCodeUrl([FromBody]GetLoginQRCodeUrlArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ClientId)) throw new ArgumentNullException("clientId");
            WechatQRScene model = new WechatQRScene()
            {
                Category = "Login",
                Status = WechatQRSceneStatus.未处理,
                QrCodeTicket = ""
            };
            wechatCoreDb.AddToWechatQRScene(model);
            wechatCoreDb.SaveChanges();


            var b = model.Id / 4294967295;
            model.SceneId = (int)(model.Id - 4294967295 * b);

            var accessToken = GetAccessToken();
            var qrResult = QrCodeApi.Create(accessToken, 60 * 5, model.SceneId, QrCode_ActionName.QR_SCENE);

            model.QrCodeTicket = qrResult.ticket;
            wechatCoreDb.AddToMemberWeChatLoginTask(new MemberWeChatLoginTask()
            {
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                Code = model.SceneId.ToString(),
                OpenId = "",
                ClientId = args.ClientId,
                Status = MemberWeChatLoginTaskStatus.扫二维码进行中
            });
            wechatCoreDb.SaveChanges();

            var url = QrCodeApi.GetShowQrCodeUrl(model.QrCodeTicket);
            return Success<string>(url);
        }
        /// <summary>
        /// 尝试使用微信二维码进行登陆
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult<bool>> TryLoginAsync([FromBody]TryLoginArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ClientId)) throw new ArgumentNullException("clientId");
            var ip = GetIp();
            var task = wechatCoreDb.QueryMemberWeChatLoginTask()
                .Where(m => m.ClientId == args.ClientId)
                .Where(m => m.AddIp == ip)
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();

            if (task == null)
            {
                throw new Exception("请先发起登陆请求");
            }

            switch (task.Status)
            {
                case MemberWeChatLoginTaskStatus.扫二维码进行中:
                    return Success<bool>(false);
                case MemberWeChatLoginTaskStatus.扫二维码完成:
                    var member = wechatCoreDb.QueryMemberWechat()
                        .Where(m => m.OpenId == task.OpenId)
                        .Where(m => !m.IsDel)
                        .FirstOrDefault();
                    if (member == null) throw new Exception("请先绑定微信");

                    task.Status = MemberWeChatLoginTaskStatus.登陆完成;
                    wechatCoreDb.SaveChanges();

                    List<Claim> claims = new List<Claim>();
                    var username = "member" + member.MemberId;

                    var member2 = memberDb.QueryMember()
                    .Where(m => m.Id == member.MemberId)
                    .Where(m => !m.IsDel)
                    .Select(m => new
                    {
                        Id = m.Id,
                        Email = m.Email,
                        Password = m.Password,
                        Truename = m.Truename
                    })
                    .FirstOrDefault();
                    if (member == null) throw new ArgumentException("账号不正确");

                    claims.Add(new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, null));
                    claims.Add(new Claim("Truename", member2.Truename, ClaimValueTypes.String));
                    var userIdentity = new ClaimsIdentity("Form");
                    userIdentity.AddClaims(claims);

                    var principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Success<bool>(true);
                case MemberWeChatLoginTaskStatus.登陆完成:
                default:
                    return Error<bool>("登陆已经完成，任务失效");
            }
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult CreateMenu([FromBody]CreateMenuArgsModel args)
        {
            if (args.Items == null && args.Items.Count <= 0) throw new ArgumentNullException("items");

            var accessToken = GetAccessToken();
            var buttonData = new ButtonGroup();
            foreach (var item in args.Items)
            {
                if (item.Items == null || item.Items.Count <= 0)
                {
                    buttonData.button.Add(new SingleViewButton() { name = item.Name, type = item.Type, url = item.Url });
                }
                else
                {
                    var subButton = new SubButton()
                    {
                        name = "面试者中心",
                        sub_button = new List<SingleButton>()
                    };
                    buttonData.button.Add(subButton);

                    foreach (var subItem in item.Items)
                    {
                        subButton.sub_button.Add(new SingleViewButton() { name = subItem.Name, type = subItem.Type, url = subItem.Url });
                    }
                }
            }
            //buttonData.button.Add(new SingleViewButton() { name = "关于携程", type = "view", url = "http://www.cscoder.cn" });
            //buttonData.button.Add(new SingleViewButton() { name = "岗位直击", type = "view", url = "http://www.cscoder.cn" });
            //buttonData.button.Add(new SubButton()
            //{
            //    name = "面试者中心",
            //    sub_button = new List<SingleButton>() {
            //    new SingleViewButton() { name = "绑定简历", type = "view", url = "http://demo.cscoder.cn/dist/index.html#/bindmobile" },
            //    new SingleViewButton() { name = "面试登记表", type = "view", url = "http://demo.cscoder.cn/dist/index.html#/writeinfo" },
            //    new SingleViewButton() { name = "我的进度", type = "view", url = "http://demo.cscoder.cn/dist/index.html#/info" }
            //}
            //});
            var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(accessToken, buttonData);
            if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                return Success();
            }
            else
            {
                throw new Exception(string.Format("{0}:{1}", result.errcode, result.errmsg));
            }
        }
        /// <summary>
        /// 通过客户的手机获得他的OpenId
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<string> GetOpenIdByCustomerPhone([FromBody]GetOpenIdByCustomerPhoneArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Phone)) throw new ArgumentNullException("phone");
            var openId = wechatCoreDb.GetOpenIdByCustomerPhone(args.Phone);
            if (string.IsNullOrEmpty(openId)) throw new Exception("手机未有微信绑定纪录");
            return Success<string>(openId);
        }
        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SendCustomerText([FromBody]SendCustomerTextArgsModel args)
        {
            if (string.IsNullOrEmpty(args.OpenId)) throw new ArgumentNullException("OpenId");
            if (string.IsNullOrEmpty(args.Text)) throw new ArgumentNullException("text");

            var user = GetUsername();
            wechatCoreDb.EnsureIsCustomerSessionWorker(args.OpenId, user);

            var time48 = DateTime.Now.AddHours(-47);
            var last = wechatCoreDb.QueryCustomerMessage()
                .OrderByDescending(m => m.Time)
                .Select(m => m.Time)
                .FirstOrDefault();

            if (last <= time48)
            {
                throw new Exception("最后发送信息超过48小时");
            }

            var accessToken = GetAccessToken();
            Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(accessToken, args.OpenId, args.Text, 10000, "");

            var msg = new CustomerMessage()
            {
                FromUser = GetUsername(),
                ToUser = args.OpenId,
                Time = DateTime.Now,
                ContentObject = new
                {
                    msgType = "text",
                    text = args.Text
                },
                ChatFlag = args.OpenId
            };
            wechatCoreDb.AddToCustomerMessage(msg);
            wechatCoreDb.UpdateCustomerSessionTime(args.OpenId);

            wechatCoreDb.SaveChanges();
            return Success();
        }
        /// <summary>
        /// 发送新闻链接
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SendCustomerNews([FromBody]SendCustomerNewsArgsModel args)
        {
            if (string.IsNullOrEmpty(args.OpenId)) throw new ArgumentNullException("OpenId");
            if (string.IsNullOrEmpty(args.Description)) throw new ArgumentNullException("Description");
            if (string.IsNullOrEmpty(args.Url)) throw new ArgumentNullException("Url");

            var user = GetUsername();
            wechatCoreDb.EnsureIsCustomerSessionWorker(args.OpenId, user);

            var accessToken = GetAccessToken();
            var article = new Article()
            {
                Description = args.Description,
                Title = args.Title,
                Url = args.Url
            };
            Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendNews(accessToken, args.OpenId, new List<Article>() { article }, 10000, "");
            var msg = new CustomerMessage()
            {
                FromUser = GetUsername(),
                ToUser = args.OpenId,
                Time = DateTime.Now,
                //Content = $"title:{args.Title},description:{args.Description},url:{args.Url}",
                ContentObject = new
                {
                    msgType = "news",
                    title = args.Title,
                    description = args.Description,
                    url = args.Url
                },
                ChatFlag = args.OpenId
            };
            wechatCoreDb.AddToCustomerMessage(msg);
            wechatCoreDb.UpdateCustomerSessionTime(args.OpenId);
            wechatCoreDb.SaveChanges();
            return Success();
        }
        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> SendCustomerImage([FromBody]SendCustomerImageArgsModel args)
        {
            if (string.IsNullOrEmpty(args.OpenId)) throw new ArgumentNullException("OpenId");

            var user = GetUsername();
            wechatCoreDb.EnsureIsCustomerSessionWorker(args.OpenId, user);


            string requestStrValue = args.Data.Substring(args.Data.IndexOf(',') + 1);//代表 图片 的base64编码数据  
            string requestFileExtension = args.Data.Split(new char[] { ';' })[0].Substring(args.Data.IndexOf('/') + 1);//获取后缀名 

            var buffer = Convert.FromBase64String(requestStrValue);
            var folderPath = $"{Directory.GetCurrentDirectory()}/App_Data/Weixin/TemporaryMedia";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = $"{folderPath}/{DateTime.Now.ToString("yyyyMMddHHhhss")}{CommonUtil.CreateNoncestr(6)}.{requestFileExtension}";

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
                ////通过代码审查说，使用了using,不需要再次调用close
                //fs.Close();
            }

            //fileName = @"C:\Xiang\Temp\1.jpg";
            var accessToken = GetAccessToken();
            //var uploadResult = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(accessToken, UploadMediaFileType.image, fileName);

            var uploadResult = await WechatMediaApiProxy.UploadAsync<Senparc.Weixin.MP.AdvancedAPIs.Media.UploadTemporaryMediaResult>(accessToken, "image", fileName);
            var data = new
            {
                msgType = "image",
                mediaId = uploadResult.media_id
            };

            System.IO.File.Move(fileName, CustomerMessage.GetCachePathForTemporaryMedia(data.mediaId));
            var msg = new CustomerMessage()
            {
                FromUser = GetUsername(),
                ToUser = args.OpenId,
                Time = DateTime.Now,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(data),
                ChatFlag = args.OpenId
            };
            wechatCoreDb.AddToCustomerMessage(msg);
            wechatCoreDb.UpdateCustomerSessionTime(args.OpenId);

            wechatCoreDb.SaveChanges();
            return Success();
        }
        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SendCustomerTemplateMessage([FromBody]SendCustomerTemplateMessageArgsModel args)
        {
            if (args.Data == null) throw new ArgumentNullException("Data");
            if (string.IsNullOrEmpty(args.TemplateId)) throw new ArgumentNullException("TemplateId");
            if (string.IsNullOrEmpty(args.OpenId)) throw new ArgumentNullException("OpenId");

            var user = GetUsername();
            //暂时去掉
            //wechatCoreDb.EnsureIsCustomerSessionWorker(args.OpenId, user);

            var accessToken = GetAccessToken();
            Senparc.Weixin.MP.AdvancedAPIs.TemplateApi.SendTemplateMessage(accessToken, args.OpenId, args.TemplateId, args.Url, args.Data);

            var msg = new CustomerMessage()
            {
                FromUser = GetUsername(),
                ToUser = args.OpenId,
                Time = DateTime.Now,
                Content = "[模板消息]",
                ContentObject = new
                {
                    msgType = "templateMessage",
                    data = args.Data,
                    url = args.Url,
                    templateId = args.TemplateId
                },
                ChatFlag = args.OpenId
            };
            wechatCoreDb.AddToCustomerMessage(msg);
            wechatCoreDb.UpdateCustomerSessionTime(args.OpenId);

            wechatCoreDb.SaveChanges();
            return Success();
        }

        ///<summary>
        ///接入客服会话
        ///</summary>
        [HttpPost]
        [Authorize]
        public APIResult ConnectCustomerSession([FromBody]ConnectCustomerSessionArgsModel args)
        {
            var username = GetUsername();
            var customerSession = wechatCoreDb.QueryCustomerSession()
            .Where(m => m.OpenId == args.OpenId)
            .FirstOrDefault();
            if (customerSession == null)
            {
                customerSession = new CustomerSession()
                {
                    OpenId = args.OpenId,
                    Time = DateTime.Now,
                    Worker = "",
                    Status = CustomerSessionStatus.未接入

                };
                wechatCoreDb.AddToCustomerSession(customerSession);
                wechatCoreDb.SaveChanges();
            }

            if (customerSession.Status != CustomerSessionStatus.未接入 && customerSession.Worker != username)
            {
                throw new Exception("状态为非未接入，不能接入");
            }

            customerSession.Worker = username;
            customerSession.Status = CustomerSessionStatus.客服接入;

            var messageList = wechatCoreDb.QueryCustomerMessage()
                .Where(m => m.FromUser == args.OpenId)
                .Where(m => m.ToUser == "" && m.ToUser == null)
                .ToList();
            foreach (var message in messageList)
            {
                message.ToUser = username;
            }
            wechatCoreDb.SaveChanges();
            return Success();
        }

        ///<summary>
        ///取消接入客服会话
        ///</summary>
        [HttpPost]
        [Authorize]
        public APIResult DisConnectCustomerSession([FromBody]DisConnectCustomerSessionArgsModel args)
        {
            var username = GetUsername();
            var customerSession = wechatCoreDb.QueryCustomerSession()
            .Where(m => m.OpenId == args.OpenId)
            .FirstOrDefault();
            if (customerSession != null)
            {
                if (args.MustDo.HasValue && args.MustDo.Value)
                {
                    //如果必须做，那么这里就不进行是否已经接入判断。
                }
                else
                {
                    if (customerSession.Status != CustomerSessionStatus.未接入 && customerSession.Worker != username)
                    {
                        throw new Exception("不是你接入的会话，不能取消");
                    }
                }
                customerSession.Worker = "";
                customerSession.Status = CustomerSessionStatus.未接入;
                wechatCoreDb.SaveChanges();
            }
            return Success();
        }

        ///<summary>
        ///关闭接入客服会话
        ///</summary>
        [HttpPost]
        [Authorize]
        public APIResult CloseCustomerSession([FromBody]CloseCustomerSessionArgsModel args)
        {
            var username = GetUsername();
            var customerSession = wechatCoreDb.QueryCustomerSession()
            .Where(m => m.OpenId == args.OpenId)
            .FirstOrDefault();
            if (customerSession != null)
            {
                if (customerSession.Status != CustomerSessionStatus.未接入 && customerSession.Worker != username)
                {
                    throw new Exception("不是你接入的会话，不能关闭");
                }

                customerSession.Worker = "";
                customerSession.Status = CustomerSessionStatus.关闭;
                wechatCoreDb.SaveChanges();
            }
            return Success();
        }

        /// <summary>
        /// 获得无人接入的会话OpenId列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<List<string>> GetCustomerSessionOpenIdListForNobody([FromBody]GetCustomerSessionOpenIdListForNobodyArgsModel args)
        {
            var username = GetUsername();
            var items = wechatCoreDb.QueryCustomerSession()
            .Where(m => m.Status == CustomerSessionStatus.未接入)
            .OrderByDescending(m=>m.Time)
            .Select(m => m.OpenId)
            .ToList();

            return Success<List<string>>(items);
        }

        /// <summary>
        /// 获得当前用户的接入会话的OpenId列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<List<string>> GetCustomerSessionOpenIdList([FromBody]GetCustomerSessionOpenIdListArgsModel args)
        {
            var username = GetUsername();
            var items = wechatCoreDb.QueryCustomerSession()
            .Where(m => m.Worker == username)
            .Where(m => m.Status == CustomerSessionStatus.客服接入)
            .OrderByDescending(m => m.Time)
            .Select(m => m.OpenId)
            .ToList();

            return Success<List<string>>(items);
        }

        /// <summary>
        /// 获得指定OpenId接入会话的未读列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<GetCustomerMessageCountForUnreadModel> GetCustomerMessageCountForUnread([FromBody]GetCustomerMessageCountForUnreadArgsModel args)
        {
            var query = wechatCoreDb.QueryCustomerMessage()
                .Where(m => m.FromUser == args.OpenId)
                .Where(m => !m.MemberIsRead);

            var model = new GetCustomerMessageCountForUnreadModel();
            model.Count = query.Count();
            model.StartMsgId = query.Select(m => m.Id).FirstOrDefault();

            return Success(model);
        }

        /// <summary>
        /// 获得指定OpenId接入会话的未读列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<List<GetCustomerMessageCountForUnreadModel>> GetCustomerMessageCountForAllUnread([FromBody]GetCustomerMessageCountForAllUnreadArgsModel args)
        {
            var username = GetUsername();
            var items = wechatCoreDb.QueryCustomerMessage()
                .Where(m => m.ToUser == username)
                .Where(m => !m.MemberIsRead)
                .Select(m => new
                {
                    Id = m.Id,
                    FromUser = m.FromUser
                })
                .OrderBy(m => m.Id)
                .ToList()
                .GroupBy(m => m.FromUser)
                .Select(g => new GetCustomerMessageCountForUnreadModel
                {
                    StartMsgId = g.Select(m => m.Id).FirstOrDefault(),
                    Count = g.Count(),
                    FromUser = g.Key
                })
                .ToList();

            return Success(items);
        }

        /// <summary>
        /// 获得指定CharFlag的消息列表（CharFlag，在这里通常表现为OpenId)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<PagedList<CustomerMessage>> GetCustomerMessageList([FromBody]GetCustomerMessageListArgsModel args)
        {
            if (string.IsNullOrEmpty(args.ChatFlag)) throw new ArgumentNullException("ChatFlag");
            if (args.PageSize <= 0) args.PageSize = 20;
            var query = wechatCoreDb.QueryCustomerMessage();

            if (args.BeginMsgId.HasValue)
            {
                query = query.Where(m => m.Id > args.BeginMsgId);
            }

            if (!string.IsNullOrEmpty(args.ChatFlag))
            {
                query = query.Where(m => m.ChatFlag == args.ChatFlag);
            }

            var list = query.OrderBy(m => m.Id).ToPagedList(args.PageIndex, args.PageSize);
            var me = GetUsername();

            //CustomerMessaged的ChatFlag 就是 OpenId
            //如果当前对接人是自己，那么自己则可以将消息设置为已读
            if (wechatCoreDb.QueryCustomerSession().Where(m => m.OpenId == args.ChatFlag).Select(m => m.Worker).FirstOrDefault() == me)
            {
                foreach (var item in list)
                {
                    item.MemberIsRead = true;
                }
                wechatCoreDb.SaveChanges();
            }

            foreach (var item in list)
            {
                if (item.FromUser == me)
                {
                    item.FromUser = "我";
                }
                else if (item.FromUser == args.ChatFlag)
                {
                    item.FromUser = "客户";
                }
            }



            return Success(list);
        }

        /// <summary>
        /// 获取头像的Url地址
        /// </summary>
        /// <param name="args">参数类</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<string> GetHeadImageUrl([FromBody]GetHeadImageArgsModel args)
        {
            args.Size = args.Size ?? 0;
            var accessToken = GetAccessToken();
            var userInfo = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetUserInfo(accessToken, args.OpenId);
            var url = userInfo.GetHeadImageUrl(args.Size.Value);
            return Success<string>(url);
        }

        /// <summary>
        /// 获取头像的Url地址
        /// </summary>
        /// <param name="args">参数类</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<GetUserInfoModel> GetUserInfo([FromBody]GetUserInfoArgsModel args)
        {
            args.Size = args.Size ?? 0;
            var accessToken = GetAccessToken();
            var userInfo = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetUserInfo(accessToken, args.OpenId);
            if (userInfo.errcode != Senparc.Weixin.ReturnCode.请求成功) throw new Exception($"getUserInfo error:{userInfo.errcode},{userInfo.errmsg}");
            var url = userInfo.GetHeadImageUrl(args.Size.Value);
            return Success<GetUserInfoModel>(new GetUserInfoModel
            {
                HeadImageUrl = url,
                Nickname = userInfo.nickname,
                City = userInfo.city,
                Country = userInfo.country,
                Province = userInfo.province,
                Sex = userInfo.sex,
                Language = userInfo.language
            });
        }

        /// <summary>
        /// 获得新闻素材的列表
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<GetNewsMediaListModel> GetNewsMediaList([FromBody]GetNewsMediaListArgsModel args)
        {
            args.PageIndex = args.PageIndex ?? 0;
            args.PageSize = args.PageSize ?? 0;
            var offset = args.PageIndex.Value * args.PageSize.Value;
            var accessToken = GetAccessToken();
            var list = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.GetNewsMediaList(accessToken, offset, args.PageSize.Value);

            return Success<GetNewsMediaListModel>(new GetNewsMediaListModel
            {
                TotalCount = list.total_count,
                Items = list.item
            });
        }

        /// <summary>
        /// 获得其他素材的列表
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<GetOtherMediaListModel> GetOtherMediaList([FromBody]GetOtherMediaListArgsModel args)
        {
            args.MediaFileType = args.MediaFileType ?? UploadMediaFileType.image;
            args.PageIndex = args.PageIndex ?? 0;
            args.PageSize = args.PageSize ?? 0;
            var offset = args.PageIndex.Value * args.PageSize.Value;
            var accessToken = GetAccessToken();
            var list = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.GetOthersMediaList(accessToken, args.MediaFileType.Value, offset, args.PageSize.Value);

            return Success<GetOtherMediaListModel>(new GetOtherMediaListModel
            {
                TotalCount = list.total_count,
                Items = list.item
            });
        }

        /// <summary>
        /// 上传新闻素材
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult<UploadNewsModel>> UploadNews([FromBody]UploadNewsArgsModel args)
        {
            var accessToken = GetAccessToken();
            var result = await Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadNewsAsync(accessToken, 10000, new Senparc.Weixin.MP.AdvancedAPIs.GroupMessage.NewsModel()
            {
                content = args.Content,
                title = args.Title,
                thumb_media_id = args.ThumbMediaId
            });

            return Success<UploadNewsModel>(new UploadNewsModel
            {
                MediaId = result.media_id
            });
        }

        /// <summary>
        /// 上传永久媒体素材
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult<UploadForeverMediaModel>> UploadForeverMedia([FromBody]UploadForeverMediaArgsModel args)
        {
            args.MediaFileType = args.MediaFileType ?? UploadMediaFileType.image;
            var fileName = SaveBase64ToFile(args.Data, "App_Data/Weixin/UploadForeverMedia");
            var accessToken = GetAccessToken();

            var uploadResult = await WechatMediaApiProxy.UploadForeverAsync<Senparc.Weixin.MP.AdvancedAPIs.Media.UploadTemporaryMediaResult>(accessToken, args.MediaFileType.ToString(), fileName);

            //var result = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadForeverMedia(accessToken, fileName);

            return Success<UploadForeverMediaModel>(new UploadForeverMediaModel
            {
                MediaId = uploadResult.media_id
            });
        }

        /// <summary>
        /// 删除永久媒体素材
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult DeleteForeverMedia([FromBody]DeleteForeverMediaArgsModel args)
        {

            var accessToken = GetAccessToken();
            var result = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.DeleteForeverMedia(accessToken, args.MediaId);

            return Success();
        }

        /// <summary>
        /// 判定当前用户是否绑定微信
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult<bool> HasMemberWechat()
        {
            var memberId = GetMemberId();
            var hasRecord = wechatCoreDb.QueryMemberWechat()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Count() > 0;
            return Success<bool>(hasRecord);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDeleteForMemberWechat()
        {
            var memberId = GetMemberId();
            var memberWechat = wechatCoreDb.QueryMemberWechat()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();
            if (memberWechat == null) throw new Exception("无微信绑定纪录");
            memberWechat.IsDel = true;
            wechatCoreDb.SaveChanges();
            return Success();
        }

        string SaveBase64ToFile(string data, string targetRelativePath)
        {
            string requestStrValue = data.Substring(data.IndexOf(',') + 1);//代表 图片 的base64编码数据  
            string requestFileExtension = data.Split(new char[] { ';' })[0].Substring(data.IndexOf('/') + 1);//获取后缀名 

            var buffer = Convert.FromBase64String(requestStrValue);
            var folderPath = $"{Directory.GetCurrentDirectory()}/{targetRelativePath}/{DateTime.Now.ToString("yyyyMMddHHmmss")}/";
            FileUtils.CreateDirectory(folderPath);

            var fileName = $"{folderPath}{CommonUtil.CreateNoncestr(6)}.{requestFileExtension}";

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
            }

            return fileName;
        }
    }
}
