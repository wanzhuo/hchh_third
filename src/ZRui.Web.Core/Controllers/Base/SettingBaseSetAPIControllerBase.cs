using System;
using System.Linq;
using ZRui.Web.SettingBaseSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SettingBaseSetAPIControllerBase : CommunityApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        IDbContextFactory dbFactory;
        public SettingBaseSetAPIControllerBase(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , IDbContextFactory dbFactory
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options,memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.dbFactory = dbFactory;
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            using (var db = dbFactory.Create(_communityService, args.CommunityFlag, args.AppFlag))
            {
                var query = db.Query<SettingBase>()
                         .Where(m => !m.IsDel);

                var list = query
                    .OrderByDescending(m => m.Id)
                    .Select(m => new RowItem()
                    {
                        Detail = m.Detail,
                        Flag = m.Flag,
                        GroupFlag = m.GroupFlag,
                        Id = m.Id,
                        IsDel = m.IsDel,
                        SettingType = m.SettingType,
                        Value = m.Value
                    })
                    .ToList();

                return Success(new GetListModel()
                {
                    Items = list
                });
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            args.OrderName = args.OrderName ?? "";
            using (var db = dbFactory.Create(_communityService, args.CommunityFlag, args.AppFlag))
            {
                if (args.PageSize <= 0) args.PageSize = 10;
                if (args.PageIndex == 0) args.PageIndex = 1;
                var query = db.Query<SettingBase>()
                         .Where(m => !m.IsDel);

                var list = query
                    .Select(m => new RowItem()
                    {
                        Detail = m.Detail,
                        Flag = m.Flag,
                        GroupFlag = m.GroupFlag,
                        Id = m.Id,
                        IsDel = m.IsDel,
                        SettingType = m.SettingType,
                        Value = m.Value
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


        [HttpPost]
        [Authorize]
        public APIResult SetValue([FromBody]SetValueArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Flag)) throw new ArgumentNullException("Flag");
            args.GroupFlag = args.GroupFlag ?? "";

            using (var db = dbFactory.Create(_communityService, args.CommunityFlag, args.AppFlag))
            {
                db.SetSettingValue(args.Flag, args.Value, args.GroupFlag);
                db.SaveChanges();

                return Success();
            }
        }

        [HttpPost]
        [Authorize]
        public APIResult GetValue([FromBody]GetValueArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Flag)) args.Flag = System.Guid.NewGuid().ToString();
            using (var db = dbFactory.Create(_communityService, args.CommunityFlag, args.AppFlag))
            {
                var v = db.GetSettingValue<string>(args.Flag, args.GroupFlag);
                return Success<string>(v);
            }
        }
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            using (var db = dbFactory.Create(_communityService, args.CommunityFlag, args.AppFlag))
            {
                var model = db.GetSingle<SettingBase>(args.Id);
                if (model == null) throw new Exception("记录不存在");

                model.IsDel = true;
                db.SaveChanges();

                return Success();
            }

        }
    }
}
