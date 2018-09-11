using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopStatisticsAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class ShopIdArgsModel
    {
        /// <summary>
        /// 商铺Id
        /// </summary>
        public int? ShopId { get; set; }
    }
    /// <summary>
    /// 获取总计的返回值类
    /// </summary>
    public class GetTotalModel
    {
        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int CommodityCount { get; set; }
        /// <summary>
        /// 用户数量
        /// </summary>
        public int MemberCount { get; set; }
    }


    public class GetCallingQueueModel
    {
        /// <summary>
        /// 成功叫号上座的人数
        /// </summary>
        public int CallingSuccessTotal { get; set; }
        /// <summary>
        /// 用户数量
        /// </summary>
        public int MemberCount { get; set; }
    }

    public class GetBookingModel
    {
        /// <summary>
        /// 所有预订人数
        /// </summary>
        public int BookTotal { get; set; }
        /// <summary>
        /// 预订并就餐人数
        /// </summary>
        public int BookAndUse { get; set; }
        /// <summary>
        /// 用户数量
        /// </summary>
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// 获取订单数日统计的参数类
    /// </summary>
    public class GetOrderCountForDayArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
    /// <summary>
    /// 获取订单数日统计的返回值类
    /// </summary>
    public class GetOrderCountForDayModel
    {
        /// <summary>
        /// 列表
        /// </summary>
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }


}