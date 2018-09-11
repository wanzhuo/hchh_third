using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopCallingQueueSetAPIModels
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
    public class RowItem : ShopCallingQueue
    {

    }

    public class SetStatusArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 新状态
        /// </summary>
        public ShopCallingQueueStatus Status { get; set; }
        /// <summary>
        /// 解决原因
        /// </summary>
        public string RefuseReason { get; set; }
    }

    public class SetIsUsedArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 是否已经使用
        /// </summary>
        public bool IsUsed { get; set; }
    }

    public class SetQueueIndexArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 新的排队位置
        /// </summary>
        public int QueueIndex { get; set; }
    }
}