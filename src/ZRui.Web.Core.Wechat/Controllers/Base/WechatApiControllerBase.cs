using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.Controllers
{
    public class WechatApiControllerBase : ApiControllerBase
    {
        protected MemberAPIOptions _options;
        protected MemberDbContext memberDb;
        protected WechatCoreDbContext wechatCoreDb;

        public WechatApiControllerBase(IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            )
        {
            _options = options.Value;
            this.memberDb = memberDb;
            this.wechatCoreDb = wechatCoreDb;
        }

        //protected int GetOrCreateMemberIdByWechatOpenId()
        //{
        //    var openId = GetUsername();
        //    return GetOrCreateMemberIdByWechatOpenId(openId);
        //}

        //protected int GetOrCreateMemberIdByWechatOpenId(string openId)
        //{
        //    var memberId = wechatCoreDb.Query<MemberWechat>()
        //        .Where(m => !m.IsDel)
        //        .Where(m => m.OpenId == openId)
        //        .Select(m => m.MemberId)
        //        .FirstOrDefault();
        //    //1、判定MemberWechat中是否存在，如果不存在，则需要添加
        //    if (memberId <= 0)
        //    {
        //        var email = $"{openId}@wechat.tmp";
        //        var member = memberDb.Query<Member>()
        //            .Where(m => !m.IsDel)
        //            .Where(m => m.Email == email)
        //            .FirstOrDefault();
        //        //2、判定Member中是否已经有了指定email的存在，如果不存在，则需要添加S
        //        if (member == null)
        //        {
        //            //3、添加第二点提到的
        //            member = new Member()
        //            {
        //                Email = $"{openId}@wechat.tmp",
        //                Password = CommonUtil.CreateNoncestr(8),
        //                Status = MemberStatus.正常,
        //                Truename = "wechat member",
        //                LastLoginTime = DateTime.Now,
        //                RegTime = DateTime.Now
        //            };

        //            memberDb.AddToMember(member);
        //            memberDb.SaveChanges();
        //        }

        //        //4、添加第一点提到的
        //        var memberwechat = new MemberWechat()
        //        {
        //            MemberId = member.Id,
        //            OpenId = openId
        //        };

        //        wechatCoreDb.AddToMemberWechat(memberwechat);
        //        wechatCoreDb.SaveChanges();

        //        memberId = memberwechat.MemberId;
        //    }

        //    return memberId;
        //}

        protected string GetUsername()
        {
            return User.Identity.Name;
        }

        protected string GetTruename()
        {
            return this.HttpContext.GetTruename();
        }

        protected int GetMemberId()
        {
            var loginFlag = User.Identity.Name;
            return this.wechatCoreDb.GetMemberIdByLoginFlag(loginFlag);
        }

        protected string GetOpenId()
        {
            var loginFlag = User.Identity.Name;
            var m = this.wechatCoreDb.GetSingleMemberLogin(loginFlag);
            if (m == null)
            {
                HttpContext.Response.StatusCode = 401;
                throw new Exception("未登录");
            }
            return m.GetloginSettingValue<string>("openId");
        }
    }
}
