using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Data;

namespace ZRui.Web.BLL.Third
{
    public class ThirdPartyDistributionParameterModel
    {
        public string app_key { get; set; }
        public string signature { get; set; }
        public string timestamp { get; set; }
        public string format { get; set; }
        public string v { get; set; }
        public string source_id { get; set; }
        public string body { get; set; }
    }

    public class RechargeModel
    {
        public double amount { get; set; }
        public string category { get; set; }
        public string notify_url { get; set; }

    }

    public class CThirdShopRechargeQueryModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 查询运费账户类型（1：运费账户；2：红包账户，3：所有），默认查询运费账户余额
        /// </summary>
        public int category { get; set; }
    }

    /// <summary>
    /// 充值查询返回结果
    /// </summary>
    public class CThirdRechargeQueryResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 运费账户或红包账户的余额
        /// </summary>
        public CThirdAccountType result { get; set; }
    }

    public class CThirdAccountType
    {
        public double deliverBalance { get; set; }
        public double redPacketBalance { get; set; }
    }


    /// <summary>
    /// 订单信息查询
    /// </summary>
    public class COrderInfoQueryModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        public string order_id { get; set; }
    }

    /// <summary>
    /// 接单Model
    /// </summary>
    public class CThirdOrdersModel
    {
        /// <summary>
        /// 返回达达运单号，默认为空
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 添加订单接口中的origin_id值
        /// </summary>
        public string order_number { get; set; }
        public string OrderStatusStr { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus order_status { get; set; }
        /// <summary>
        /// 订单取消原因,其他状态下默认值为空字符串
        /// </summary>
        public string cancel_reason { get; set; }

        public string CancelFromStr { get; set; }
        /// <summary>
        /// 订单取消原因来源(1:达达配送员取消；2:商家主动取消；3:系统或客服取消；0:默认值)
        /// </summary>
        public CancelFrom cancel_from
        {
            get; set;

        }
        /// <summary>
        /// 更新时间,时间戳
        /// </summary>
        public int update_time { get; set; }
        /// <summary>
        /// 对client_id, order_id, update_time的值进行字符串升序排列，再连接字符串，取md5值
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 达达配送员id，接单以后会传
        /// </summary>
        public int dm_id { get; set; }
        /// <summary>
        /// 配送员姓名，接单以后会传
        /// </summary>
        public string dm_name { get; set; }
        /// <summary>
        /// 配送员手机号，接单以后会传
        /// </summary>
        public string dm_mobile { get; set; }
        public string ThirdTypeStr { get; set; }
        /// <summary>
        /// 配送类型 1 达达配送 2 其它配送
        /// </summary>
        public ThirdType third_type { get; set; }

        public string ThirdUpdateTime { get; set; }

    }


    public class CFormalCancel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 第三方订单编号
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 取消原因ID
        /// </summary>
        public int cancel_reason_id { get; set; }
        /// <summary>
        /// 取消原因(当取消原因ID为其他时，此字段必填)
        /// </summary>
        public string cancel_reason { get; set; }
    }
    /// <summary>
    /// 订单取消返回结果
    /// </summary>
    public class CThirdCancelResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public CThirdCancel result { get; set; }
    }

    public class CThirdCancel
    {
        /// <summary>
        /// 违约金
        /// </summary>
        public double deduct_fee { get; set; }
    }

    /// <summary>
    /// 订单取消原因返回结果
    /// </summary>
    public class CThirdCancelReasonsResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public List<CThirdReasons> result { get; set; }
    }

    public class CThirdReasons
    {
        /// <summary>
        /// 理由编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 取消理由
        /// </summary>
        public string reason { get; set; }
    }


    /// <summary>
    /// 订单详情查询返回参数
    /// </summary>
    public class CThirdInfoQueryResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 订单详情查询
        /// </summary>
        public CThirdInfoQuery result { get; set; }
    }


    /// <summary>
    /// 订单详情查询
    /// </summary>
    public class CThirdInfoQuery
    {
        /// <summary>
        /// 第三方订单编号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 订单状态(待接单＝1 待取货＝2 配送中＝3 已完成＝4 已取消＝5 已过期＝7 指派单=8 妥投异常之物品返回中=9 妥投异常之物品返回完成=10 系统故障订单发布失败=1000 可参考文末的状态说明）
        /// </summary>
        public int statusCode { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string statusMsg { get; set; }
        /// <summary>
        /// 骑手姓名
        /// </summary>
        public string transporterName { get; set; }
        /// <summary>
        /// 骑手电话
        /// </summary>
        public string transporterPhone { get; set; }
        /// <summary>
        /// 骑手经度
        /// </summary>
        public string transporterLng { get; set; }
        /// <summary>
        /// 骑手纬度
        /// </summary>
        public string transporterLat { get; set; }
        /// <summary>
        /// 配送费,单位为元
        /// </summary>
        public double deliveryFee { get; set; }
        /// <summary>
        /// 小费,单位为元
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 优惠券费用,单位为元
        /// </summary>
        public double couponFee { get; set; }
        /// <summary>
        /// 保价费,单位为元
        /// </summary>
        public double insuranceFee { get; set; }
        /// <summary>
        /// 实际支付费用,单位为元
        /// </summary>
        public double actualFee { get; set; }
        /// <summary>
        /// 配送距离（米）
        /// </summary>
        public double distance { get; set; }
        /// <summary>
        /// 发单时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 接单时间,若未接单,则为空
        /// </summary>
        public string acceptTime { get; set; }
        /// <summary>
        /// 取货时间,若未取货,则为空
        /// </summary>
        public string fetchTime { get; set; }
        /// <summary>
        /// 送达时间,若未送达,则为空
        /// </summary>
        public string finishTime { get; set; }
        /// <summary>
        /// 取消时间,若未取消,则为空
        /// </summary>
        public string cancelTime { get; set; }
        /// <summary>
        /// 收货码
        /// </summary>
        public string orderFinishCode { get; set; }

    }
}
