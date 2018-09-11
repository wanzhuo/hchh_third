using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.Third
{
    /// <summary>
    /// 第三方配送配置
    /// </summary>
    public class ThirdConfig
    {

        public string AppSecret { get; set; }
        public string Appkey { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }
        /// <summary>
        /// 生成充值链接地址
        /// </summary>
        public string RechargeUrl { get; set; }
    }
}
