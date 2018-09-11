using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团快递信息表
    /// </summary>
    public class ConglomerationExpress : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 快递订单号
        /// </summary>
        public string ExpressSingle { get; set; }

        /// <summary>
        /// 配送地址Fk_Id
        /// </summary>
        public int? MemberAddressId { get; set; }

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

        /// <summary>
        /// 期望配送时间
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        public string Address { get; set; }

        ///// <summary>
        ///// 快递状态
        ///// </summary>
        //public Status Status { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        public int ActivityDeliveryFee { get; set; }


        /// <summary>
        /// 拼团订单IFk_Id
        /// </summary>
        public int ShopConglomerationOrderId { get; set; }
    }

 

}
