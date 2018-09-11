using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBrandCommodityCategorySetAPIModels
{
    /// <summary>
    /// 获取列表参数类
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
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        [JsonProperty("orderType")]
        public string OrderType { get; set; }
    }

    public class GetPagedListModel : GetListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem
    {
        public virtual Int32 Id { get; set; }
        public virtual String Name { get; set; }
        public virtual float OrderWeight { get; set; }
        public virtual String Detail { get; set; }
        [JsonProperty("pid")]
        public virtual Int32? PId { get; set; }
        public virtual String Flag { get; set; }
        public virtual bool IsDel { get; set; }
        public virtual String Keywords { get; set; }
        public virtual String Description { get; set; }
        public string ParentName { get;  set; }
        public string Ico { get;  set; }
        public string Tags { get;  set; }
    }

    public class GetTreeArgsModel : CommunityArgsModel
    {
        public int? ShopBrandId { get; set; }
    }

    public class GetTreeModel
    {
        [JsonProperty("tree")]
        public List<TreeNode> Tree { get; set; }
        [JsonIgnore]
        public List<TreeNode> AllNodes { get; set; }
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

            Tree = AllNodes.Where(m => !m.PId.HasValue || m.PId == 0).OrderByDescending(m=>m.OrderWeight).ToList();
        }
    }

    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title
        {
            get
            {
                return Name;
            }
        }
        public List<TreeNode> Nodes
        {
            get
            {
                return root.AllNodes.Where(m => m.PId == Id).OrderByDescending(m => m.OrderWeight).ToList();
            }
        }
        public float OrderWeight { get; set; }
        public string Detail { get; set; }
        public int? PId { get; set; }
        public string Flag { get; set; }
        public virtual String Keywords { get; set; }
        public virtual String Description { get; set; }
        public string ParentName { get; internal set; }
        public string Ico { get;  set; }
        public string Tags { get; set; }

        GetTreeModel root;
        public TreeNode(GetTreeModel root)
        {
            this.root = root;
        }
    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        public int ShopBrandId { get; set; }
        public String Name { get; set; }
        public Int32 OrderWeight { get; set; }
        public String Detail { get; set; }
        public Int32? PId { get; set; }
        public String Flag { get; set; }
        public String Keywords { get; set; }
        public String Description { get; set; }
        public string Ico { get;  set; }
        public string Tags { get;  set; }
    }

    public class UpdateArgsModel : CommunityArgsModel
    {
        public Int32 Id { get; set; }
        public Int32? PId { get; set; }
        public String Name { get; set; }
        public Int32 OrderWeight { get; set; }
        public String Detail { get; set; }
        public String Flag { get; set; }
        public String Keywords { get; set; }
        public String Description { get; set; }
        public string Ico { get;  set; }
        public string Tags { get;  set; }
    }

    public class GetSingleModel
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public float OrderWeight { get; set; }
        public String Detail { get; set; }
        public Int32? PId { get; set; }
        public String Flag { get; set; }
        public String Keywords { get; set; }
        public String Description { get; set; }
        public string Ico { get;  set; }
        public string Tags { get;  set; }
    }

    public class GetShopBrandsModel
    {
        public List<GetShopBrandsItem> Items { get; set; }
    }
    public class GetShopBrandsItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}