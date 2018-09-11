using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ZRui.Web.Common;
using ZRui.Web.CommunitySetAPIModels;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CommunitySetAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        public CommunitySetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<CommunitySetAPIController>();
        }

        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            try
            {
                var username = GetUsername();
                var items = _communityService.GetListForBase(username);
                return Success<GetListModel>(new GetListModel
                {
                    Items = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "GetList有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            try
            {

                if (args.PageSize <= 0) args.PageSize = 10;
                if (args.PageIndex <= 0) args.PageIndex = 1;

                var items = _communityService.GetListForBase()
                    .ToPagedList(args.PageIndex, args.PageSize);

                return Success(new GetPagedListModel()
                {
                    PageIndex = items.PageIndex,
                    PageSize = items.PageSize,
                    TotalCount = items.TotalItemCount,
                    Items = items.ToList()
                });
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
            var username = GetUsername();
            var flag = args.CurrentCommunityFlag;

            var community = _communityService.GetSingle(flag);

            return Success(new GetSingleModel()
            {
                Apps = community.Apps,
                Flag = community.Flag,
                Members = community.Members,
                Name = community.Name,
                Ico = community.Ico,
                Managers = community.Managers
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetMembers([FromBody]GetMembersArgsModel args)
        {
            try
            {
                var username = GetUsername();
                var flag = args.CurrentCommunityFlag;


                var community = _communityService.GetSingle(flag);
                if (community == null)
                {
                    throw new Exception("群组不存在");
                }
                //判定当前用户是否在群组中
                if (!community.IsMember(username))
                {
                    throw new Exception("你不在群组中");
                }

                return Success(new GetMembersModel()
                {
                    Members = community.Members
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "GetMembers有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult Add([FromBody]AddArgsModel args)
        {
            try
            {
                var username = GetUsername();

                _communityService.Add(args.Name);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Add有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult AddMember([FromBody]AddMemberArgsModel args)
        {
            try
            {
                var username = GetUsername();
                var flag = args.CurrentCommunityFlag;


                _communityService.AddMember(flag, args.Username);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "AddMember有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult AddManager([FromBody]AddManagerArgsModel args)
        {
            try
            {
                var username = GetUsername();
                _communityService.AddManager(args.CurrentCommunityFlag, args.Username);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "AddManager有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult RemoveMember([FromBody]RemoveMemberArgsModel args)
        {
            try
            {
                var username = GetUsername();
                _communityService.RemoveMember(args.CurrentCommunityFlag, args.Username);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "RemoveMember有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult RemoveManager([FromBody]RemoveManagerArgsModel args)
        {
            try
            {
                var username = GetUsername();

                _communityService.RemoveManager(args.CurrentCommunityFlag, args.Username);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "RemoveManager有错误发生");
                return Error(ex.Message);
            }
        }



        [HttpPost]
        [Authorize]
        public APIResult SetCommunityName([FromBody]SetCommunityNameArgsModel args)
        {
            var username = GetUsername();

            _communityService.SetName(args.CurrentCommunityFlag, args.Name);
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetCommunityBase([FromBody]SetCommunityBaseArgsModel args)
        {
            var username = GetUsername();

            _communityService.SetBase(args.CurrentCommunityFlag, args.Name, args.Ico);
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult AddApp([FromBody]AddAppArgsModel args)
        {
            try
            {
                var username = GetUsername();
                _communityService.AddApp(args.CurrentCommunityFlag, args.Name, args.Url, args.Ico);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "AddManager有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult RemoveApp([FromBody]RemoveAppArgsModel args)
        {
            try
            {
                var username = GetUsername();
                _communityService.RemoveApp(args.CurrentCommunityFlag, args.CurrentAppFlag);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "RemoveManager有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppName([FromBody]SetAppNameArgsModel args)
        {
            try
            {
                var username = GetUsername();

                if (string.IsNullOrEmpty(args.CurrentCommunityFlag)) throw new ArgumentNullException("CurrentCommunityFlag");
                if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("name");
                if (string.IsNullOrEmpty(args.CurrentAppFlag)) throw new ArgumentNullException("CurrentAppFlag");

                _communityService.SetAppName(args.CurrentCommunityFlag, args.CurrentAppFlag, args.Name);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "UpdateAppName有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppBase([FromBody]SetAppBaseArgsModel args)
        {
            try
            {
                var username = GetUsername();

                if (string.IsNullOrEmpty(args.CurrentCommunityFlag)) throw new ArgumentNullException("CurrentCommunityFlag");
                if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("name");
                if (string.IsNullOrEmpty(args.CurrentAppFlag)) throw new ArgumentNullException("CurrentAppFlag");

                _communityService.SetAppBase(args.CurrentCommunityFlag, args.CurrentAppFlag, args.Name, args.Ico);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "UpdateAppName有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppIsDisabled([FromBody]SetAppIsDisabledArgsModel args)
        {
            try
            {
                var username = GetUsername();

                if (string.IsNullOrEmpty(args.CurrentCommunityFlag)) throw new ArgumentNullException("CurrentCommunityFlag");
                if (string.IsNullOrEmpty(args.CurrentAppFlag)) throw new ArgumentNullException("CurrentAppFlag");
                _communityService.SetAppIsDisabled(args.CurrentCommunityFlag, args.CurrentAppFlag, args.IsDisabled);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "SetAppIsDisabled有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppIsDefaultOpen([FromBody]SetAppIsDefaultOpenArgsModel args)
        {
            try
            {
                var username = GetUsername();

                if (string.IsNullOrEmpty(args.CurrentCommunityFlag)) throw new ArgumentNullException("CurrentCommunityFlag");
                if (string.IsNullOrEmpty(args.CurrentAppFlag)) throw new ArgumentNullException("CurrentAppFlag");
                _communityService.SetAppIsDefaultOpen(args.CurrentCommunityFlag, args.CurrentAppFlag, args.IsDefaultOpen);
                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "SetAppIsDisabled有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult GetAppSettings([FromBody]GetAppSettingsArgsModel args)
        {
            try
            {
                var username = GetUsername();
                var communityFlag = args.CurrentCommunityFlag;
                var appFlag = args.CurrentAppFlag;

                var community = _communityService.GetSingle(communityFlag);

                //从群组中获得应用
                var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
                if (app == null) throw new Exception("应用不存在");

                return Success(app.AppSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "GetAppSettings有错误发生");
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppSettings([FromBody]SetAppSettingsArgsModel args)
        {
            try
            {
                var username = GetUsername();
                var communityFlag = args.CurrentCommunityFlag;
                var appFlag = args.CurrentAppFlag;

                _communityService.SetAppSettings(communityFlag, appFlag, args.Settings);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "UpdateAppSettings有错误发生");
                return Error(ex.Message);
            }

        }
    }

}
