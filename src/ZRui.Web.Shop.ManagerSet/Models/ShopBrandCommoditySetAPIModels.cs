using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommoditySetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel
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

        public string SearchName { get; set; }

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
    public class RowItem : ShopBrandCommodity
    {
        /// <summary>
        /// �������
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// �ײ�����ids
        /// </summary>
        public List<string> CommodityIds { get; set; }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel 
    {
        /// <summary>
        /// ���������Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// ����������Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// �г���
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// ������λ
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// ��ϸ
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public int SalesForMonth { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public int Upvote { get; set; }
        /// <summary>
        /// �Ƿ��Ƽ�
        /// </summary>
        public bool IsRecommand { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// �Ͳͷ�ʽ
        /// </summary>
        public DiningWay DiningWay { get; set; } = DiningWay.����;
        /// <summary>
        /// ��Ʒids
        /// </summary>
        public List<int> CommodityIds { get; set; }
        /// <summary>
        /// �Ƿ�ɨ����
        /// </summary>
        public bool IsScanCode { get; set; }
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool IsTakeout { get; set; }
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsSelfOrder { get; set; }
        public bool UseMemberPrice { get; set; }
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }


    public class AddComboArgsModel
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Cover { get; set; }
        /// <summary>
        /// �Ƿ�ɨ����
        /// </summary>
        public bool IsScanCode { get; set; }
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool IsTakeout { get; set; }
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsSelfOrder { get; set; }
        public List<ComboItem> Items { get; set; }
    }


    public class ComboItem
    {
        public string CommodityName { get; set; }
        public int Count { get; set; }
        public string Sku { get; set; }
        public int SalePrice { get; set; }
    }

    public class GetComboSingleDto:AddComboArgsModel
    {

    }

    public class GetComboListDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodity
    {

    }
}