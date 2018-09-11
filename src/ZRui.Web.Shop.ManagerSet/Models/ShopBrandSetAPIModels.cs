using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandSetAPIModels
{
    public class GetShopBrandsArgsModel
    {
    }

    public class GetShopBrandItem
    {
        public int ShopBrandId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 一些说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }
    }

    public class GetSingleModel : ShopBrand
    {

    }
}