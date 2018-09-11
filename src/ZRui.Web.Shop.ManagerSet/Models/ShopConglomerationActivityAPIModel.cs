using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZRui.Web.Models
{
    public class ShopConglomerationActivityAPIModel
    {
        public ActivityModel ActivityModel { get; set; }
    }

    /// <summary>
    /// 拼团活动业务实体
    /// </summary>
    public class ActivityModel : ModelBase
    {
        #region 活动



        /// <summary>
        ///活动入口图
        /// </summary>
        [Required]
        public string CoverPortal { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [Required]
        public string ActivityName { get; set; }

        /// <summary>
        /// 活动简介
        /// </summary>
        [Required]
        public string Intro { get; set; }

        [Required]
        /// <summary>
        /// 活动内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        [Required]
        public DateTime ActivityEndTime { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [Required]
        public DateTime ActivityBeginTime { get; set; }

        /// <summary>
        /// 曝光量
        /// </summary>
        public int BrowseNumber { get; set; }

        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        [Required]
        public int ShopId { get; set; }


        /// <summary>
        /// 市场价格
        /// </summary>
        [Required]
        public Decimal MarketPriceM { get; set; }

        public int MarketPrice
        {
            get
            {
                return (int)(MarketPriceM * 100);

            }

        }

        /// <summary>
        /// 拼团倒计时剩余分钟（单位分）
        /// </summary>
        public int ConglomerationCountdown { get; set; }
        #endregion

        #region 配送配置



        /// <summary>
        /// 配送方式配置集合 1自提，2快递
        /// </summary>
        public int[] Type { get; set; }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }


        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string PickingSettingName { get; set; }

        DateTime _DeliveryTakeTheirBeginTimeMD;
        /// <summary>
        /// 配送自提时间段开始(记录月日)
        /// </summary>
        public DateTime DeliveryTakeTheirBeginTimeMD
        {
            get
            {
                return new DateTime(0001, _DeliveryTakeTheirBeginTimeMD.Month, _DeliveryTakeTheirBeginTimeMD.Day);
            }
            set
            {

                _DeliveryTakeTheirBeginTimeMD = value;
            }
        }

        DateTime _DeliveryTakeTheirEndTimeMD;
        /// <summary>
        /// 配送自提时间结束(记录月日)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeMD
        {
            get
            {
                return new DateTime(_DeliveryTakeTheirEndTimeMD.Year, _DeliveryTakeTheirEndTimeMD.Month, _DeliveryTakeTheirEndTimeMD.Day);
            }
            set
            {

                _DeliveryTakeTheirEndTimeMD = value;
            }
        }


        DateTime _DeliveryTakeTheirBeginTimeHM;
        /// <summary>
        /// 配送自提时间段开始(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirBeginTimeHM
        {
            get
            {
                return new DateTime(0001, 01, 01, _DeliveryTakeTheirBeginTimeHM.Hour, _DeliveryTakeTheirBeginTimeHM.Minute, _DeliveryTakeTheirBeginTimeHM.Second);
            }
            set
            {

                _DeliveryTakeTheirBeginTimeHM = value;
            }
        }

        DateTime _DeliveryTakeTheirEndTimeHM;
        /// <summary>
        /// 配送自提时间结束(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeHM
        {
            get
            {
                return new DateTime(0001, 01, 01, _DeliveryTakeTheirEndTimeHM.Hour, _DeliveryTakeTheirEndTimeHM.Minute, _DeliveryTakeTheirEndTimeHM.Second);
            }
            set
            {

                _DeliveryTakeTheirEndTimeHM = value;
            }
        }

        decimal _ActivityDeliveryFee;
        /// <summary>
        /// 配送费
        /// </summary>
        public decimal ActivityDeliveryFee
        {
            get
            {
                return _ActivityDeliveryFee * 100;

            }
            set { _ActivityDeliveryFee = value; }
        }


        #endregion


        /// <summary>
        /// 活动状态
        /// </summary>
        public ConglomerationActivityStatut ConglomerationActivityStatut { get; set; }

        /// <summary>
        /// 拼团活动类型
        /// </summary>
        public ICollection<ConglomerationActivityTypeModel> ConglomerationActivityTypes { get; set; }

    }


    /// <summary>
    /// 拼团类型业务实体
    /// </summary>
    public class ConglomerationActivityTypeModel : ModelBase
    {
        /// <summary>
        /// 拼团人数
        /// </summary>
        [Required]
        public int ConglomerationMembers { get; set; }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        [Required]
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 引用的活动
        /// </summary>

        public ActivityModel ConglomerationActivity { get; set; }

        /// <summary>
        /// 拼团类型描述
        /// </summary>
        public string TypeDescribe { get; set; }

        /// <summary>
        /// 拼团价格
        /// </summary>
        public decimal ConglomerationPriceM { get; set; }
    }


    /// <summary>
    /// 获取活动详情请求实体
    /// </summary>
    public class GetActivityDetailsModel : ModelBase
    {

    }

    ///// <summary>
    ///// 配置配送方式实体
    ///// </summary>
    //public class SetPickingModel : ModelBase
    //{



    //    /// <summary>
    //    /// 配送方式配置集合 1自提，2快递
    //    /// </summary>
    //    public int[] Type { get; set; }

    //    /// <summary>
    //    /// 拼团活动KF_Id
    //    /// </summary>
    //    public int ConglomerationActivityId { get; set; }


    //    /// <summary>
    //    /// 配送方式名称
    //    /// </summary>
    //    public string PickingSettingName { get; set; }

    //    DateTime _DeliveryTakeTheirBeginTimeMD;
    //    /// <summary>
    //    /// 配送自提时间段开始(记录月日)
    //    /// </summary>
    //    public DateTime DeliveryTakeTheirBeginTimeMD
    //    {
    //        get
    //        {
    //            return new DateTime(0001, _DeliveryTakeTheirBeginTimeMD.Month, _DeliveryTakeTheirBeginTimeMD.Month);
    //        }
    //        set
    //        {

    //            _DeliveryTakeTheirBeginTimeMD = value;
    //        }
    //    }

    //    DateTime _DeliveryTakeTheirEndTimeMD;
    //    /// <summary>
    //    /// 配送自提时间结束(记录月日)
    //    /// </summary>
    //    public DateTime DeliveryTakeTheirEndTimeMD
    //    {
    //        get
    //        {
    //            return new DateTime(0001, _DeliveryTakeTheirEndTimeMD.Month, _DeliveryTakeTheirEndTimeMD.Month);
    //        }
    //        set
    //        {

    //            _DeliveryTakeTheirEndTimeMD = value;
    //        }
    //    }


    //    DateTime _DeliveryTakeTheirBeginTimeHM;
    //    /// <summary>
    //    /// 配送自提时间段开始(记录时分)
    //    /// </summary>
    //    public DateTime DeliveryTakeTheirBeginTimeHM
    //    {
    //        get
    //        {
    //            return new DateTime(0001, 01, 01, _DeliveryTakeTheirBeginTimeHM.Hour, _DeliveryTakeTheirBeginTimeHM.Minute, _DeliveryTakeTheirBeginTimeHM.Second);
    //        }
    //        set
    //        {

    //            _DeliveryTakeTheirBeginTimeHM = value;
    //        }
    //    }

    //    DateTime _DeliveryTakeTheirEndTimeHM;
    //    /// <summary>
    //    /// 配送自提时间结束(记录时分)
    //    /// </summary>
    //    public DateTime DeliveryTakeTheirEndTimeHM
    //    {
    //        get
    //        {
    //            return new DateTime(0001, 01, 01, _DeliveryTakeTheirEndTimeHM.Hour, _DeliveryTakeTheirEndTimeHM.Minute, _DeliveryTakeTheirEndTimeHM.Second);
    //        }
    //        set
    //        {

    //            _DeliveryTakeTheirEndTimeHM = value;
    //        }
    //    }

    //    decimal _ActivityDeliveryFee;
    //    /// <summary>
    //    /// 配送费
    //    /// </summary>
    //    public decimal ActivityDeliveryFee
    //    {
    //        get
    //        {
    //            return _ActivityDeliveryFee * 100;

    //        }
    //        set { _ActivityDeliveryFee = value; }
    //    }

    //}
}
