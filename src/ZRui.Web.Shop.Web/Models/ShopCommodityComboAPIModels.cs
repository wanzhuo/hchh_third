using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZRui.Web.ShopCommodityAPIModels;

namespace ZRui.Web.ShopCommodityComboAPIModels
{


    public class AddComboArgsModel
    {
        public int? comboId { get; set; }
    }

    public class GetComboListArgsModel
    {
        public int? ShopId { get; set; }
        public DiningWay? DiningWay { get; set; }
    }



    public class GetComboDetailArgsModel
    {
        public int? CommodityId { get; set; }
    }

    public class ComboRow
    {
        public int Id { get; set; }
        public string Cover { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get;  set; }
    }

    public class GetCommoditysModel
    {
        public IList<CommodityRowItem> Items { get; set; }
    }

    public class StockRowItem
    {
        public int SkuId { get;  set; }
        public string SkuFlag { get;  set; }
        public int CommodityId { get;  set; }
        public string Cover { get; set; }
        public string Name { get; set; }
        public int SalePrice { get;  set; }
        public int MarketPrice { get;  set; }
        public int Stock { get;  set; }
    }

    public class Combo
    {
        public int id { get; set; }
        public int ShopId { get; set; }
        public string ComboName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal MarketPrice { get; set; }
        public List<ComboItem> Items { get; set; }
    }

    public class ComboItem
    {
        public string ComboItemName { get; set; }
        public string CommodityIds { get; set; }
        public List<CommodityRowItem> Commodities { get; set; }
    }


    public class CommodityRowItem
    {
        public string Name { get; set; }
        public int SalesForMonth { get; set; }
        public int Upvote { get; set; }
        public string Cover { get; set; }
        public string Detail { get; set; }
        public IList<CommoditySku> Skus { get; set; }
        public IList<CommodityParameter> Parameters { get; set; }
        public int Id { get;  set; }
    }



    public class SkuItem
    {
        public int SkuId { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
        public int ParameterId { get;  set; }
        public string ParameterName { get;  set; }
        public int ParameterValueId { get;  set; }
    }

    public class CommoditySku
    {
        public int SkuId { get; set; }
        public string SkuFlag { get; set; }
        public int Stock { get; set; }
        public decimal SalePrice { get; set; }
        public int MarketPrice { get; set; }
        public IList<SkuItem> Items { get; set; }

        public CommoditySku(int skuId, IList<SkuItem> items, StockRowItem stock)
        {
            this.SkuId = skuId;
            this.Items = items;
            if (stock != null)
            {
                this.SkuFlag = stock.SkuFlag;
                this.Stock = stock.Stock;
                this.SalePrice = stock.SalePrice;
                this.MarketPrice = stock.MarketPrice;
            }
        }
    }

    public class CommodityParameter
    {
        public CommodityParameter(IList<SkuItem> skuItems)
        {
            Id = skuItems.First().Id;
            Name = skuItems.First().ParameterName;
            Values = skuItems.GroupBy(m => m.ParameterValueId).Select(m => new CommodityParameterValues()
            {
                Id = m.First().ParameterValueId,
                Value = m.First().Value
            }).ToList();
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<CommodityParameterValues> Values { get; set; }
    }

    public class CommodityParameterValues
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

}