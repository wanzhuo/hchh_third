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
    public class ShopManagerApiControllerBase : ApiControllerBase
    {
        protected MemberAPIOptions _options;
        protected ShopDbContext db;
        protected MemberDbContext memberDb;

        public ShopManagerApiControllerBase(IOptions<MemberAPIOptions> options
            , ShopDbContext db,MemberDbContext memberDb)
        {
            _options = _options = options.Value;
            this.db = db;
            this.memberDb = memberDb;
        }
        /// <summary>
        /// 检查是否拥有指定的店铺权限，如果没有则抛出异常
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="actorTypes"></param>
        protected void CheckShopActor(int shopId, params ShopActorType[] actorTypes)
        {
            var memberId = GetMemberId();
            var actors = db.Query<ShopActor>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.ShopId == shopId)
                .Select(m => m.ActorType)
                .ToList();
            //如果有交集，代表拥有指定的权限
            if (actors.Intersect(actorTypes).Count() <= 0) throw new Exception("用户缺少指定的店铺权限");
        }
        /// <summary>
        /// 检查是否拥有指定的店铺品牌权限，如果没有则抛出异常
        /// </summary>
        /// <param name="shopBrandId"></param>
        /// <param name="actorTypes"></param>
        protected void CheckShopBrandActor(int shopBrandId, params ShopBrandActorType[] actorTypes)
        {
            var memberId = GetMemberId();
            var actors = db.Query<ShopBrandActor>()
                .Where(m => !m.IsDel)
                .Where(m => m.MemberId == memberId)
                .Where(m => m.ShopBrandId == shopBrandId)
                .Select(m => m.ActorType)
                .ToList();
            //如果有交集，代表拥有指定的权限
            if (actors.Intersect(actorTypes).Count() <= 0) throw new Exception("用户缺少指定的店铺品牌权限");
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
            var login = this.memberDb.GetSingleMemberLogin(loginFlag);
            if (login == null) throw new Exception("未登录");
            if (!login.MemberId.HasValue) throw new Exception("用户未登陆");
            return login.MemberId.Value;
        }
    }
}
