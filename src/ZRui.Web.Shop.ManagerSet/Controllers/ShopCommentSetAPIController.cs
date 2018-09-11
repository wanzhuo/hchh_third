using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using ZRui.Web.ShopManager.ShopCommentSetAPIModels;
using ZRui.Web.Common;
using System.IO;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopCommentSetAPIController : ShopManagerApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommentSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// 修改评论
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody] SetCommentArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopComment>()
                .Where(m => !m.IsDel)
                //.Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论不存在");
            if (!model.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(model.ShopId.Value, ShopActorType.超级管理员);

            model.Content = args.Comment;
            model.Grade = args.Grade;
            db.SaveChanges();
            return Success(model);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody] DelCommentArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopComment>()
                .Where(m => !m.IsDel)
                //.Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论不存在");
            if (!model.ShopId.HasValue) throw new ArgumentNullException("ShopId");
            CheckShopActor(model.ShopId.Value, ShopActorType.超级管理员);

            model.IsDel = true;
            db.SaveChanges();
            return Success(model);
        }

        /// <summary>
        /// 获取商铺所有评论
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            CheckShopActor(args.ShopId, ShopActorType.超级管理员);
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopComment>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopId == args.ShopId);

            var list = query
                .Select(m => new RowItem()
                {
                    Content = m.Content,
                    Grade = m.Grade,
                    KeyWord = m.KeyWord,
                    AddIp = m.AddIp,
                    ShopId = m.ShopId,
                    AddTime = m.AddTime,
                    Id = m.Id,
                    IsDel = m.IsDel
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            list.ForEach(item =>
            {
                var picids = db.Query<ShopCommentPicture>()
                    .Where(p => p.ShopCommentId == item.Id)
                    .Select(p => p.Id)
                    .ToList();
                item.PicIds = picids;
            });

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.ToList()
            });
        }

        /// <summary>
        /// 获取评论图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetPicture(int id)
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
    }
}
