using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.CommunityAPIModels;

namespace ZRui.Web.Models
{
    public class ShopInvoiceAPIModel
    {

    }

    /// <summary>
    /// 修改发票请求状态
    /// </summary>
    public class MakeOutInvoiceModel
    {
        public int Id { get; set; }


    }


    /// <summary>
    /// 分页获取发票索取请求记录
    /// </summary>
    public class GetPagedListInvoiceRequsetModel
    {
        int _PageIndex;
        public int PageIndex
        {
            get
            {
                if (_PageIndex == 0)
                {
                    return 1;
                }
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
            }
        }
        int _PageSize;
        public int PageSize
        {
            get
            {
                if (_PageSize <= 0)
                {
                    return 1;
                }
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }

        /// <summary>
        /// 商铺Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 商铺Id
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }

    }


    /// <summary>
    /// 请求列表返回实体
    /// </summary>
    public class ResultGetPageListModel
    {

        public int Id { get; set; }
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int ShopOrderId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 请求状态
        /// </summary>
        public ProcessState State { get; set; }


        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        public ShopOrderInfo ShopOrder { get; set; }

        /// <summary>
        /// 发票抬头信息
        /// </summary>
        public MemberInvoiceTitleInfo MemberInvoiceTitle { get; set; }

    }


    /// <summary>
    /// 返回订单基础信息
    /// </summary>
    public class ShopOrderInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 标识，订单编号
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        decimal _Amount;
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get { return _Amount / 100.00M; } set { _Amount = value; } }

        decimal? _Payment;
        /// <summary>
        /// 实际支付金额
        /// </summary>
        public decimal? Payment { get { return _Payment / 100.00M; } set { _Payment = value; } }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }
    }

    /// <summary>
    /// 返回发票抬头信息
    /// </summary>
    public class MemberInvoiceTitleInfo
    {

        public int id { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType Type { get; set; }

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
    }


    public class GetPagedListModel : GetListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<ResultGetPageListModel> Items { get; set; }
    }

}
