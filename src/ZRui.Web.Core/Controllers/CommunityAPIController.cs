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
using ZRui.Web.CommunityAPIModels;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CommunityAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        public CommunityAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            _logger = loggerFactory.CreateLogger<CommunityAPIController>();
        }



        [HttpPost]
        [Authorize]
        public APIResult<GetListModel> GetList([FromBody]GetListArgsModel args)
        {
            var username = GetUsername();
            var items = _communityService.GetListForBase(username);
            return Success<GetListModel>(new GetListModel
            {
                Items = items
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult<GetListForVueRouteModel> GetListForVueRoute([FromBody]GetListArgsModel args)
        {
            var username = GetUsername();
            var items = _communityService.GetList(username);
            return Success<GetListForVueRouteModel>(new GetListForVueRouteModel(items));
        }

        [HttpPost]
        [Authorize]
        public APIResult<GetSingleModel> GetSingle([FromBody]GetSingleArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

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
        public APIResult<GetMembersModel> GetMembers([FromBody]GetMembersArgsModel args)
        {
            var username = GetUsername();
            var community = _communityService.GetSingle(args.CommunityFlag);

            return Success(new GetMembersModel()
            {
                Members = community.Members
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult AddMember([FromBody]AddMemberArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);

            _communityService.AddMember(flag, args.Username);
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult AddManager([FromBody]AddManagerArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);

            _communityService.AddManager(flag, args.Username);

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult RemoveMember([FromBody]RemoveMemberArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);
            _communityService.RemoveMember(flag, args.Username);

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult RemoveManager([FromBody]RemoveManagerArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);
            _communityService.RemoveManager(flag, args.Username);

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetCommunityName([FromBody]SetCommunityNameArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);
            _communityService.SetName(flag, args.Name);

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetCommunityBase([FromBody]SetCommunityBaseArgsModel args)
        {
            var username = GetUsername();
            var flag = args.CommunityFlag;

            checkIsManager(flag, username);
            _communityService.SetBase(flag, args.Name, args.Ico);

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppName([FromBody]SetAppNameArgsModel args)
        {
            var username = GetUsername();
            var communityFlag = args.CommunityFlag;

            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(args.AppFlag)) throw new ArgumentNullException("AppFlag");

            _communityService.SetAppName(communityFlag, args.AppFlag, args.Name);
            return Success();
        }


        [HttpPost]
        [Authorize]
        public APIResult SetAppBase([FromBody]SetAppBaseArgsModel args)
        {
            var username = GetUsername();
            var communityFlag = args.CommunityFlag;

            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(args.Name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(args.AppFlag)) throw new ArgumentNullException("AppFlag");

            _communityService.SetAppBase(communityFlag, args.AppFlag, args.Name,args.Ico);
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppIsDisabled([FromBody]SetAppIsDisabledArgsModel args)
        {
            var username = GetUsername();
            var communityFlag = args.CommunityFlag;

            if (string.IsNullOrEmpty(communityFlag)) throw new ArgumentNullException("communityFlag");
            if (string.IsNullOrEmpty(args.AppFlag)) throw new ArgumentNullException("AppFlag");
            _communityService.SetAppIsDisabled(communityFlag, args.AppFlag, args.IsDisabled);
            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult<CommunityAppSettings> GetAppSettings([FromBody]GetAppSettingsArgsModel args)
        {
            var username = GetUsername();
            var communityFlag = args.CommunityFlag;
            var appFlag = args.AppFlag;

            var community = _communityService.GetSingle(communityFlag);
            checkIsManager(community, username);

            //从群组中获得应用
            var app = community.Apps.Where(m => m.Flag == appFlag).FirstOrDefault();
            if (app == null) throw new Exception("应用不存在");

            return Success(app.AppSettings);
        }

        [HttpPost]
        [Authorize]
        public APIResult SetAppSettings([FromBody]SetAppSettingsArgsModel args)
        {
            var username = GetUsername();
            var communityFlag = args.CommunityFlag;
            var appFlag = args.AppFlag;

            checkIsManager(communityFlag, username);
            _communityService.SetAppSettings(communityFlag, appFlag, args.Settings);

            return Success();
        }
    }

}
