using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.CommercialDistrictShopSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int? CommercialDistrictId { get; set; }
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
    public class RowItem : CommercialDistrictShop
    {
        public string ShopName { get; set; }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ��������ȦId
        /// </summary>
        public int CommercialDistrictId { get; set; }
        /// <summary>
        /// ����������Id
        /// </summary>
        public int ShopId { get; set; }
    }

    public class GetCommercialDistrictsModel
    {
        public List<GetCommercialDistrictsItem> Items { get; set; }
    }

    public class GetCommercialDistrictsItem
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
    }
}