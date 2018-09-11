using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.SettingBaseSetAPIModels
{
    public class GetListArgsModel : CommunityArgsModel
    {
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }

    public class GetPagedListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class RowItem:EntityBase
    {
        public string Flag { get; set; }
        public string GroupFlag { get; set; }
        public string Value { get; set; }
        public string Detail { get; set; }
        public SettingType SettingType { get; set; }
    }

    public class SetValueArgsModel : CommunityArgsModel
    {
        public string Flag { get; set; }
        public string GroupFlag { get; set; }
        public string Value { get; set; }
        public string Detail { get; set; }
    }

    public class GetValueArgsModel : CommunityArgsModel
    {
        public string Flag { get; set; }
        public string GroupFlag { get; set; }
    }
}