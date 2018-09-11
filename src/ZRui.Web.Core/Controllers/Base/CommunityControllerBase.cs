using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
namespace ZRui.Web.Controllers
{
    public class CommunityControllerBase : ControllerBase
    {
        protected ICommunityService _communityService;
        protected MemberAPIOptions _options;
        protected MemberDbContext memberDb;
        public CommunityControllerBase(ICommunityService communityService, IOptions<MemberAPIOptions> options,MemberDbContext memberDb)
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

        protected int GetMemberId()
        {
            var loginFlag = User.Identity.Name;
            return this.memberDb.GetMemberIdByLoginFlag(loginFlag);
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
    }
}
