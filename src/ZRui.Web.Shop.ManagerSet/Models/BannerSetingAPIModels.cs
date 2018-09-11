using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class UpdateArgsModel : BannerSetingAPIModels
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
    }
    public class BannerSetingAPIModels
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sorting { get; set; }

        public bool IsDel { get; set; }
    }

}
