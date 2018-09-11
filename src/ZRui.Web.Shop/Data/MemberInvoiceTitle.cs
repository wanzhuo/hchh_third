using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{

    /// <summary>
    /// 用户发票抬头
    /// </summary>
    public class MemberInvoiceTitle : EntityBase
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }

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

    /// <summary>
    /// 发票类型
    /// </summary>
    public enum InvoiceType
    {
        增值税普通发票 = 1,
        增值税专用发票 = 2  //暂不使用
    }
}
