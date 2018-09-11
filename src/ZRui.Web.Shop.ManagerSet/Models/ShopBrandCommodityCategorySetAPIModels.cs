using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommodityCategorySetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel
    {
        /// <summary>
        /// 商铺品牌Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// 获取列表返回值类
    /// </summary>
    public class GetListModel
    {
        /// <summary>
        /// 列表
        /// </summary>
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// 获取分页列表参数类
    /// </summary>
    public class GetPagedListArgsModel : GetListArgsModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 排序的字段名字
        /// </summary>
        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        /// <summary>
        /// 升序或者降序
        /// </summary>
        [JsonProperty("orderType")]
        public string OrderType { get; set; }
    }
    /// <summary>
    /// 获取分页列表的返回值类
    /// </summary>
    public class GetPagedListModel : GetListModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 总纪录数
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopBrandCommodityCategory
    {
        /// <summary>
        /// 上级类别的名字
        /// </summary>
        public string ParentName { get; set; }
    }
    /// <summary>
    /// 获取类别树的参数类
    /// </summary>
    public class GetTreeArgsModel
    {
        /// <summary>
        /// 商铺品牌Id
        /// </summary>
        public int? ShopBrandId { get; set; }
    }
    /// <summary>
    /// 获取类别树的返回类
    /// </summary>
    public class GetTreeModel
    {
        /// <summary>
        /// 树
        /// </summary>
        [JsonProperty("tree")]
        public List<TreeNode> Tree { get; set; }
        /// <summary>
        /// 所有节点
        /// </summary>
        [JsonIgnore]
        public List<TreeNode> AllNodes { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">列表</param>
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
    /// 数节点
    /// </summary>
    public class TreeNode : ShopBrandCommodityCategory
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return base.Name;
            }
        }
        /// <summary>
        /// 节点列表
        /// </summary>
        public List<TreeNode> Nodes
        {
            get
            {
                return root.AllNodes.Where(m => m.PId == Id).OrderByDescending(m => m.OrderWeight).ToList();
            }
        }

        /// <summary>
        /// 上级类别名字
        /// </summary>
        public string ParentName { get; internal set; }

        GetTreeModel root;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="root">根节点</param>
        public TreeNode(GetTreeModel root)
        {
            this.root = root;
        }
    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel
    {
        /// <summary>
        /// 商铺品牌Id
        /// </summary>
        public int? ShopBrandId { get; set; }
        /// <summary>
        /// 商品类型名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 排序权重
        /// </summary>
        public Int32 OrderWeight { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public String Detail { get; set; }
        /// <summary>
        /// 上级类别Id
        /// </summary>
        public Int32? PId { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public String Flag { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public String Keywords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 图标，封面
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }
        
    }
    /// <summary>
    /// 更新方法的参数类
    /// </summary>
    public class UpdateArgsModel : AddArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public Int32 Id { get; set; }
    }
    /// <summary>
    /// 获得单条纪录的返回类
    /// </summary>
    public class GetSingleModel : ShopBrandCommodityCategory
    {
    }
}