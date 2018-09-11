using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopPartSetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class ShopIdArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListArgsModel: ShopIdArgsModel
    {
    }

    public class GetListModel
    {
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem : ShopPart
    {

    }

    public class GetPagedListModel : GetListModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class AddArgsModel
    {
        public int? ShopId { get; set; }
        public String Title { get; set; }
        public String Detail { get; set; }
    }

    public class UpdateArgsModel
    {
        public Int32 Id { get; set; }
        public String Title { get; set; }
        public String Detail { get; set; }
    }

    public class InitQRCodeJumpArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 测试范围：1为开发版（配置只对开发者生效）2为体验版（配置对管理员、体验者生效）3为线上版本（配置对管理员、开发者和体验者生效）
        /// </summary>
        public string OpenVersion { get; set; }
        public string Path { get; set; }
        public List<string> DebugUrl { get; set; }
    }
}