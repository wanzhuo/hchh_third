using System;
using System.Linq;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using ZRui.Web.Core.Wechat;
using ZRui.Web.MemberAmountChangeLogForWechatAPIModels;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberAmountChangeLogForWechatAPIController : WechatApiControllerBase
    {
        FinanceDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public MemberAmountChangeLogForWechatAPIController(
            IOptions<MemberAPIOptions> options
            , FinanceDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<GetPagedListModel> GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            var memberId = GetMemberId();
            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;

            var query = db.Query<MemberAmountChangeLog>()
                     .Where(m => m.MemberId == memberId);

            var list = query
                .Select(m => new RowItem()
                {
                    AddTime = m.AddTime,
                    Amount = m.Amount,
                    NowAmount = m.NowAmount,
                    OriginalAmount = m.OriginalAmount,
                    Title = m.Title
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
    }
}
