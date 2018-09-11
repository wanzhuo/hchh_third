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
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberSetAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        public MemberSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<MemberSetAPIController>();
        }

        [HttpPost]
        [Authorize]
        public APIResult<GetListModel> GetList([FromBody]GetListArgsModel args)
        {
            var query = memberDb.QueryMember()
                     .Where(m => !m.IsDel);

            if (args.Status.HasValue)
            {
                query = query.Where(m => m.Status == args.Status.Value);
            }
            if (!string.IsNullOrEmpty(args.Email))
            {
                query = query.Where(m => m.Email.Contains(args.Email));
            }
            if (!string.IsNullOrEmpty(args.Truename))
            {
                query = query.Where(m => m.Truename.Contains(args.Truename));
            }

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Email = m.Email,
                    EmailIsValid = m.EmailIsValid,
                    LastLoginIP = m.LastLoginIP,
                    LastLoginTime = m.LastLoginTime,
                    LoginCount = m.LoginCount,
                    Truename = m.Truename,
                    RegIP = m.RegIP,
                    RegTime = m.RegTime,
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

            var query = memberDb.QueryMember()
                     .Where(m => !m.IsDel);

            if (args.Status.HasValue)
            {
                query = query.Where(m => m.Status == args.Status.Value);
            }
            if (!string.IsNullOrEmpty(args.Email))
            {
                query = query.Where(m => m.Email.Contains(args.Email));
            }
            if (!string.IsNullOrEmpty(args.Truename))
            {
                query = query.Where(m => m.Truename.Contains(args.Truename));
            }

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    Id = m.Id,
                    Email = m.Email,
                    EmailIsValid = m.EmailIsValid,
                    LastLoginIP = m.LastLoginIP,
                    LastLoginTime = m.LastLoginTime,
                    LoginCount = m.LoginCount,
                    Truename = m.Truename,
                    RegIP = m.RegIP,
                    RegTime = m.RegTime,
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
                if (string.IsNullOrEmpty(args.Email)) throw new ArgumentNullException("email");
                
                var model = new Member()
                {
                    Email = args.Email,
                    Password = MemberPasswordToMD5(args.Password),
                    Truename = args.Truename,
                    RegIP = GetIp(),
                    RegTime = DateTime.Now,
                    LastLoginIP = GetIp(),
                    LastLoginTime = DateTime.Now,
                    Status = MemberStatus.正常
                };
                memberDb.AddToMember(model);
                memberDb.SaveChanges();
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

                var model = memberDb.QueryMember()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");
                if (model.Email != args.Email)
                {//如果Email有改变，则需要判断一下email有没有被使用
                    if (memberDb.MemberEmailIsUsed(model.Email))
                    {
                        throw new Exception("Email is Used");
                    }
                }
                model.Truename = args.Truename;
                model.Email = args.Email;
                model.EmailIsValid = args.EmailIsValid;
                model.Status = args.Status;
                memberDb.SaveChanges();
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

                var viewModel = memberDb.QueryMember()
                    .Where(m => m.Id == args.ID)
                    .Select(m => new GetSingleModel()
                    {
                        Id = m.Id,
                        Email = m.Email,
                        EmailIsValid = m.EmailIsValid,
                        LastLoginIP = m.LastLoginIP,
                        LastLoginTime = m.LastLoginTime,
                        LoginCount = m.LoginCount,
                        Truename = m.Truename,
                        RegIP = m.RegIP,
                        RegTime = m.RegTime,
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

                var model = memberDb.QueryMember()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                model.IsDel = true;
                memberDb.SaveChanges();

                return Success();

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult ResetPassword([FromBody]ResetPasswoordArgsModel args)
        {
            try
            {

                var model = memberDb.QueryMember()
                    .Where(m => m.Id == args.Id)
                    .FirstOrDefault();
                if (model == null) throw new Exception("数据库记录不存在");

                var password = CommonUtil.CreateNoncestr(12);
                model.Password = MemberPasswordToMD5(password);
                memberDb.SaveChanges();

                return Success<string>(password);

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
