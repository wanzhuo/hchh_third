using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int? ShopBrandId { get; set; }
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
    public class RowItem : Shop
    {
        public string PayWay { get; set; }
        public string MchId { get; set; }
        public string SecretKey { get; set; }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        public int ShopBrandId { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// �˾�����
        /// </summary>
        public string UsePerUser { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ��ַ������Ϣ
        /// </summary>
        public string AddressGuide { get; set; }
        /// <summary>
        /// γ��
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// Ӫҵʱ��/����ʱ��
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// ����ֵ
        /// </summary>
        public int ScoreValue { get; set; }
        /// <summary>
        /// һЩ˵��
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get;  set; }
        /// <summary>
        /// �Ƿ���ʾ
        /// </summary>
        public bool IsShowApplets { get; set; }

        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        public string Phone { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : Shop
    {

    }
}