using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopCallingQueueAPIModels
{

    public class GetListForMeArgsModel
    {
        public ShopCallingQueueStatus? Status { get; set; }
        public bool? IsUsed { get; set; }
        public string ShopFlag { get; set; }
    }

    public class GetListForMeModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// ÐÐ
    /// </summary>
    public class RowItem : ShopCallingQueue
    {
        public string ShopName { get; set; }
    }

    public class GetProductsArgsModel
    {
        public int ShopId { get; set; }
    }

    public class GetProductsModel
    {
        [JsonProperty("items")]
        public IList<GetProductsRowItem> Items { get; set; }
    }

    public class GetProductsRowItem : ShopCallingQueueProduct
    {

    }

    public class AddArgsModel
    {
        public int ShopId { get; set; }
        public int? ProductId { get; set; }
        public bool CanShareTable { get;  set; }
        public string Title { get; set; }
    }

    public class SetRemarkArgsModel : IdArgsModel
    {
        public string Remark { get; set; }
    }

}