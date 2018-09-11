namespace ZRui.Web.ShopOrderMoneyOffAPIModels
{
    public class GetRuleArgsModel
    {
        public string shopFlag { get; set; }
        public int? Amount { get; set; }
        public MoneyOffType? moneyOffType { get; set; }
    }

    public class GetShopRuleArgsModel
    {
        public string shopFlag { get; set; }
        public MoneyOffType? moneyOffType { get; set; }
    }

    public class GetShopRule
    {
        public decimal FullAmount { get; set; }
        public decimal Discount { get; set; }
    }
}
