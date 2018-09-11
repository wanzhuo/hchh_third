using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopOrderSetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        public string SearchId { get; set; }
    }

    public class GetShopOrderViewArgsModel
    {
        public int orderid { get; set; }
        public string openid { get; set; }
    }

    public class SetTakeoutStatusArgsModel
    {
        public int? Id { get; set; }
        public Status? status { get; set; }
    }

    public class GetShopOrderViewResultModel : ShopOrder
    {
        public string ShopPartName { get; set; }
        public double OrderAmount { get; set; }
        public double PayAmount { get; set; }
        public string Address { get; set; }
        public string TakeWay { get; set; }
        public string Headimgurl { get; set; }

        public string NickName { get; set; }
        public string Name { get; set; }
        public DateTime PickupTime { get; set; }
        public List<ShopOrderItemInfo> ShopOrderItems { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }
    }

    public class ShopOrderItemInfo
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double Amount { get; set; }
        public string SkuSummary { get; set; }
    }

    public class GetPagedListModel : GetListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }


    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopOrder
    {
        public string ShopPartTitle { get; set; }
        public string Address { get; set; }
        public string TakeOutPhone { get; set; }
        public string TakeOutName { get; set; }
        public Status? TakeOutStatus { get; set; }
    }


    public class GetOrderItemsArgsModel
    {
        public int OrderId { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderTypeE OrderType { get; set; }
    }

    public class SetStatusArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 新状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }
    }


    #region 订单列表大改添加的代码
    /// <summary>
    /// GetPagedList返回实体
    /// </summary>
    public class GetPagedListResulrModel
    {
        public OrderTypeE OrderType { get; set; }
        public int OrderId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 关联的商铺Id
        /// </summary>
        public int ShopId { get; set; }
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
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        public string ShopPartTitle { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }
        /// <summary>
        /// 订单状态描述
        /// </summary>
        public string StatusStr { get; set; }




        /// <summary>
        /// 外卖地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 快递电话
        /// </summary>
        public string TakeOutPhone { get; set; }
        /// <summary>
        /// 外卖姓名
        /// </summary>
        public string TakeOutName { get; set; }
        /// <summary>
        /// 外卖状态
        /// </summary>
        public Status TakeOutStatus { get; set; }

        /// <summary>
        /// 下单方式
        /// </summary>
        public TakeWay TakeWay { get; set; }

        /// <summary>
        /// 拼团状态
        /// </summary>
        public ConglomerationSetUpStatus ConglomerationSetUpStatus { get; set; }
    }


    /// <summary>
    /// 返回订单列表实体
    /// </summary>
    public class ConglomerationOrderModel : ModelBase
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        public string TypeStr { get; set; }


        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public int Amount { get; set; }
        public decimal AmountM { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public int? Payment { get; set; }
        public decimal PaymentM { get; set; }

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
        /// 关联拼团活动
        /// </summary>
        public ConglomerationOrderActivityModel ConglomerationActivity { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }

        /// <summary>
        /// 状态（字符串）
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// 快递信息
        /// </summary>
        public ConglomerationOrderConglomerationExpressModel ConglomerationExpress { get; set; }


        /// <summary>
        /// 自提时间
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// 成团时间
        /// </summary>
        public DateTime? SuccessfulTime { get; set; }


        /// <summary>
        /// 订单用户信息
        /// </summary>
        public OrderShopMember OrderShopMember { get; set; }


        public int MemberId { get; set; }


        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }
    }

    /// <summary>
    /// 订单详情内的活动信息实体
    /// </summary>
    public class ConglomerationOrderActivityModel
    {
        /// <summary>
        /// 活动名称
        /// </summary>

        public string ActivityName { get; set; }


        public string Deliverys { get; set; }
    }


    /// <summary>
    /// 快递信息
    /// </summary>
    public class ConglomerationOrderConglomerationExpressModel
    {
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


        /// <summary>
        /// 快递订单号
        /// </summary>
        public string ExpressSingle { get; set; }

        ///// <summary>
        ///// 快递状态
        ///// </summary>
        //public Status Status { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        public int ActivityDeliveryFee { get; set; }
        public decimal ActivityDeliveryFeeM { get; set; }


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

        public decimal MarketPriceM { get { return ConglomerationCountdown / 100.00M; } }


    }





    public class GetPagedListRequestModel : GetPagedListBaseModel
    {

        public OrderTypeE OrderType { get; set; }
        public string SearchId { get; set; } = "";
    }
    public enum OrderTypeE
    {
        扫码点餐订单 = 1,
        外卖订单 = 2,
        自助点餐订单 = 3,
        拼团订单 = 4
    }
    #endregion



    public class GetOrderInfoResultModel
    {
        public SelfHelpInfo SelfHelp { get; set; }
        public object Items { get; set; }
        public OtherFee OtherFee { get; set; }
        public MoneyOffRule MoneyOffRule { get; set; }
        public TakeOutInfo TakeOutInfo { get; set; }
        public decimal? PayMent { get; set; }
        public string CreateTime { get; set; }
        public string PayTime { get; set; }
        public string Code { get; set; }
        public string ShopPartName { get; set; }
        public string Remark { get; set; }

        public OrderShopMember OrderShopMember { get; set; }



        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayWay { get; set; }
    }

    public class SelfHelpInfo
    {
        public string SelfHelpCode { get; set; }
        public string TakeWay { get; set; }
    }

    public class TakeOutInfo
    {
        public string PickTile { get; set; }
        public string PickUpTime { get; set; }
        public string DiningWay { get; set; }
        public string Address { get; set; }
        public string Person { get; set; }
        public string Phone { get; set; }
    }

    public class MoneyOffRule
    {
        public decimal FullAmount { get; set; }
        public decimal Discount { get; set; }
    }

    public class OtherFee
    {
        public decimal BoxFee { get; set; }
        public decimal DeliveryFee { get; set; }
    }

    /// <summary>
    /// 订单内会员信息
    /// </summary>
    public class OrderShopMember
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

    }

}