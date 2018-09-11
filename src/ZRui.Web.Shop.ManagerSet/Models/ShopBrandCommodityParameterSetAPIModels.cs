using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommodityParameterSetAPIModels
{
    /// <summary>
    /// 获取列表参数
    /// </summary>
    public class GetListArgsModel
    {
        /// <summary>
        /// 商铺品牌Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// 获取列表返回值
    /// </summary>
    public class GetListModel
    {
        /// <summary>
        /// 列表
        /// </summary>
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// 获取分页列表参数
    /// </summary>
    public class GetPagedListArgsModel : GetListArgsModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 排序的字段名字
        /// </summary>
        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        /// <summary>
        /// 升序或者降序
        /// </summary>
        [JsonProperty("orderType")]
        public string OrderType { get; set; }
    }
    /// <summary>
    /// 获取分页列表的返回值
    /// </summary>
    public class GetPagedListModel : GetListModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 总纪录数
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopBrandCommodityParameter
    {

    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel 
    {
        /// <summary>
        /// 关联的商铺品牌Id
        /// </summary>
        public int? ShopBrandId { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodityParameter
    {

    }
}