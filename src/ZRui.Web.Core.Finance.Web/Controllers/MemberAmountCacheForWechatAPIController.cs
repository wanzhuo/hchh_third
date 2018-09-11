using System;
using System.Linq;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberAmountCacheForWechatAPIController : WechatApiControllerBase
    {
        FinanceDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public MemberAmountCacheForWechatAPIController(
            IOptions<MemberAPIOptions> options
            , FinanceDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IHostingEnvironment hostingEnvironment)
            : base(options,memberDb,wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetMemberAmountCache()
        {
            var memberId = GetMemberId();
            var amountCache = db.Query<MemberAmountCache>()
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();
            return Success(amountCache);
        }
    }
}
