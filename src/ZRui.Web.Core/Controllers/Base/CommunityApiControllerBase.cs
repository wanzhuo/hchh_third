using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace ZRui.Web.Controllers
{
    public class CommunityApiControllerBase : ApiControllerBase
    {
        protected ICommunityService _communityService;
        protected MemberAPIOptions _options;
        protected MemberDbContext memberDb;

        public CommunityApiControllerBase(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb)
        {
            _communityService = communityService;
            _options = _options = options.Value;
            this.memberDb = memberDb;
        }

        private string _memberPasswordMD5Key = "kspig";
        protected string MemberPasswordToMD5(string password)
        {
            MD5 md5Hash = MD5.Create();
            var source = string.Format("{0}__{1}", _memberPasswordMD5Key, password);
            string hash = CommonUtil.GetMD5Hash(source);
            return hash;
        }

        protected string GetUsername()
        {
            return "member" + GetMemberId();
        }

        protected string GetTruename()
        {
            return this.HttpContext.GetTruename();
        }

        protected int GetMemberId()
        {
            var loginFlag = User.Identity.Name;
            return this.memberDb.GetMemberIdByLoginFlag(loginFlag);
            //return this.HttpContext.GetMemberId();
        }

        protected void checkIsMember(string communityFlag)
        {
            checkIsMember(communityFlag, GetUsername());
        }

        protected void checkIsMember(string communityFlag, string username)
        {
            var community = _communityService.GetSingle(communityFlag);
            checkIsMember(community, username);
        }

        protected void checkIsMember(Community community, string username)
        {
            if (community == null)
            {
                throw new Exception("群组不存在");
            }
            //判定当前用户是否在群组中
            if (!community.IsMember(username))
            {
                throw new Exception("你不在群组中");
            }
        }

        protected void checkIsManager(string communityFlag, string username)
        {
            var community = _communityService.GetSingle(communityFlag);
            checkIsManager(community, username);
        }

        protected void checkIsManager(Community community, string username)
        {
            checkIsMember(community, username);

            var isManager = _options.IsRoot(username);
            if (!isManager)
            {
                isManager = community.IsManager(username);
                if (!isManager) throw new Exception("操作错误：你不是管理员");
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //如果已经登陆上去才进行验证
            if (context.HttpContext.User.Identity.IsAuthenticated && context.ActionArguments.Count > 0)
            {
                var communityArgsModel = context.ActionArguments["args"] as CommunityArgsModel;
                if (communityArgsModel != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(communityArgsModel.CommunityFlag))
                        {
                            throw new Exception("缺少CommunityFlag");
                        }

                        var community = _communityService.GetSingle(communityArgsModel.CommunityFlag);
                        checkIsMember(community, GetUsername());
                        //checkIsMember(communityArgsModel.CommunityFlag, GetUsername());
                        var app = community.Apps.Where(m => m.Flag == communityArgsModel.AppFlag).FirstOrDefault();
                        if (app == null) throw new Exception("应用程序纪录不存在");
                        var appPermission = _communityService.GetSingleAppPermission(app.Url);
                        if (appPermission == null) throw new Exception("应用程序未注册");
                        if (!appPermission.Sources.Select(m => m.ToLower()).Contains(context.HttpContext.Request.Path.Value.ToLower())) throw new Exception("无权限访问此资源");
                        //TODO:这里没有判定当前的应用，是否当前的群组能够使用。
                    }
                    catch (Exception ex)
                    {
                        context.Result = Json(Error(ex.Message));
                        return;
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
