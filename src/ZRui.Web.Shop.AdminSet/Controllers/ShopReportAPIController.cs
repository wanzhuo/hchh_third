using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Models;

namespace ZRui.Web.Controllers
{

    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class ShopReportAPIController : CommunityApiControllerBase
    {
        ShopDbContext db;
        readonly IHostingEnvironment hostingEnvironment;
        public ShopReportAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IHostingEnvironment hostingEnvironment)
            : base(communityService, options, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public APIResult ShopReport([FromBody]ShopReportModels model)
        {
            try
            {
                if (model==null)throw new Exception("参数有误");
                
                if (model.StartDateTime> model.EndDateTime) {
                    throw new Exception("开始时间不能大于结束时间");
                }

                var query = from order in db.ShopOrders join shop in db.Shops on 
                            order.ShopId equals shop.Id where !shop.IsDel&&order.Status== ShopOrderStatus.已完成 
                            && order.AddTime>=model.StartDateTime&&order.AddTime<=model.EndDateTime group 
                            order by order.ShopId into e select new{
                                transactionmoney = e.Sum(p=>p.Payment).Value / 100M,
                                transactioncount = e.Count(),
                                shopname = e.First().Shop.Name
                            };

                return Success(query);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }



        }
    }
}
