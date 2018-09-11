using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.WechatPay.PayAPIModels
{
    public class BeginRechangeArgsModel
    {
        //public int TotalFee { get; set; }
        public string ShopFlag { get; set; }
        public int? ShopOrderId { get; set; }
        public int? ConglomerationOrderId { get; set; }
    }

    public class GetRechangeResultArgsModel
    {
        public string TradeNo { get; set; }
        public string ShopFlag { get; set; }
    }

    public class RefundArgsModel
    {
        public int OrderId { get; set; }
        public string ShopFlag { get; set; }
        public string TradeNo { get; set; }

        public OrderType OrderType { get; set; }
    }

    public class ThirdModel
    {
        public int ShopId { get; set; }
        public int OrderId { get; set; }
    }

    public class PayInfo
    {
        public string timeStamp { get; set; }
        public string nonceStr { get; set; }
        public string package { get; set; }
        public string signType { get; set; }
        public string paySign { get; set; }

    }

    public class PayInfoModel
    {
        public PayInfo payInfo { get; set; }
    }


    public class BalanceConsumeArgsModel
    {
        public int OrderId { get; set; }
        public string Password { get; set; }
    }

    public class OfflinePayModel
    {
        /// <summary>
        /// 交易金额
        /// </summary>
        public int TotalFee { get; set; }

        /// <summary>
        /// 交易终端IP
        /// </summary>
        public string MchCreateIp { get; set; }
        /// <summary>
        /// 客户凭证
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 操作员Id
        /// </summary>
        public int OpUserId { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string OpShopId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string OpDeviceId { get; set; }
    }
}
