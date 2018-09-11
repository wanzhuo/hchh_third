using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommodityCategorySetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel
    {
        /// <summary>
        /// ����Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// ��ȡ�б���ֵ��
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
    /// ��ȡ��ҳ�б������
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
    /// ��ȡ��ҳ�б�ķ���ֵ��
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
    public class RowItem : ShopBrandCommodityCategory
    {
        /// <summary>
        /// �ϼ���������
        /// </summary>
        public string ParentName { get; set; }
    }
    /// <summary>
    /// ��ȡ������Ĳ�����
    /// </summary>
    public class GetTreeArgsModel
    {
        /// <summary>
        /// ����Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// ��ȡ������ķ�����
    /// </summary>
    public class GetTreeModel
    {
        /// <summary>
        /// ��
        /// </summary>
        [JsonProperty("tree")]
        public List<TreeNode> Tree { get; set; }
        /// <summary>
        /// ���нڵ�
        /// </summary>
        [JsonIgnore]
        public List<TreeNode> AllNodes { get; set; }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="items">�б�</param>
        public GetTreeModel(List<RowItem> items)
        {
            AllNodes = items.Select(m => new TreeNode(this)
            {
                Name = m.Name,
                Detail = m.Detail,
                Flag = m.Flag,
                Id = m.Id,
                OrderWeight = m.OrderWeight,
                PId = m.PId,
                Keywords = m.Keywords,
                Ico = m.Ico,
                Description = m.Description,
                ParentName = m.ParentName,
                Tags = m.Tags
            }).ToList();

            Tree = AllNodes.Where(m => !m.PId.HasValue || m.PId == 0).OrderByDescending(m => m.OrderWeight).ToList();
        }
    }
    /// <summary>
    /// ���ڵ�
    /// </summary>
    public class TreeNode : ShopBrandCommodityCategory
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Title
        {
            get
            {
                return base.Name;
            }
        }
        /// <summary>
        /// �ڵ��б�
        /// </summary>
        public List<TreeNode> Nodes
        {
            get
            {
                return root.AllNodes.Where(m => m.PId == Id).OrderByDescending(m => m.OrderWeight).ToList();
            }
        }

        /// <summary>
        /// �ϼ��������
        /// </summary>
        public string ParentName { get; internal set; }

        GetTreeModel root;
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="root">���ڵ�</param>
        public TreeNode(GetTreeModel root)
        {
            this.root = root;
        }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel
    {
        /// <summary>
        /// ����Ʒ��Id
        /// </summary>
        public int? ShopBrandId { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public Int32 OrderWeight { get; set; }
        /// <summary>
        /// ˵��
        /// </summary>
        public String Detail { get; set; }
        /// <summary>
        /// �ϼ����Id
        /// </summary>
        public Int32? PId { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public String Flag { get; set; }
        /// <summary>
        /// �ؼ���
        /// </summary>
        public String Keywords { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// ͼ�꣬����
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// ��ǩ
        /// </summary>
        public string Tags { get; set; }
        
    }
    /// <summary>
    /// ���·����Ĳ�����
    /// </summary>
    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public Int32 Id { get; set; }
    }
    /// <summary>
    /// ��õ�����¼�ķ�����
    /// </summary>
    public class GetSingleModel : ShopBrandCommodityCategory
    {
    }
}