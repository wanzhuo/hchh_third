using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopCommodityStockSetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int ShopId { get; set; }
        public int CommodityId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopCommodityStock
    {

    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 商铺Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// SkuId
        /// </summary>
        public int SkuId { get; set; }
    }

    public class UpdateArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        // <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 成本价，单位是分
        /// </summary>
        public int CostPrice { get; set; }
        /// <summary>
        /// 销售价，单位是分
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 市场价，单位是分
        /// </summary>
        public int MarketPrice { get; set; }
    }

    public class GetSkuItemsArgsModel : CommunityArgsModel
    {
        public int CommodityId { get; set; }
    }

    public class GetSkuItemsModel
    {
        public IList<SkuItem> Items { get; set; }
        public IList<Sku> Skus
        {
            get
            {
                return Items.GroupBy(m => m.SkuId).Select(g => new Sku
                {
                    Id = g.Key,
                    Values = g.OrderBy(m => m.Id).Select(m => m.Value).ToList()
                }).ToList();
            }
        }
    }

    public class Sku
    {
        public int Id { get; set; }
        public IList<string> Values { get; set; }
    }

    public class SkuItem
    {
        public int SkuId { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class GetShopBrandsModel
    {
        public List<ShopBrandsItem> Items { get; set; }
    }
    public class ShopBrandsItem
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

    public class GetShopsArgsModel : CommunityArgsModel
    {
        public int ShopBrandId { get; set; }
    }
    public class GetShopsModel
    {
        public List<ShopItem> Items { get; set; }
    }
    public class ShopItem
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