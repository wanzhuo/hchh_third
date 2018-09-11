using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.CommercialDistrictAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel
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

        /// <summary>
        /// γ��
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// ����1-12����Χ��
        /// </summary>
        public int? Precision { get; set; }

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

    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : CommercialDistrict
    {

    }
}