using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.ShopOrderMoneyOffSetAPIModel
{
    public class AddArgs
    {
        public List<RuleModel> Items { get; set; }
        public int ShopId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MoneyOffType MoneyOffType { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 是否扫码点餐
        /// </summary>
        public bool IsScanCode { get; set; }
        /// <summary>
        /// 是否外卖
        /// </summary>
        public bool IsTakeout { get; set; }
        /// <summary>
        /// 是否自助点餐
        /// </summary>
        public bool IsSelfOrder { get; set; }
        /// <summary>
        /// 是否收银系统
        /// </summary>
        public bool IsCashier { get; set; }

    }

    public class GetListArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetRuleListArgsModel
    {
        public int? id { get; set; }
    }

    public class SetMoneyOffIsEnableArgsModel
    {
        public int Id { get; set; }
        public bool IsEnable { get; set; }
    }

    public class GetListModel
    {
        public List<ShopOrderMoneyOff> Items { get; set; }
    }

    public class GetRuleListModel
    {
        public List<RuleModel> Items { get; set; }
    }

    public class RuleModel
    {
        public decimal FullAmount { get; set; }
        public decimal Discount { get; set; }
    }
}
