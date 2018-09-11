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
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopCommentAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopCommentAPIController(ICommunityService communityService
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
        /// 添加商铺评论
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Add([FromBody] AddArgsModel args)
        {
            var memberId = GetMemberId();
            var model = new ShopComment()
            {
                AddTime = DateTime.Now,
                MemberId = memberId,
                ShopId = args.ShopId,
                KeyWord = args.KeyWord,
                Content = args.Content,
                Grade = args.Grade
            };
            db.AddTo<ShopComment>(model);
            db.SaveChanges();

            db.Query<ShopCommentPicture>()
                .Where(m => args.PicIds.IndexOf(m.Id) > -1)
                .ToList()
                .ForEach(m => m.ShopCommentId = model.Id);
            db.SaveChanges();

            return Success(model);
        }


        /// <summary>
        /// 修改评论
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Update([FromBody] SetCommentArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopComment>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论不存在");

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
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDelete([FromBody] DelCommentArgsModel args)
        {
            var memberId = GetMemberId();
            var model = db.Query<ShopComment>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.Id == args.Id)
                .FirstOrDefault();
            if (model == null) throw new Exception("指定评论不存在");

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
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
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
                .AsNoTracking()
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
    }
}
