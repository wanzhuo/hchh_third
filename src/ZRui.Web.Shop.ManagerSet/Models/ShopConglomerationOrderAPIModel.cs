using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{

    /// <summary>
    /// 拼团订单请求实体
    /// </summary>
    public class ShopConglomerationOrderAPIModel : GetPagedListBaseModel
    {

    }

    /// <summary>
    /// 添加快递单号实体
    /// </summary>
    public class AddExpressSingleModel
    {
        public int OrderId { get; set; }
        /// <summary>
        /// 快递订单号
        /// </summary>
        public string ExpressSingle { get; set; }

        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }
    }


    /// <summary>
    /// 拼团订单返回实体
    /// </summary>
    public class ConglomerationOrderListResultModel : ModelBase
    {

        /// <summary>
        /// 微信头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
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

        ///// <summary>
        ///// 配送自提时间段开始(记录月日)
        ///// </summary>
        //public DateTime DeliveryTakeTheirBeginTimeMD { get; set; }

        ///// <summary>
        ///// 配送自提时间结束(记录月日)
        ///// </summary>
        //public DateTime DeliveryTakeTheirEndTimeMD { get; set; }

        ///// <summary>
        ///// 配送自提时间段开始(记录时分)
        ///// </summary>
        //public DateTime DeliveryTakeTheirBeginTimeHM { get; set; }

        ///// <summary>
        ///// 配送自提时间结束(记录时分)
        ///// </summary>
        //public DateTime DeliveryTakeTheirEndTimeHM { get; set; }


        //public DateTime PickupBeginTime { get { return new DateTime(DateTime.Now.Year, DeliveryTakeTheirBeginTimeMD.Month, DeliveryTakeTheirBeginTimeMD.Day, DeliveryTakeTheirBeginTimeHM.Hour, DeliveryTakeTheirBeginTimeHM.Minute, DeliveryTakeTheirBeginTimeHM.Second); } }
        //public DateTime PickupEndTimeHM { get { return new DateTime(DateTime.Now.Year, DeliveryTakeTheirEndTimeMD.Month, DeliveryTakeTheirEndTimeMD.Day, DeliveryTakeTheirEndTimeHM.Hour, DeliveryTakeTheirEndTimeHM.Minute, DeliveryTakeTheirEndTimeHM.Second); } }


        /// <summary>
        /// 用户ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime Delivery { get; set; }

        /// <summary>
        /// 自提时间
        /// </summary>
        public string Deliverys { get; set; }

        /// <summary>
        /// 快递信息
        /// </summary>
        public ConglomerationExpress ConglomerationExpress { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }

    }


    /// <summary>
    /// 拼团订单退款请求实体
    /// </summary>
    public class ConglomerationOrderRequestModel
    {
        /// <summary>
        /// 拼团订单Id
        /// </summary>
        public int ConglomerationOrderId { get; set; }


        /// <summary>
        /// 店铺ID
        /// </summary>
        public int ShopId { get; set; }
        ///// <summary>
        ///// 快递订单号
        ///// </summary>
        //public string ExpressSingle { get; set; }

    }

}
