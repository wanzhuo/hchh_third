using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 中信商户密钥
    /// </summary>
    public class SwiftpassKey
    {
        public int Id { get; set; }
        /// <summary>
        /// 商户标识
        /// </summary>
        public string ShopFlag { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrviateKey { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string ReqUrl { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string Notify { get; set; }

        public bool IsEnable { get; set; }

    }
}
