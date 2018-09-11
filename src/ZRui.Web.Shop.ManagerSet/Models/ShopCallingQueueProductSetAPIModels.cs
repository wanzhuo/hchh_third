using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopCallingQueueProductSetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopCallingQueueProduct
    {

    }
    /// <summary>
    /// 添加参数类
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 商铺Id
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 餐桌规格
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Detail { get; set; }
    }

    public class UpdateArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 餐桌规格
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ShopCallingQueueProductStatus Status { get; set; }
    }

    public class SetShopOpenStatusArgsModel : IdArgsModel
    {
        public bool IsOpen { get; set; }
    }
}