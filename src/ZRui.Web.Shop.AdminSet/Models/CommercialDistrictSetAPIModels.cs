using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.CommercialDistrictSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
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

    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : CommercialDistrict
    {
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// һЩ˵��
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// γ��
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        public CommercialDistrictStatus? Status { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : CommercialDistrict
    {

    }
}