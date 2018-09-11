using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 第三方配送商户
    /// </summary>
    public class Merchant
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 达达商户ID
        /// </summary>
        public int DaDaShopId { get; set; } 
        /// <summary>
        /// 注册商户手机号,用于登陆商户后台
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 商户城市名称(如,上海)
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 企业全称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 企业地址
        /// </summary>
        public string EnterpriseAddress { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }


    }

}
