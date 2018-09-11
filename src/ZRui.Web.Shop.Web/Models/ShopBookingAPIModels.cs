using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBookingAPIModels
{
    public class GetListForMeArgsModel
    {
        public ShopBookingStatus? Status { get; set; }
        public bool? IsUsed { get; set; }
        public string ShopFlag { get; set; }
    }

    public class GetListForMeModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopBooking
    {
        public string ShopName { get; set; }
    }

    public class AddArgsModel
    {
        /// <summary>
        /// 预定的店铺
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public string Users { get; set; }
        /// <summary>
        /// 联系人昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 就餐时间
        /// </summary>
        public DateTime DinnerTime { get; set; }
    }

    public class SetRemarkArgsModel : IdArgsModel
    {
        public string Remark { get; set; }
    }

}