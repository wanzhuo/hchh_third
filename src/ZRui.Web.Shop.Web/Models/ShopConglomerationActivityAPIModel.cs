using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZRui.Web.Models;

namespace ZRui.Web
{
    public class ShopConglomerationActivityAPIModel
    {
    }

    /// <summary>
    /// 拼团活动业务实体
    /// </summary>
    public class ActivityModel : ModelBase
    {
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
        public DateTime DeliveryTakeTheirBeginTimeMD
        {
            get;

            set;

        }

        /// <summary>
        /// 配送自提时间结束(记录月日)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeMD
        {
            get;

            set;

        }


        /// <summary>
        /// 配送自提时间段开始(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirBeginTimeHM
        {
            get;

            set;

        }

        /// <summary>
        /// 配送自提时间结束(记录时分)
        /// </summary>
        public DateTime DeliveryTakeTheirEndTimeHM
        {
            get;

            set;

        }

        /// <summary>
        /// 配送费
        /// </summary>
        public int ActivityDeliveryFee { get; set; }

        public decimal ActivityDeliveryFeeM { get { return ActivityDeliveryFee / 100.00M; } }


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

        public decimal MarketPriceM { get { return MarketPrice / 100.00M; } }

        /// <summary>
        /// 配送方式
        /// </summary>
        public Dictionary<string, ConsignmentType> ConglomerationActivityPickingSettings { get; set; }

        /// <summary>
        /// 最低拼团价格
        /// </summary>
        public int MinConglomerationPriceM { get; set; }



        /// <summary>
        /// 最低拼团价格
        /// </summary>
        public decimal ConglomerationLowestPrice { get; set; }


        /// <summary>
        /// 最最高拼团价格
        /// </summary>
        public decimal ConglomerationToptPrice { get; set; }
    }


    /// <summary>
    /// 拼团类型表
    /// </summary>
    public class ConglomerationActivityTypeModel : ModelBase
    {

        /// <summary>
        /// 拼团人数
        /// </summary>
        public int ConglomerationMembers { get; set; }
        /// <summary>
        /// 拼团价格
        /// </summary>
        public int ConglomerationPrice { get; set; }
        public decimal ConglomerationPriceM { get { return ConglomerationPrice / 100.00M; } }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 拼团类型描述
        /// </summary>
        public string TypeDescribe { get; set; }
    }

