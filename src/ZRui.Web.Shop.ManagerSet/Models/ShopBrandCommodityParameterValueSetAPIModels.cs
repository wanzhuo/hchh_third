using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommodityParameterValueSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel 
    {
        public int? ParameterId { get; set; }
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

    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopBrandCommodityParameterValue
    {

    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel 
    {
        /// <summary>
        /// ����������Id
        /// </summary>
        public int ParameterId { get; set; }
        /// <summary>
        /// ֵ
        /// </summary>
        public string Value { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodityParameterValue
    {

    }
}