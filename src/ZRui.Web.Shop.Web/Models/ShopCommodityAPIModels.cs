namespace ZRui.Web.ShopCommodityAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel
    {
        public string ShopFlag { get; set; }
        public string CategoryFlag { get; set; }
        public DiningWay? DiningWay { get; set; }
    }


    public class GetCategoryTreeArgsModel
    {
        public string ShopFlag { get; set; }
    }


    public class GetShopCategoryAndCommoditiesArgsModel
    {
        public string ShopFlag { get; set; }
        public DiningWay DiningWay { get; set; }
    }



    public class GetSkuItemsArgsModel
    {
        public string CommodityFlag { get; set; }
    }



}