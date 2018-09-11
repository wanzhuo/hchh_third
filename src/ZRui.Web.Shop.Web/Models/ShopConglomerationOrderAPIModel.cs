using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class ShopConglomerationOrderAPIModel
    {

    }



    /// <summary>
    /// 获取订单支付状态信息
    /// </summary>
    public class GetGetOrderStatusModel
    {
        public int ShopId { get; set; }
        public int OrderId { get; set; }
    }

    /// <summary>
    /// 添加拼团订单业务实体
    /// </summary>
    public class ConglomerationOrderModel : CreateModelBase
    {

        /// <summary>
        /// 	订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 快递订单号
        /// </summary>
        public string ExpressSingle { get; set; }
        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }


        /// <summary>
        /// 已发起的拼团FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }

        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }


        /// <summary>
        /// 快递发出时间
        /// </summary>
        public DateTime DateDelivery { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public int? Payment { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 配送地址Fk_Id
        /// </summary>
        public int? MemberAddressId { get; set; }

        /// <summary>
        /// 关联的商铺fK_Id
        /// </summary>
        public int ShopId { get; set; }
    }



    /// <summary>
    /// 获取订单列表请求实体
    /// </summary>
    public class GetOrderListRequretModel : GetPagedListBaseModel
    {
        public int ShopId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }

    }

    /// <summary>
    /// 拼团订单返回实体
    /// </summary>
    public class GetOrderListResultModel: ModelBase
    {
        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }
        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }
        /// <summary>
        /// 应付金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public int? Payment { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }
        /// <summary>
        /// 成团时间
        /// </summary>
        public DateTime ConglomerationSetUpCreateTime { get; set; }

        /// <summary>
        ///活动入口图
        /// </summary>
        public string CoverPortal { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 配送自提时间段开始(记录月日)
        /// </summary>
        public DateTime DeliveryTakeTheirBeginTimeMD { get; set; }

        /// <summary>
        /// 配送自提时间结束(记录月日)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeMD { get; set; }

        /// <summary>
        /// 配送自提时间段开始(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirBeginTimeHM { get; set; }

        /// <summary>
        /// 配送自提时间结束(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeHM { get; set; }
        /// <summary>
        /// 自提配送时间
        /// </summary>
        public DateTime Delivery { get; set; }

        /// <summary>
        /// 状态码字符串
        /// </summary>
        public string StatusStr { get; set; }


        /// <summary>
        /// 自提地址
        /// </summary>
        public string ShopAddress { get; set; }

        /// <summary>
        /// 应付金额（decimal类型）
        /// </summary>
        public decimal AmountM { get; set; }

        /// <summary>
        ///实际支付金额(decimal类型）
        /// </summary>
        public decimal PaymentM { get; set; }


        /// <summary>
        /// 快递信息
        /// </summary>
        public  ConglomerationExpress ConglomerationExpress { get; set; }


        /// <summary>
        /// 成团时间
        /// </summary>
        public DateTime? SuccessfulTime { get; set; }
    }


}
