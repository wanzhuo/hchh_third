using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团活动表
    /// </summary>
    public class ConglomerationActivity : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///活动入口图
        /// </summary>
        public string CoverPortal { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 活动简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 活动内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime ActivityEndTime { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime ActivityBeginTime { get; set; }

        /// <summary>
        /// 曝光量
        /// </summary>
        public int BrowseNumber { get; set; }

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
        /// 配送费
        /// </summary>
        public int ActivityDeliveryFee { get; set; }


        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }


        /// <summary>
        /// 市场价格
        /// </summary>
        public int MarketPrice { get; set; }


        /// <summary>
        /// 拼团倒计时剩余分钟（单位分）
        /// </summary>
        public int ConglomerationCountdown { get; set; }


        /// <summary>
        /// 活动状态
        /// </summary>
        public ConglomerationActivityStatut ConglomerationActivityStatut { get; set; }



        /// <summary>
        /// 拼团类型
        /// </summary>
        public virtual ICollection<ConglomerationActivityType> ConglomerationActivityTypes { get; set; }
    }

    /// <summary>
    /// 活动状态
    /// </summary>
    public enum ConglomerationActivityStatut
    {
        待发布 =1,
        已发布 = 2
    }

}