    /// <summary>
    /// GetInitiateConglomerationOrderDetails方法返回实体
    /// </summary>
    public class GetInitiateConglomerationOrderDetailsResultModel
    {
        /// <summary>
        /// 活动封面图
        /// </summary>
        public string CoverPortal { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 拼团价格 
        /// </summary>
        public decimal ConglomerationPriceM { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public Dictionary<ConsignmentType, string> PickingSetting { get; set; }

        /// <summary>
        /// 支付价格
        /// </summary>
        public decimal PayPrice { get; set; }
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
        public decimal ActivityDeliveryFeeM { get; set; }

    }

    /// <summary>
    /// GetInitiateConglomerationOrderDetails方法请求实体
    /// </summary>
    public class ConglomerationSetUpModel : CreateModelBase
    {


        /// <summary>
        /// 所需人数
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// 当前人数
        /// </summary>
        public int CurrentMemberNumber { get; set; }

        /// <summary>
        /// 拼团状态
        /// </summary>
        public ConglomerationSetUpStatus Status { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        [Required]
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>

        public ActivityModel ConglomerationActivity { get; set; }

        /// <summary>
        /// 拼团结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 拼团类型ID
        /// </summary>
        [Required]
        public int ConglomerationActivityTypeId { get; set; }

        /// <summary>
        /// 关联拼团类型
        /// </summary>
        public ConglomerationActivityTypeModel ConglomerationActivityType { get; set; }



        /// <summary>
        /// 送货地址
        /// </summary>
        public int MemberAddressId { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        [Required]
        public int ShopId { get; set; }
        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        [Required]
        public ConsignmentType Type { get; set; }


        /// <summary>
        /// 发起人关联的用户Id
        /// </summary>
        public int MemberId { get; set; }


        /// <summary>
        /// 自提配送时间
        /// </summary>
        public DateTime Delivery { get; set; }

        /// <summary>
        /// 小程序提交的formId
        /// </summary>
        public string FormId { get; set; }

    }


    /// <summary>
    /// 拼团活动业务实体
    /// </summary>

    public class GetActivityPageListResultModel : ModelBase
    {
        /// <summary>
        ///活动入口图
        /// </summary>

        public string CoverPortal { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>

        public string ActivityName { get; set; }



        /// <summary>
        /// 市场价格
        /// </summary>
        public int MarketPrice { get; set; }
        public decimal MarketPriceM { get; set; }



        /// <summary>
        /// 最低拼团价格
        /// </summary>
        public decimal ConglomerationLowestPrice { get; set; }


        /// <summary>
        /// 最最高拼团价格
        /// </summary>
        public decimal ConglomerationToptPrice { get; set; }


        /// <summary>
        /// 参加人数（已参团）
        /// </summary>

        public int Participants { get; set; }


        /// <summary>
        /// 活动结束时间
        /// </summary>

        public DateTime ActivityEndTime { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>

        public DateTime ActivityBeginTime { get; set; }

    }

    /// <summary>
    /// 获取活动列表参数
    /// </summary>
    public class ActivityPageList : GetPagedListBaseModel
    {

        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }
    }

    /// <summary>
    /// 参团请求参数（需要取消已经移动至BLLDto）
    /// </summary>
    public class ParticipationModel
    {
        /// <summary>
        /// 已发起拼团表FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }




        /// <summary>
        /// 送货地址
        /// </summary>
        public int MemberAddressId { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        [Required]
        public int ShopId { get; set; }
        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        [Required]
        public ConsignmentType Type { get; set; }

        /// <summary>
        /// 自提配送时间
        /// </summary>
        public DateTime? Delivery { get; set; }


        /// <summary>
        /// 小程序提交的formId
        /// </summary>
        public string FormId { get; set; }

    }

    /// <summary>
    /// 获取发起拼团请求实体
    /// </summary>
    public class GetConglomerationSetUpModel : GetPagedListBaseModel
    {

        /// <summary>
        /// 活动ID
        /// </summary>
        [Required]
        public int ConglomerationActivityId { get; set; }
    }


    /// <summary>
    /// 获取可参与拼团列表返回实体
    /// </summary>
    public class GetConglomerationSetUpResultModel : ModelBase
    {
        /// <summary>
        /// 所需人数
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团类型Id
        /// </summary>
        public int ConglomerationActivityTypeId { get; set; }

        /// <summary>
        /// 拼团结束时间
        /// </summary>

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 发起人关联的用户Id
        /// </summary>

        public int MemberId { get; set; }
        public ConglomerationMemberBaseInfo Initiator { get; set; }


        /// <summary>
        /// 参团人员信息
        /// </summary>
        public ICollection<ConglomerationMemberBaseInfo> ConglomerationParticipations { get; set; }

        /// <summary>
        /// 当前人数
        /// </summary>
        public int CurrentMemberNumber { get; set; }

        /// <summary>
        /// 拼团类型
        /// </summary>
        public ConglomerationActivityTypeModel ConglomerationActivityType { get; set; }

    }

    /// <summary>
    /// 基本信息
    /// </summary>
    public class ConglomerationMemberBaseInfo
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 在以参加的拼团中的角色
        /// </summary>
        public ParticipationRole Role { get; set; }
    }


    /// <summary>
    /// 获取拼团详细信息请求实体
    /// </summary>
    public class GetConglomerationSetUpDetailsRequetModel
    {
        /// <summary>
        /// 已发起拼团表FK_Id
        /// </summary>
        //public int ConglomerationSetUpId { get; set; }
        public int OrderId { get; set; }
    }

    /// <summary>
    /// 获取拼团详细信息返回实体
    /// </summary>
    public class GetConglomerationSetUpDetailsResultModel : ModelBase
    {


        /// <summary>
        /// 所需人数
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// 当前人数
        /// </summary>
        public int CurrentMemberNumber { get; set; }

        /// <summary>
        /// 拼团状态
        /// </summary>
        public ConglomerationSetUpStatus Status { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>

        public ActivityModel ConglomerationActivity { get; set; }



        /// <summary>
        /// 拼团结束时间
        /// </summary>

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 发起人关联的用户Id
        /// </summary>

        public int MemberId { get; set; }

        /// <summary>
        /// 拼团价格
        /// </summary>
        public decimal ConglomerationPriceM { get; set; }

        public virtual ICollection<ConglomerationParticipationModel> ConglomerationParticipations { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int ConglomerationOrderId { get; set; }

        /// <summary>
        /// 活动类型Id
        /// </summary>
        public int ConglomerationActivityTypeId { get; set; }
    }




    public class ConglomerationParticipationModel : ModelBase
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
        /// 角色（1：团长 2：团员）
        /// </summary>
        public ParticipationRole Role { get; set; }

        /// <summary>
        /// 会员FK_Id
        /// </summary>
        public int MemberId { get; set; }

    }



    /// <summary>
    /// 取消支付请求实体
    /// </summary>
    public class CancelThePayment
    {
        public int ConglomerationOrderId { get; set; }
    }
}
