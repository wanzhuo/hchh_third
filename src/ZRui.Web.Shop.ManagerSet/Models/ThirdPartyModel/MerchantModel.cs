using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models.ThirdPartyModel
{
    /// <summary>
    /// 注册商户
    /// </summary>
    public class MerchantModel
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 注册商户手机号,用于登陆商户后台
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 商户城市名称(如,上海)
        /// </summary>
        public string city_name { get; set; }
        /// <summary>
        /// 企业全称
        /// </summary>
        public string enterprise_name { get; set; }
        /// <summary>
        /// 企业地址
        /// </summary>
        public string enterprise_address { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contact_name { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string contact_phone { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string email { get; set; }


    }
}
