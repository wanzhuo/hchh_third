using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团订单表
    /// </summary>
    public class ConglomerationOrder : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }


        /// <summary>
        /// 已发起的拼团FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }

        /// <summary>
        /// 已发起的拼团
        /// </summary>
        [ForeignKey("ConglomerationSetUpId")]
        public virtual ConglomerationSetUp ConglomerationSetUp { get; set; }

        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }



        /// <summary>
        /// 应付金额(不包含优惠)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 实际支付金额(最终支付，包含所有费用)
        /// </summary>
        public int? Payment { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime FinishTime { get; set; }



        /// <summary>
        /// 关联的商铺fK_Id
        /// </summary>
        public int ShopId { get; set; }


        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        [ForeignKey("ConglomerationActivityId")]
        public virtual ConglomerationActivity ConglomerationActivity { get; set; }



        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }


        /// <summary>
        /// 快递配配送信息外键
        /// </summary>
        public int ConglomerationExpressId { get; set; }
        /// <summary>
        /// 快递配配送信息
        /// </summary>
        [ForeignKey("ConglomerationExpressId")]
        public virtual ConglomerationExpress ConglomerationExpress { get; set; }


        /// <summary>
        /// 自提时间
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// 是否推送
        /// </summary>
        public bool IsSend { get; set; }



        /// <summary>
        /// 小程序提交的formId
        /// </summary>
        public string FormId { get; set; }



        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }
    }


    public enum ConsignmentType
    {
        自提 = 1,
        快递 = 2
    }


}
