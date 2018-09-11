using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    /// <summary>
    /// 第三方订单
    /// </summary>
    public class ThirdOrder
    {
        public int Id { get; set; }

        public int ShopId { get; set; }
        /// <summary>
        /// 返回达达运单号，默认为空
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 取消来源
        /// </summary>
        public CancelFrom CancelFrom { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 配送员id，接单以后会传
        /// </summary>
        public int DmId { get; set; }
        /// <summary>
        /// 配送员姓名，接单以后会传
        /// </summary>
        public string DmName { get; set; }
        /// <summary>
        /// 配送员手机，接单以后会传
        /// </summary>
        public string DmMobile { get; set; }
        /// <summary>
        /// 第三方配送类型
        /// </summary>
        public ThirdType ThirdType { get; set; }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public string ConvertStringToDateTime()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dt = startTime.AddSeconds(UpdateTime);
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }

    }

    public enum ThirdType
    {
        达达配送 = 1,
        其它配送 = 2
    }

    public enum CancelFrom
    {
        达达配送员取消 = 1,
        商家取消 = 2,
        系统或客服取消 = 3,
        默认值 = 4
    }

    public enum OrderStatus
    {
        待接单 = 1,
        待取货 = 2,
        配送中 = 3,
        已完成 = 4,
        已取消 = 5,
        已过期 = 7,
        指派单 = 8,
        妥投异常之物品返回中 = 9,
        妥投异常之物品返回完成 = 10,
        创建达达运单失败 = 1000
    }
}
