using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.Logging;
using ZRui.Web.ShopMemberAPIModels;
using System.Linq;
using ZRui.Web.BLL.Servers;
using System.Collections.Generic;
using ZRui.Web.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.BLL.Attribute;
using Microsoft.Extensions.Caching.Memory;
using ZRui.Web.BLL;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopMemberAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        ILogger _logger;
        private IMemoryCache _memoryCache;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment hostingEnvironment;
        public ShopMemberAPIController(
            IOptions<MemberAPIOptions> options,
            IMemoryCache memoryCache
            , ShopDbContext db
            , ILoggerFactory loggerFactory
             , IMapper mapper
            , WechatCoreDbContext wechatCoreDb
            , MemberDbContext memberDb
            , IHostingEnvironment hostingEnvironment)
            : base(options, memberDb, wechatCoreDb)
        {
            this.db = db;
            _mapper = mapper;
            _memoryCache = memoryCache;
            this.hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ShopOrderAPIController>();
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetSingle([FromBody]GetSingleArgsModel args)
        {
            int memberId = GetMemberId();
            var shopMember = db.Query<ShopMember>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == args.ShopId && m.MemberId == memberId)
                .FirstOrDefault();
            if (shopMember == null) return Error("当前没有会员信息");
            shopMember.ShopMemberLevel = db.ShopMemberLevel.Find(shopMember.ShopMemberLevelId);
            var getSingleModel = _mapper.Map<GetSingleModel>(shopMember);

            return Success(getSingleModel);
        }


        /// <summary>
        /// 会员注册
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelCheckingFilter]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult Register([FromBody]RegisterArgsModel args)
        {

            if (!CheckVerificationCode(args.Phone, args.Code))
            {
                return Error("验证码错误");
            }
            int memberId = GetMemberId();
            var model = db.Query<ShopMember>()
                .FirstOrDefault(m => !m.IsDel && m.MemberId == memberId && m.ShopId == args.ShopId.Value);
            if (model != null) throw new Exception("该会员已注册过");
            var members = memberDb.Members.Find(memberId);
            var shopMember = new ShopMember()
            {
                ShopId = args.ShopId.Value,
                AddIp = GetIp(),
                AddTime = DateTime.Now,
                MemberId = memberId,
                Sex = args.Sex,
                Phone = args.Phone,
                Credits = 0,
                Balance = 0,
                BirthDay = args.BirthDay.Value,
                Name = members.NickName
            };
            db.Add(shopMember);
            var shopMemberServer = new ShopMemberServer(db, shopMember);
            if (!shopMemberServer.CheckPhoneNumCanUse(args.Phone,args.ShopId.Value))
                return Error("该手机号码已注册");
            //shopMemberServer.SetPassword(args.Password);
            db.SaveChanges();
            //更新会员等级
            ShopMemberLevelServer.UpdateMemberLevel(db, db.ShopMemberSet.FirstOrDefault(m => m.ShopId.Equals(args.ShopId) && !m.IsDel), memberId, args.ShopId.Value, _logger);

            return Success(new
            {
                shopMember.Id
            });
        }

        /// <summary>
        /// 获取会员折扣
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetMemberDiscount([FromBody]GetMemberDiscountArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopId不能为空");
            int memberId = GetMemberId();
            ShopMemberServer memberServer;
            try
            {
                memberServer = new ShopMemberServer(db, args.ShopId.Value, memberId);
            }
            catch (Exception)
            {
                return Success(new GetMemberDiscountModel()
                {
                    Balance = 0,
                    Discount = 0
                });
            }
            var dic = new Dictionary<int, IQueryable<ShopCommodityStock>>();
            foreach (var item in args.Items)
            {
                var stock = db.Query<ShopCommodityStock>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == args.ShopId.Value)
                    .Where(m => m.Sku.Flag == item.SkuFlag);
                dic.Add(item.Count, stock);
            }
            int rtn = memberServer.ComputeMemberDiscount(dic);
            return Success(new GetMemberDiscountModel()
            {
                Discount = rtn / 100d,
                Balance = memberServer.GetBalance() / 100d
            });
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetBill([FromBody]GetBillArgsModel args)
        {
            if (!args.ShopId.HasValue) throw new Exception("ShopId不能为空");
            int memberId = GetMemberId();
            ShopMember shopMember = db.Query<ShopMember>()
                .Where(m => !m.IsDel && m.MemberId == memberId && m.ShopId == args.ShopId.Value)
                .FirstOrDefault();

            var rechargeQuery = db.Query<ShopMemberRecharge>()
                .Where(m => !m.IsDel && m.Status == ShopMemberTransactionStatus.已完成)
                .Where(m => m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = ((m.Amount + m.PresentedAmount) / 100m).ToString("#0.00"),
                    BillType = 1,
                    BillDateTime = m.TransactionTime
                });
            var consumeQuery = db.Query<ShopMemberConsume>()
                .Where(m => !m.IsDel && m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = (m.Amount / 100m).ToString("#0.00"),
                    BillType = 2,
                    BillDateTime = m.TransactionTime
                });
            var refundQuery = db.Query<ShopMemberRufund>()
                .Where(m => !m.IsDel && m.ShopMemberId == shopMember.Id)
                .Select(m => new GetBillModel()
                {
                    Amount = (m.Amount / 100m).ToString("#0.00"),
                    BillType = 3,
                    BillDateTime = m.TransactionTime
                });
            var unionQuery = rechargeQuery.Union(consumeQuery).Union(refundQuery);
            var rtn = unionQuery.OrderByDescending(m => m.BillDateTime)
                .ToPagedList(args.PageIndex, args.PageSize);
            decimal balance = shopMember == null ? 0 : shopMember.Balance / 100m;
            return Success(new GetBillReturnModel()
            {
                Balance = balance,
                Bill = rtn
            });
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetMemberBalance([FromBody]GetSingleArgsModel args)
        {
            int memberId = GetMemberId();
            var shopMember = ShopMemberServer.GetShopMember(db, args.ShopId, memberId);
            if (shopMember == null)
                return Success(0);
            else
                return Success(shopMember.Balance);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult ChangePassword([FromBody]ChangePasswordArgsModel args)
        {
            if (!args.shopId.HasValue) throw new Exception("ShopId不能为空");
            var server = new ShopMemberServer(db, args.shopId.Value, GetMemberId());
            server.ChangePassword(args.OldPWD, args.NewPWD);
            db.SaveChanges();
            return Success();
        }

        //[HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        //public APIResult MemberTransaction([FromBody]GetSingleArgsModel args)
        //{

        //}

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public APIResult SendVerificationCode([FromBody]RegisterArgsModel args)
        {


            var shopMembers = db.ShopMembers.FirstOrDefault(m => m.Phone.Equals(args.Phone) && !m.IsDel);
            if (shopMembers != null)
            {
                return Error("该号码已经绑定过了");
            }

            string cacheKey = args.Phone;
            string code;

            if (!_memoryCache.TryGetValue(cacheKey, out code))
            {
                code = new Random().Next(1000, 9999).ToString();
                //设置绝对过期时间2分钟
                _memoryCache.Set(cacheKey, code, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            //发送验证码
            return SMSHelper.Send(args.Phone, $"验证码为{code},五分钟后过期");
        }


        /// <summary>
        /// 校对验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public bool CheckVerificationCode(string phone, string code)
        {
            string cacheKey = phone;
            string serverCode;

            if (!_memoryCache.TryGetValue(cacheKey, out serverCode))
            {
                return false;
            }
            if (code.Equals(serverCode))
            {
                _memoryCache.Remove(cacheKey);
                return true;

            }
            return false;
        }


    }
}
