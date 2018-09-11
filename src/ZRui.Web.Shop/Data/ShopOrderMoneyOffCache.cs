using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺折扣
    /// </summary>
    public class ShopOrderMoneyOffCache : EntityBase
    {
        /// <summary>
        /// 关联的满减
        /// </summary>
        [ForeignKey("MoneyOffId")]
        public ShopOrderMoneyOff ShopOrderMoneyOff { get; set; }
        /// <summary>
        /// 关联的满减Id
        /// </summary>
        public int MoneyOffId { get; set; }
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }

    }

}
