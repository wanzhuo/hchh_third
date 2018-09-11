using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System.IO;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.MemberWechatModels;
using ZRui.Web.Common;
using SixLabors.ImageSharp;

namespace ZRui.Web.Controllers
{
    public class MemberWechatController : CommunityControllerBase
    {
        private readonly ILogger _logger;
        WechatCoreDbContext wechatCoreDb;
        WechatOptions wechatOptions;
        string GetAccessToken()
        {
            return AccessTokenContainer.TryGetAccessToken(wechatOptions.AppId, wechatOptions.AppSecret);
        }

        public MemberWechatController(IOptions<WechatOptions> wechatOptions
            , WechatCoreDbContext wechatCoreDb
            , ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
        : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<MemberWechatController>();
            this.wechatCoreDb = wechatCoreDb;
            this.wechatOptions = wechatOptions.Value;
        }


        [Authorize]
        public ActionResult Index(string communityFlag, string appFlag)
        {
            ViewData.Model = new CommunityArgsModel()
            {
                AppFlag = appFlag,
                CommunityFlag = communityFlag
            };
            return View();
        }

        [Authorize]
        public ActionResult Bind()
        {
            var memberId = GetMemberId();
            var count = wechatCoreDb.QueryMemberWechat()
            .Where(m => m.MemberId == memberId)
            .Count();
            if (count > 0)
            {
                return RedirectToAction("BindInfo");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult BindInfo()
        {
            var memberId = GetMemberId();
            var count = wechatCoreDb.QueryMemberWechat()
            .Where(m => m.MemberId == memberId)
            .Count();
            if (count <= 0)
            {
                return RedirectToAction("Bind");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult BindQRCodeSence()
        {
            var memberId = GetMemberId();
            var member = wechatCoreDb.QueryMemberWechat()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();
            if (member != null) throw new Exception("已经绑定微信");
            var code = Common.CommonUtil.CreateNoncestr(15);

            WechatQRScene model = new WechatQRScene()
            {
                Category = "BindMember",
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
            wechatCoreDb.AddToMemberWeChatBindTask(new MemberWeChatBindTask()
            {
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                Code = model.SceneId.ToString(),
                MemberId = GetMemberId(),
                OpenId = "",
                Status = MemberWeChatBindTaskStatus.未使用
            });
            wechatCoreDb.SaveChanges();

            using (var stream = new MemoryStream())
            {
                QrCodeApi.ShowQrCode(model.QrCodeTicket, stream);
                byte[] data = stream.ToArray();

                return File(data, "image/jpeg");
            }
        }


        public ActionResult Login(string returnUrl)
        {
            var clientId = Common.CommonUtil.CreateNoncestr(45);
            ViewData.Model = new LoginModel()
            {
                ClientId = clientId,
                ReturnUrl = returnUrl
            };
            return View();
        }

        /// <summary>
        /// 此页面只能在微信打开
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginConfirm(string code)
        {
            var task = wechatCoreDb.QueryMemberWeChatBindTask()
            .Where(m => m.Code == code)
            .FirstOrDefault();
            if (task == null) throw new Exception("请先发起登陆操作");
            return View();
        }

        [Authorize]
        public ActionResult ShowImage(string mediaId)
        {
            var smallFileName = CustomerMessage.GetCacheSmallPathForTemporaryMedia(mediaId);
            var targetFileName = smallFileName;
            if (!System.IO.File.Exists(smallFileName))
            {//如果没有小图，说明没进行缩小处理过

                var fileName = CustomerMessage.GetCachePathForTemporaryMedia(mediaId);
                if (!System.IO.File.Exists(fileName))//如果没有原图，则需要下载下来
                {
                    var accessToken = GetAccessToken();
                    using (FileStream createFs = new FileStream(fileName, FileMode.Create))
                    {
                        MediaApi.Get(accessToken, mediaId, createFs);
                    }
                }
                try
                {
                    ResizeImage(fileName, smallFileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "缩放出错,文件名：{0}", fileName);
                    targetFileName = fileName; //显示的时候用原图显示。
                }
            }

            FileStream fs = new FileStream(targetFileName, FileMode.Open);
            return File(fs, "image/jpeg");
        }

        public static void ResizeImage(string fromPath, string toPath)
        {
            if (!System.IO.File.Exists(fromPath)) throw new Exception("原路径不能为空");
            using (Image<Rgba32> image = Image.Load(fromPath))
            {
                if (image.Width > 850 || image.Height > 2048)
                {
                    var options = new SixLabors.ImageSharp.Processing.ResizeOptions
                    {
                        Size = new SixLabors.Primitives.Size(850, 2048),
                        Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max
                    };

                    image.Mutate(x => x.Resize(options));
                    image.Save(toPath); // automatic encoder selected based on extension.
                }
                else
                {
                    System.IO.File.Copy(fromPath, toPath);
                }
            }
        }
    }
}
