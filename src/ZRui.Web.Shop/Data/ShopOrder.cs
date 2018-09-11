using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺的订单
    /// </summary>
    public class ShopOrder : EntityBase
    {
        /// <summary>
        /// 标识，订单编号
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联的商铺的位置（桌号或者房号）
        /// </summary>
        [ForeignKey("ShopPartId")]
        public ShopPart ShopPart { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int? ShopPartId { get; set; }
        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 实际支付金额
        /// </summary>
        public int? Payment { get; set; }
        /// <summary>
        /// 是否外卖
        /// </summary>
        public bool IsTakeOut { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否打印
        /// </summary>
        public bool IsPrint { get; set; }
        /// <summary>
        /// 是否推送
        /// </summary>
        public bool IsSend { get; set; }
        /// <summary>
        /// 自助点餐相关信息
        /// </summary>
        [ForeignKey("ShopOrderSelfHelpId")]
        public ShopOrderSelfHelp ShopOrderSelfHelp { get; set; }
        public int? ShopOrderSelfHelpId { get; set; }
        /// <summary>
        /// 满减规则id
        /// </summary>
        public int? MoneyOffRuleId { get; set; }
        [ForeignKey("MoneyOffRuleId")]
        public ShopOrderMoneyOffRule ShopOrderMoneyOffRule { get; set; }
        /// <summary>
        /// 外卖地址Id   已弃用
        /// </summary>
        public int? memberAddressId { get; set; }
        [ForeignKey("memberAddressId")]
        public MemberAddress memberAddress { get; set; }
        /// <summary>
        /// 其它费用id
        /// </summary>
        public int? OtherFeeId { get; set; }
        [ForeignKey("OtherFeeId")]
        public ShopOrderOtherFee ShopOrderOtherFee { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 会员折扣
        /// </summary>
        public int MemberDiscount { get; set; }
        /// <summary>
        /// 余额消费记录
        /// </summary>
        [ForeignKey("ShopMemberConsumeId")]
        public ShopMemberConsume ShopMemberConsume { get; set; }
        public int? ShopMemberConsumeId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 配送方式 商家配送 = 0,达达配送 = 1
        /// </summary>
        public TakeDistributionType takeDistributionType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }

    }


    /// <summary>
    /// 商铺订单类型
    /// </summary>
    public enum ShopOrderStatus
    {

        待成团 = 10,
        待自提 = 20,
        待配送 = 30,
        已完成 = 100,
        已取消 = -1,
        已确认 = 50,
        已退款 = -2,
        未处理 = 0,

        已支付 = 60,
        待支付 = 70,

        //发票相关
        已打印 = 61,
        加急申请中 = 62,
        申请发票 = 63,
        发票待领取 = 64,
        已领取 = 65,

        //外卖相关
        配送中 = 66,
        待取消 = 67,
        退款审批 = 68,
        退款中 = 69,
        自提中 = 71,
        //外卖达达配送
        待接单 = 1,
        待取货 = 2,
        已过期 = 7,
        指派单 = 8,
        妥投异常之物品返回中 = 9,
        妥投异常之物品返回完成 = 11,
        系统故障订单发布失败 = 1000

    }


    /// <summary>
    /// 订单支付状态,在前端中使用
    /// </summary>
    public enum ShopOrderPayStatus
    {
        未支付 = 1,
        已支付 = 2,
        支付过 = 3
    }
}
