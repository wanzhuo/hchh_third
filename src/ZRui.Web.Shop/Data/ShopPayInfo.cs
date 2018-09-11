using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺支付方式信息
    /// </summary>
    public class ShopPayInfo : EntityBase
    {
        /// <summary>
        /// ShopId
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// ShopFlag
        /// </summary>
        public string ShopFlag { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PayWay PayWay { get; set; }
        /// <summary>
        /// 小程序Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 签名用的密钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

    }
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayWay
    {
        Wechat = 0,
        Swiftpass = 1
    }


}
