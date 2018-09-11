using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.ShopManager.ShopCommodityComboSetAPIModels
{
    public class ComboModel: ShopBrandCombo
    {
        public int? ShopId { get; set; }
        public List<ComboGroup> Groups { get; set; }
    }


    public class ComboGroup
    {
        public string Name { get; set; }
        public List<ComboItem> Items { get; set; }
    }


    public class ComboItem
    {
        public string CommodityName { get; set; }
        public int Count { get; set; }
        public string Sku { get; set; }
        public decimal SalePrice { get; set; }
    }






    public class GetComboArgsModels
    {
        public int? shopId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetComboPagedListModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IList<ComboModel> Items { get; set; }
    }

    public class ComboCommodityItem
    {
        public int key { get; set; }
        public string label { get; set; }
    }
    
    public class AddComboCategoryArgsModels
    {
        public int ShopBrandId { get; set; }
        public string Name { get; set; }
    }

    public class GetComboCategoryArgsModels
    {
        public int ShopBrandId { get; set; }
    }

    public class GetAllCommodityArgsModels
    {
        public int ShopBrandId { get; set; }
    }
}
