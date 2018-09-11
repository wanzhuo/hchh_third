using System;
using System.Linq;
using ZRui.Web.ShopCommentAPIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Common;
using System.IO;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCommentPictureAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommentPictureAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// 上传评论图片
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Upload()
        {
            if (Request.Form.Files.Count == 0) throw new Exception("上传文件为空。");
            var memberId = GetMemberId();
            string uniqueString = GetUniqueValue();
            string fileType = GetFileType(Request.Form.Files[0].FileName);
            string newFilename = uniqueString + fileType;
            //TODO   图片保存路径暂定
            string imgSavePath = $"{hostingEnvironment.ContentRootPath}\\CommentImg\\{newFilename}";
            Stream stream = new FileStream(imgSavePath, FileMode.CreateNew);
            Request.Form.Files[0].CopyTo(stream);
            stream.Close();

            var model = new ShopCommentPicture()
            {
                AddTime = DateTime.Now,
                MemberId = memberId,
                //ShopCommentId = args.ShopId,
                SaveFileName = newFilename
            };

            db.AddTo<ShopCommentPicture>(model);
            db.SaveChanges();

            return Success(new
            {
                picid = model.Id
            });
        }

        /// <summary>
        /// 获取评论图片
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopCommentPicture>()
                .Where(m => !m.IsDel)
                .Where(m => m.Id == id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论图片不存在");
            //TODO   图片保存路径暂定
            string imgSavePath = $"{hostingEnvironment.ContentRootPath}\\CommentImg\\{model.SaveFileName}";
            return File(new FileStream(imgSavePath, FileMode.Open), "application/x-img", model.SaveFileName);
        }

        /// <summary>
        /// 删除评论图片
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDelete([FromBody] PictureArgModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopCommentPicture>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论图片不存在");
            model.IsDel = true;
            db.SaveChanges();
            return Success(model);
        }

        private static string GetFileType(string filename)
        {
            int lastIndex = filename.LastIndexOf('.');
            if (lastIndex == -1)
                return "";
            return filename.Substring(lastIndex);
        }


        private static object _UniqueLock = new object();
        /// <summary>
        /// 获取唯一值
        /// </summary>
        /// <returns></returns>
        private static string GetUniqueValue()
        {
            string rtn;
            lock (_UniqueLock)
            {
                rtn = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            }
            return rtn;
        }

    }
}
