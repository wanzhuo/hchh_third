using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class BannerConfiguration : EntityBase<int>
    {
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



    }
}
