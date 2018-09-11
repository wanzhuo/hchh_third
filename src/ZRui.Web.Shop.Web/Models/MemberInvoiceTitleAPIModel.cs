using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class MemberInvoiceTitleAPIModel
    {
    }

    /// <summary>
    /// 添加更新发票抬头
    /// </summary>
    public class InvoiceTitleModel
    {

        public int Id { get; set; }
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 名称 （抬头名称）
        /// </summary>
        public string MemberInvoiceTitleName { get; set; }

        /// <summary>
        /// 税号（纳税人识别号）
        /// </summary>
        public string BuyerNumber { get; set; }

        /// <summary>
        /// 单位地址（选填）
        /// </summary>

        public string EnterpriseAddress { get; set; }

        /// <summary>
        /// 电话号码（选填）
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        ///开户银行（选填）
        /// </summary>
        public string BankDeposit { get; set; }

        /// <summary>
        /// 银行账户（选填）
        /// </summary>
        public string BankAccount { get; set; }

        public bool IsDel { get; set; }
        public InvoiceType Type { get; set; }

    }


    /// <summary>
    /// 我要发票请求
    /// </summary>
    public class InvoiceRequestModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public int ShopOrderId { get; set; }

        /// <summary>
        /// 发票抬头ID
        /// </summary>
        public int MemberInvoiceTitleId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }


        public bool IsDel { get; set; }
        public ProcessState State { get; set; }
    }

    public class GetPagedListGetInvoiceTitleModelModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public IList<InvoiceTitleModel> Items { get; set; }
    }

}
