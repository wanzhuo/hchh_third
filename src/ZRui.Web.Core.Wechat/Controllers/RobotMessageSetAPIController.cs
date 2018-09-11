using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZRui.Web.RobotMessageSetAPIModels;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using ZRui.Web.Core;
using Microsoft.Extensions.Options;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class RobotMessageSetAPIController : CommunityApiControllerBase
    {
        WechatCoreDbContext wechatCoreDb;
        private readonly ILogger _logger;
        public RobotMessageSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            this.wechatCoreDb = wechatCoreDb;
            _logger = loggerFactory.CreateLogger<RobotMessageSetAPIController>();
        }

        [HttpPost]
        [Authorize]
        public APIResult<GetListModel> GetList([FromBody]GetListArgsModel args)
        {
            var query = wechatCoreDb.QueryRobotMessage()
                     .Where(m => !m.IsDel);

            if (args.Status.HasValue)
            {
                query = query.Where(m => m.Status == args.Status.Value);
            }
            if (!string.IsNullOrEmpty(args.Question))
            {
                query = query.Where(m => m.Question.Contains(args.Question));
            }
            if (args.QuestionType.HasValue)
            {
                query = query.Where(m => m.QuestionType == args.QuestionType.Value);
            }

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Question = m.Question,
                    QuestionType = m.QuestionType,
                    Answer = m.Answer,
                    Status = m.Status
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult<GetPagedListModel> GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex <= 0) args.PageIndex = 1;

            var query = wechatCoreDb.QueryRobotMessage()
                     .Where(m => !m.IsDel);

            if (args.Status.HasValue)
            {
                query = query.Where(m => m.Status == args.Status.Value);
            }
            if (!string.IsNullOrEmpty(args.Question))
            {
                query = query.Where(m => m.Question.Contains(args.Question));
            }
            if (args.QuestionType.HasValue)
            {
                query = query.Where(m => m.QuestionType == args.QuestionType.Value);
            }

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Question = m.Question,
                    QuestionType = m.QuestionType,
                    Answer = m.Answer,
                    Status = m.Status
                })
                .ToPagedList(args.PageIndex, args.PageSize);

            return Success(new GetPagedListModel()
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = list.ToList()
            });
        }


        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Question)) throw new ArgumentNullException("Question");
                var isExit = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Question == args.Question)
                    .Where(m => !m.IsDel)
                    .Count() > 0;
                if (isExit) throw new Exception("问题已经存在");

                var model = new RobotMessage()
                {
                    Question = args.Question,
                    QuestionType = args.QuestionType,
                    Answer = args.Answer,
                    Status = RobotMessageStatus.正常
                };
                wechatCoreDb.AddToRobotMessage(model);
                wechatCoreDb.SaveChanges();
                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult Update([FromBody]UpdateArgsModel args)
        {
            try
            {
                var model = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                model.Answer = args.Answer;
                wechatCoreDb.SaveChanges();
                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]GetSingleArgsModel args)
        {
            try
            {
                var viewModel = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Id == args.Id)
                    .Select(m => new GetSingleModel()
                    {
                        Id = m.Id,
                        Question = m.Question,
                        QuestionType = m.QuestionType,
                        Answer = m.Answer,
                        Status = m.Status
                    })
                    .FirstOrDefault();
                if (viewModel == null) throw new Exception("记录不存在");
                return Success(viewModel);

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]SetIsDeleteArgsModel args)
        {
            try
            {

                var model = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                model.IsDel = true;
                wechatCoreDb.SaveChanges();

                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetStatus([FromBody]SetStatusArgsModel args)
        {
            try
            {

                var model = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                model.Status = args.Status;
                wechatCoreDb.SaveChanges();

                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAnswer([FromBody]SetAnswerArgsModel args)
        {
            try
            {

                var model = wechatCoreDb.QueryRobotMessage()
                    .Where(m => m.Question == args.Question)
                    .Where(m => !m.IsDel)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                model.Answer = args.Answer;
                wechatCoreDb.SaveChanges();

                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
