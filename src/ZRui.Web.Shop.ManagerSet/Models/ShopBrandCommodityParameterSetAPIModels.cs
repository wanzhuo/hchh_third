using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommodityParameterSetAPIModels
{
    /// <summary>
    /// ��ȡ�б����
    /// </summary>
    public class GetListArgsModel
    {
        /// <summary>
        /// ����Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// ��ȡ�б���ֵ
    /// </summary>
    public class GetListModel
    {
        /// <summary>
        /// �б�
        /// </summary>
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// ��ȡ��ҳ�б����
    /// </summary>
    public class GetPagedListArgsModel : GetListArgsModel
    {
        /// <summary>
        /// ҳ��
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// ÿҳ����
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// ������ֶ�����
        /// </summary>
        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        /// <summary>
        /// ������߽���
        /// </summary>
        [JsonProperty("orderType")]
        public string OrderType { get; set; }
    }
    /// <summary>
    /// ��ȡ��ҳ�б�ķ���ֵ
    /// </summary>
    public class GetPagedListModel : GetListModel
    {
        /// <summary>
        /// ҳ��
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// ÿҳ����
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// �ܼ�¼��
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopBrandCommodityParameter
    {

    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel 
    {
        /// <summary>
        /// ����������Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodityParameter
    {

    }
}