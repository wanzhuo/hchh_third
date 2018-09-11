using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiChiHuiHe.WinService
{
    /// <summary>
    /// 服务配置
    /// </summary>
    public class TaskConfig
    {
        public string Id { get; set; }

        /// <summary>
        /// 服务名字
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 执行时间间隔（秒）
        /// </summary>

        public int ServiceInternal { get; set; }

        /// <summary>
        /// 执行时间段（用英文逗号隔开）
        /// </summary>

        public string TimeBucket { get; set; }

        /// <summary>
        /// 备注
        /// </summary>

        public string Remark { get; set; }

        public string Api { get; set; }

        public int Status { get; set; }

    }



}
