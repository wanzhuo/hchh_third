using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class ModelBase
    {
        public int Id { get; set; }
        public bool IsDel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        { get; set; }

    }
}
