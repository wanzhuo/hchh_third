using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBrandCommoditySetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int? ShopBrandId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }

    public class GetPagedListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopBrandCommodity
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 关联的类别Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 关联的商铺品牌Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 月销量
        /// </summary>
        public int SalesForMonth { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int Upvote { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool IsRecommand { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodity
    {

    }

    public class GetShopBrandsModel
    {
        public List<GetShopBrandsItem> Items { get; set; }
    }

    public class GetShopBrandsItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}