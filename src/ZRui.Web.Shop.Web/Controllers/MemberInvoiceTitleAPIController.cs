using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Models;
using AutoMapper;
using ZRui.Web.Utils;
using ZRui.Web.Common;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{

    /// <summary>
    /// 发票抬头管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class MemberInvoiceTitleAPIController : WechatApiControllerBase
    {
        ShopDbContext db;
        private IMapper _mapper { get; set; }
        readonly IHostingEnvironment hostingEnvironment;
        public MemberInvoiceTitleAPIController(ICommunityService communityService
            , IOptions<MemberAPIOptions> memberOptions
            , ShopDbContext db
            , MemberDbContext memberDb
            , WechatCoreDbContext wechatCoreDb
            , IMapper mapper
            , IHostingEnvironment hostingEnvironment)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.db = db;
            _mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// 添加发票抬头
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult AddInvoiceTitle([FromBody]InvoiceTitleModel input)
        {
            input.MemberId = GetMemberId();
            //input.MemberId = 87;
            if (!(input.BuyerNumber.Length >= 15 && input.BuyerNumber.Length <= 20))
            {
                return Error("税号格式错误,税号由15位、18或者20位码（字符型）组成");
            }

            if (input.Tel != null && !string.IsNullOrWhiteSpace(input.Tel))
            {
                if (!ValidateUtil.IsValidPhoneAndMobile(input.Tel))
                {
                    return Error("电话号码格式错误");
                }
            }
            //if (input.BankAccount != null && !string.IsNullOrWhiteSpace(input.BankAccount))
            //{
            //    if (!ValidateUtil.IsValidAccountNumber(input.BankAccount))
            //    {
            //        return Error("银行卡号格式错误");
            //    }
            //}

            var memberInvoiceTitle = _mapper.Map<MemberInvoiceTitle>(input);
            memberInvoiceTitle.Type = InvoiceType.增值税普通发票;
            db.MemberInvoiceTitle.Add(memberInvoiceTitle);
            db.SaveChanges();

            return Success(memberInvoiceTitle);

        }

        /// <summary>
        /// 添加发票抬头(测试)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult AddInvoiceTitleTest([FromBody]InvoiceTitleModel input)
        {
            //input.MemberId = GetMemberId();
            input.MemberId = 87;
            if (!(input.BuyerNumber.Length >= 15 && input.BuyerNumber.Length <= 20))
            {
                return Error("税号格式错误,税号由15位、18或者20位码（字符型）组成");
            }

            if (input.Tel != null && !string.IsNullOrWhiteSpace(input.Tel))
            {
                if (!ValidateUtil.IsValidPhoneAndMobile(input.Tel))
                {
                    return Error("电话号码格式错误");
                }
            }
            //if (input.BankAccount != null && !string.IsNullOrWhiteSpace(input.BankAccount))
            //{
            //    if (!ValidateUtil.IsValidAccountNumber(input.BankAccount))
            //    {
            //        return Error("银行卡号格式错误");
            //    }
            //}

            var memberInvoiceTitle = _mapper.Map<MemberInvoiceTitle>(input);
            memberInvoiceTitle.Type = InvoiceType.增值税普通发票;
            db.MemberInvoiceTitle.Add(memberInvoiceTitle);
            db.SaveChanges();

            return Success(memberInvoiceTitle);

        }


        /// <summary>
        /// 获取发票抬头
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetInvoiceTitleById([FromBody]InvoiceTitleModel input)
        {
            //var memberId = GetMemberId();
            //var memberId = 87;

            var titleData = db.MemberInvoiceTitle.Find(input.Id);
            if (titleData == null || titleData.IsDel )
            {
                return Error("记录不存在");
            }

            return Success(_mapper.Map<InvoiceTitleModel>(titleData));
        }

        /// <summary>
        /// 编辑发票抬头
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult EdityInvoiceTitle([FromBody]InvoiceTitleModel input)
        {
            var memberId = GetMemberId();
            var titleData = db.MemberInvoiceTitle.Find(input.Id);
            if (titleData == null || titleData.IsDel || titleData.MemberId != memberId)
            {
                return Error("记录不存在");
            }
            titleData.MemberInvoiceTitleName = input.MemberInvoiceTitleName;
            titleData.Tel = input.Tel;
            titleData.BuyerNumber = input.BuyerNumber;
            titleData.EnterpriseAddress = input.EnterpriseAddress;
            titleData.BankDeposit = input.BankDeposit;
            titleData.BankAccount = input.BankAccount;
            db.SaveChanges();
            return Success(_mapper.Map<InvoiceTitleModel>(titleData));
        }


        //删除发票抬头
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult DelInvoiceTitle([FromBody]InvoiceTitleModel input)
        {
            var memberId = GetMemberId();
            var titleData = db.MemberInvoiceTitle.Find(input.Id);
            if (titleData == null || titleData.IsDel || titleData.MemberId != memberId)
            {
                return Error("记录不存在");
            }
            titleData.IsDel = true;
            db.SaveChanges();
            return Success("删除成功");
        }

        /// <summary>
        /// 获取发票抬头列表 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetPagedList([FromBody]GetPagedListInvoiceTitleModel input)
        {
            var memberId = GetMemberId();
            var query = db.MemberInvoiceTitle.Where(
                m => !m.IsDel &&
                m.MemberId.Equals(memberId)
            )
                .Select(m => new InvoiceTitleModel()
                {
                    BankAccount = m.BankAccount,
                    BankDeposit = m.BankDeposit,
                    BuyerNumber = m.BuyerNumber,
                    EnterpriseAddress = m.EnterpriseAddress,
                    Id = m.Id,
                    MemberId = m.MemberId,
                    MemberInvoiceTitleName = m.MemberInvoiceTitleName,
                    Tel = m.Tel
                })
                .ToPagedList(input.PageIndex, input.PageSize);

            return Success(new GetPagedListGetInvoiceTitleModelModel()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalCount = query.TotalItemCount,
                Items = query.ToList()
            });
        }

        /// <summary>
        /// 我要发票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult InvoiceRequest([FromBody]InvoiceRequestModel input)
        {
            var memberId = GetMemberId();
            //var memberId = 87;
            var isOK = db.MemberInvoiceRequest.Count(m =>
             m.MemberId.Equals(memberId) &&
             m.ShopOrderId.Equals(input.ShopOrderId) &&
             !m.IsDel
              ) > 0;
            if (isOK)
            {
                return Error("已经请求过发票了");
            }

            MemberInvoiceRequest invoiceRequest = new MemberInvoiceRequest();
            invoiceRequest.ShopOrderId = input.ShopOrderId;
            invoiceRequest.MemberInvoiceTitleId = input.MemberInvoiceTitleId;
            invoiceRequest.ShopId = input.ShopId;
            //var invoiceRequest = _mapper.Map<MemberInvoiceRequest>(input);
            invoiceRequest.MemberId = memberId;
            invoiceRequest.CreateTime = DateTime.Now;
            invoiceRequest.State = ProcessState.待处理发票;
            db.MemberInvoiceRequest.Add(invoiceRequest);
            db.SaveChanges();
            return Success(invoiceRequest);
        }



        /// <summary>
        /// 我要发票(测试)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "jwt")]
        public APIResult InvoiceRequestTest([FromBody]InvoiceRequestModel input)
        {
            //var memberId = GetMemberId();
            var memberId = 87;
            var isOK = db.MemberInvoiceRequest.Count(m =>
             m.MemberId.Equals(memberId) &&
             m.ShopOrderId.Equals(input.ShopOrderId) &&
             !m.IsDel
              ) > 0;
            if (isOK)
            {
                return Error("已经请求过发票了");
            }
            var invoiceRequest = _mapper.Map<MemberInvoiceRequest>(input);
            invoiceRequest.MemberId = memberId;
            invoiceRequest.CreateTime = DateTime.Now;
            invoiceRequest.State = ProcessState.待处理发票;
            db.MemberInvoiceRequest.Add(invoiceRequest);
            db.SaveChanges();
            return Success(invoiceRequest);
        }
    }
}
