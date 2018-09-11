using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
  public  class MemberInvoiceRequest : EntityBase
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int ShopOrderId{ get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("ShopOrderId")]
        public ShopOrder ShopOrder { get; set; }

        /// <summary>
        /// 发票抬头ID
        /// </summary>
        public int MemberInvoiceTitleId { get; set; }
        /// <summary>
        /// 关联的发票抬头
        /// </summary>
        [ForeignKey("MemberInvoiceTitleId")]
        public MemberInvoiceTitle MemberInvoiceTitle { get; set; }
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


    }
    /// <summary>
    /// 请求状态
    /// </summary>
    public enum ProcessState
    {
        待处理发票 = 1,
        已开发票 = 2  
    }
}
