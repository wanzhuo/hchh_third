namespace ZRui.Web.ShopCommodityAPIModels
{
    /// <summary>
    /// ��ȡ�б������
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