using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBrandCommoditySetAPIModels
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
    public class RowItem : ShopBrandCommodity
    {
        /// <summary>
        /// �������
        /// </summary>
        public string CategoryName { get; set; }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ���������Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// ����������Ʒ��Id
        /// </summary>
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
    }

    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }

    public class GetSingleModel : ShopBrandCommodity
    {

    }

    public class GetShopBrandsModel
    {
        public List<GetShopBrandsItem> Items { get; set; }
    }

    public class GetShopBrandsItem
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