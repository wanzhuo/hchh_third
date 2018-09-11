using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Controllers;
using ZRui.Web.ShopManager.ShopMemberSetAPIModels;
using System.Linq;
using System;
using System.Threading.Tasks;
using ZRui.Web.BLL.ServerDto;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Common;
using AutoMapper;
using ZRui.Web.Models;
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace ZRui.Web.ShopManager.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopMemberSetAPIController : ShopManagerApiControllerBase
    {
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment hostingEnvironment;
        public ShopMemberSetAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public async Task<APIResult> GetMemberList([FromBody]GetPagedListBaseModelB input)
        {
            int shopId = input.ShopId;
            //CheckShopActor(shopId, ShopActorType.超级管理员);

            var list = (from m in db.ShopMembers
                        join c in db.ShopMemberLevel on m.ShopMemberLevelId equals c.Id
                        into d
                        from g in d.DefaultIfEmpty()
                        where !m.IsDel && m.ShopId.Equals(input.ShopId)
                        orderby m.AddTime descending
                        select new MemberListModel()
                        {

                            Id = m.Id,
                            Name = m.Name,
                            Level = g == null ? "无等级" : $"{g.LevelName}({g.MemberLevel})",
                            Credits = m.Credits,
                            AddTime = m.AddTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }).AsNoTracking().ToPagedList(input.PageIndex, input.PageSize);

            var resultlist = list.ToList().Select(m => new MemberListModel()
            {

                Id = m.Id,
                Name = m.Name,
                Level = m.Level,
                Credits = m.Credits,
                AddTime = GetConsumeTime(m.Id)

            }).ToList();
            //var list = db.Query<ShopMember>()
            //    .Where(m => !m.IsDel && m.ShopId == shopId)
            //    .Select(m => new
            //    {
            //        m.Id,
            //        m.Name,
            //        Level = m.ShopMemberLevel == null ? "" : $"{m.ShopMemberLevel.LevelName}({m.ShopMemberLevel.MemberLevel})",
            //        m.Credits,
            //        AddTime = m.AddTime.ToString("yyyy-MM-dd HH:mm:ss")
            //    })

            return await Task.FromResult(Success(new
            {
                Total = list.TotalItemCount,
                input.PageIndex,
                input.PageSize,
                list = resultlist
            }));
        }


        /// <summary>
        /// 获取会员统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public async Task<APIResult> MemberStatistics([FromBody] ShopIdModel input)
        {
            var shopMembers = db.ShopMembers.Where(m => !m.IsDel && m.ShopId.Equals(input.ShopId)).AsNoTracking();
            var nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var startNowTime = nowTime.AddDays(-1);
            var endNowTime = nowTime.AddDays(-7);
            return await Task.FromResult(Success(new
            {
                Number = await shopMembers.CountAsync(),
                DayNumber = await shopMembers.Where(m => m.AddTime <= startNowTime && m.AddTime >= endNowTime).CountAsync(),
                Credits = await shopMembers.SumAsync(m => m.Credits)
            }));
        }

        /// <summary>
        /// 获取会员详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public async Task<APIResult> GetMemberDetails([FromBody] ShopMember input)
        {
            var shopMember = await db.ShopMembers.FindAsync(input.Id);
            shopMember.ShopMemberLevel = await db.ShopMemberLevel.FindAsync(shopMember.ShopMemberLevelId);
            var shopMemberModel = _mapper.Map<ShopMemberModel>(shopMember);

            #region 直接拿小程序部分的代码使用

            IQueryable<GetBillModel> unionQuery = GetRecharge(shopMember);
            var rtn = unionQuery.OrderByDescending(m => m.BillDateTime)
                .Select(m => new
                {
                    m.Amount,
                    m.BillType,
                    BillDateTime = m.BillDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });

            #endregion

            return await Task.FromResult(Success(new
            {
                shopMemberModel,
                rtn

            }));
        }

        #region 内部方法

        /// <summary>
        /// 获取用户消费记录
        /// </summary>
        /// <param name="shopMember"></param>
        /// <returns></returns>
        private IQueryable<GetBillModel> GetRecharge(ShopMember shopMember)
        {
          
            var rechargeQuery = db.Query<ShopMemberRecharge>()
                .Where(m => !m.IsDel && m.Status == ShopMemberTransactionStatus.已完成)
                .Where(m => m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = ((m.Amount + m.PresentedAmount) / 100m),
                    BillType = 1,
                    BillDateTime = m.TransactionTime
                });
            var consumeQuery = db.Query<ShopMemberConsume>()
                .Where(m => !m.IsDel && m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = (m.Amount / 100m),
                    BillType = 2,
                    BillDateTime = m.TransactionTime
                });
            var refundQuery = db.Query<ShopMemberRufund>()
                .Where(m => !m.IsDel && m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = (m.Amount / 100m),
                    BillType = 3,
                    BillDateTime = m.TransactionTime
                });
            var unionQuery = rechargeQuery.Union(consumeQuery).Union(refundQuery);


            //var rechargeQuery = db.Query<ShopMemberRecharge>()
            //    .Where(m => !m.IsDel && m.Status == ShopMemberTransactionStatus.已完成)
            //    .Where(m => m.ShopMemberId == shopMember.Id)
            //    .Select(m => new GetBillModel()
            //    {
            //        Amount = (m.Amount + m.PresentedAmount) / 100m,
            //        BillType = 1,
            //        BillDateTime = m.TransactionTime
            //    });
            //var consumeQuery = db.Query<ShopMemberConsume>()
            //    .Where(m => !m.IsDel && m.ShopMemberId == shopMember.Id)
            //    .Select(m => new GetBillModel()
            //    {
            //        Amount = m.Amount / 100m,
            //        BillType = 2,
            //        BillDateTime = m.TransactionTime
            //    });
            //var unionQuery = rechargeQuery.Union(consumeQuery);
            return unionQuery;
        }

        /// <summary>
        /// 获取最近消费金额
        /// </summary>
        /// <param name="shopMemberId"></param>
        /// <returns></returns>
        private string GetConsumeTime(int shopMemberId)
        {
            var result = "";
            var list = GetRecharge(new ShopMember() { Id = shopMemberId });
            if (list.Count() != 0)
            {
                return list.OrderByDescending(m => m.BillDateTime).FirstOrDefault().BillDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "无消费记录";
        }
        #endregion

        /// <summary>
        /// 导出会员数据
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        public ActionResult ExportMemberList(int shopId)
        {
            CheckShopActor(shopId, ShopActorType.超级管理员);

            var list = db.Query<ShopMember>()
                .Where(m => !m.IsDel && m.ShopId == shopId)
                .AsNoTracking();
            List<ShopMember> shopMembers = new List<ShopMember>();
            foreach (var listItem in list)
            {
                listItem.ShopMemberLevel = db.ShopMemberLevel.Find(listItem.ShopMemberLevelId);
                shopMembers.Add(listItem);
            }
            var exportMemberListModels = _mapper.Map<List<ExportMemberListModel>>(shopMembers);
            var tTable = EPPlusUtil.ToDataTable(exportMemberListModels);
            var testTable = EPPlusUtil.ExportByEPPlus(tTable, "会员列表");
            return File(testTable, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "会员列表.xlsx");
        }

        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
