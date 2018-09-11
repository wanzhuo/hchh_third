using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace ZRui.Web.Controllers
{

    /// <summary>
    /// 发票管理
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopInvoiceAPIController : ShopManagerApiControllerBase
    {
        ShopDbContext db;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopInvoiceAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            _mapper = mapper;
            this._hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// 操作已开发票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult MakeOutInvoice([FromBody]MakeOutInvoiceModel input)
        {
            //var memberId = GetMemberId();
            int id = input.Id;
            var invoiceRequset = db.MemberInvoiceRequest.Find(id);
            if (invoiceRequset.IsDel)
            {
                return Error("记录不存在");
            }

            invoiceRequset.State = ProcessState.已开发票;
            db.SaveChanges();

            return Success("处理成功");
        }

        /// <summary>
        /// 获取发票请求列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetPageList([FromBody]GetPagedListInvoiceRequsetModel input)
        {
            var query = db.MemberInvoiceRequest
                .Where(m =>
                    m.ShopId.Equals(input.ShopId) &&
                    m.ShopOrderId.ToString().Contains(input.Keyword) &&
                    !m.IsDel);
            var statu = (ProcessState)input.State;
            if (input.State != 0)
            {
                query = query.Where(m => m.State.Equals(statu));
            }
            query = query.OrderByDescending(m => m.CreateTime).Include(m => m.ShopOrder)
                .Include(m => m.MemberInvoiceTitle);
            var invoiceRequsetDatas = query.ToPagedList(input.PageIndex, input.PageSize);

            var result = _mapper.Map<List<ResultGetPageListModel>>(invoiceRequsetDatas.ToList());

            return Success(new GetPagedListModel()
            {
                PageIndex = invoiceRequsetDatas.PageIndex,
                PageSize = invoiceRequsetDatas.PageSize,
                TotalCount = invoiceRequsetDatas.TotalItemCount,
                Items = result
            });
        }
    }
}
