﻿using System;
using System.Linq;
using ZRui.Web.ShopBrandActorSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopBrandActorSetAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopBrandActorSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , ShopDbContext db
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }


        [HttpPost]
        [Authorize]
        public APIResult GetList([FromBody]GetListArgsModel args)
        {
            var query = db.Query<ShopBrandActor>()
                     .Where(m => !m.IsDel);

            if (args.ShopBrandId.HasValue)
            {
                query = query.Where(m => m.ShopBrandId == args.ShopBrandId);
            }

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    ActorType = m.ActorType,
                    MemberId = m.MemberId,
                    ShopBrandId = m.ShopBrandId,
                    ShopBrandName = m.ShopBrand.Name
                })
                .ToList();

            return Success(new GetListModel()
            {
                Items = list
            });
        }

        [HttpPost]
        [Authorize]
        public APIResult GetPagedList([FromBody]GetPagedListArgsModel args)
        {
            args.OrderName = args.OrderName ?? "";
            if (args.PageSize <= 0) args.PageSize = 10;
            if (args.PageIndex == 0) args.PageIndex = 1;
            var query = db.Query<ShopBrandActor>()
                     .Where(m => !m.IsDel);

            if (args.ShopBrandId.HasValue)
            {
                query = query.Where(m => m.ShopBrandId == args.ShopBrandId);
            }


            var list = query
                .Select(m => new RowItem()
                {
                    AddIp = m.AddIp,
                    AddTime = m.AddTime,
                    AddUser = m.AddUser,
                    Id = m.Id,
                    ActorType = m.ActorType,
                    MemberId = m.MemberId,
                    ShopBrandId = m.ShopBrandId,
                    ShopBrandName = m.ShopBrand.Name
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
            if (string.IsNullOrEmpty(args.MemberFlag)) throw new ArgumentNullException("Name");
            var shopBrand = db.GetSingle<ShopBrand>(args.ShopBrandId);
            if (shopBrand == null) throw new Exception("指定的商铺不存在");

            var member = memberDb.GetSingleMemberBase(args.MemberFlag);
            if (member == null) throw new Exception("用户纪录不存在");
            var model = new ShopBrandActor()
            {
                ShopBrand = shopBrand,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                AddUser = GetUsername(),
                MemberId = member.Id,
                ActorType = args.ActorType
            };
            db.Add<ShopBrandActor>(model);
            db.SaveChanges();

            return Success();
        }

        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = db.GetSingle<ShopBrandActor>(args.Id);
            if (model == null) throw new Exception("记录不存在");

            model.IsDel = true;
            db.SaveChanges();

            return Success();
        }
    }
}
