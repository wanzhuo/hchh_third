namespace ZRui.Web
{
    /// <summary>
    /// 商铺折扣规则
    /// </summary>
    public class ShopOrderMoneyOffRule : EntityBase
    {
        public int MoneyOffId { get; set; }
        public int FullAmount { get; set; }
        public int Discount { get; set; }
    }

}
