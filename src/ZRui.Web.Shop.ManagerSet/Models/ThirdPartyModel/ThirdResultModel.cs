using System;
using System.Collections.Generic;
using System.Text;
using static ZRui.Web.Data.ThirdShop;

namespace ZRui.Web.Models.ThirdPartyModel
{
    public class ThirdResultModel
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public int result { get; set; }
    }
    /// <summary>
    /// 充值返回结果
    /// </summary>
    public class ThirdRechargeResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 充值链接
        /// </summary>
        public string result { get; set; }
    }

    /// <summary>
    /// 返回object
    /// </summary>
    public class ThirdObjectResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public object result { get; set; }
    }

    /// <summary>
    /// 订单取消返回结果
    /// </summary>
    public class ThirdCancelResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public ThirdCancel result { get; set; }
    }

    public class ThirdCancel {
        /// <summary>
        /// 违约金
        /// </summary>
        public double deduct_fee { get; set; }
    }


    /// <summary>
    /// 订单取消原因返回结果
    /// </summary>
    public class ThirdCancelReasonsResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public List<ThirdReasons> result { get; set; }
    }

    public class ThirdReasons
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
    /// 充值查询返回结果
    /// </summary>
    public class ThirdRechargeQueryResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 运费账户或红包账户的余额
        /// </summary>
        public ThirdAccountType result { get; set; }
    }

    public class ThirdAccountType
    {
        public double deliverBalance { get; set; }
        public double redPacketBalance { get; set; }
    }

    public class ThirdShopResultModel
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public ThirdShopResult result { get; set; }

    }

    public class ThirdShopResult
    {
        public int success { get; set; }
        public List<SuccessResult> successList { get; set; }
        public List<FaileResult> failedList { get; set; }



    }

    public class FaileResult
    {
        public string shopNo { get; set; }
        public string msg { get; set; }
        public string shopName { get; set; }
    }

    public class SuccessResult
    {
        public string phone { get; set; }
        public int business { get; set; }
        public string lng { get; set; }
        public string msg { get; set; }
        public string lat { get; set; }
        public string stationName { get; set; }
        public string originShopId { get; set; }
        public string contactName { get; set; }
        public string stationAddress { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
    }

    /// <summary>
    /// 发单
    /// </summary>
    public class ThirdAddOrderResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 发单业务参数
        /// </summary>
        public ThirdAddOrder result { get; set; }
    }
    /// <summary>
    /// 发单返回业务参数
    /// </summary>
    public class ThirdAddOrder
    {
        /// <summary>
        /// 配送距离（米）
        /// </summary>
        public double distance { get; set; }
        /// <summary>
        /// 实际运费（元）
        /// </summary>
        public double fee { get; set; }
        /// <summary>
        /// 运费（元）
        /// </summary>
        public double deliverFee { get; set; }
        /// <summary>
        /// 优惠券费用(单位：元)
        /// </summary>
        public double couponFee { get; set; }
        /// <summary>
        /// 小费(单位：元)
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 保价费(单位：元)
        /// </summary>
        public double insuranceFee { get; set; }

    }

    /// <summary>
    /// 订单详情查询返回参数
    /// </summary>
    public class ThirdInfoQueryResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 订单详情查询
        /// </summary>
        public ThirdInfoQuery result { get; set; }
    }


    /// <summary>
    /// 订单详情查询
    /// </summary>
    public class ThirdInfoQuery
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

    /// <summary>
    /// 门店详情查询返回参数
    /// </summary>
    public class ThirdShopInfoResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 订单详情查询
        /// </summary>
        public ThirdShopInfoResultModel result { get; set; }
    }

    /// <summary>
    /// 门店详情
    /// </summary>
    public class ThirdShopInfoResultModel
    {
        /// <summary>
        /// 门店编码
        /// </summary>
        public string origin_shop_id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string station_name { get; set; }

        /// <summary>
        ///业务类型(食品小吃-1, 饮料-2, 鲜花-3, 文印票务-8, 便利店-9, 水果生鲜-13, 同城电商-19, 医药-20, 蛋糕-21, 酒品-24, 小商品市场-25, 服装-26, 汽修零配-27, 数码-28, 小龙虾-29, 其他-5)
        /// </summary>  
        public BusinessType business { get; set; }
        /// <summary>
        ///城市名称(如, 上海)
        /// </summary>
        public string city_name { get; set; }
        /// <summary>
        ///区域名称(如, 浦东新区)
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string station_address { get; set; }

        /// <summary>
        /// 门店经度
        /// </summary>
        public double lng { get; set; }

        /// <summary>
        /// 门店纬度
        /// </summary>
        public double lat { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contact_name { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 联系人身份证
        /// </summary>
        public string id_card { get; set; }

        /// <summary>
        ///门店状态（1-门店激活，0-门店下线）
        /// </summary>
        public int status { get; set; }


    }




}
