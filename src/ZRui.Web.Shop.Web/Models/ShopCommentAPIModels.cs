using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopCommentAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel
    {
        public int ShopId { get; set; }
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

    public class AddArgsModel
    {
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public decimal Grade { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 关联的店铺
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 相关图片
        /// </summary>
        public IList<int> PicIds { get; set; }
    }

    public class SetCommentArgsModel
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public decimal Grade { get; set; }
    }


    public class DelCommentArgsModel
    {
        public int Id { get; set; }
    }


    public class PictureArgModel
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopComment
    {
        public IList<int> PicIds { get; set; }
    }
}