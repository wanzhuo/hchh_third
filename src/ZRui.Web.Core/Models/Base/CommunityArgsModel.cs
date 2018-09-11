using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web
{
    /// <summary>
    /// 群组接口调用参数通用类
    /// </summary>
    public interface ICommunityArgsModel
    {
        /// <summary>
        /// App标识
        /// </summary>
        string AppFlag { get; set; }
        /// <summary>
        /// 群组标识
        /// </summary>
        string CommunityFlag { get; set; }
    }

    /// <summary>
    /// 群组接口调用参数通用类
    /// </summary>
    public class CommunityArgsModel: ICommunityArgsModel
    {
        /// <summary>
        /// App标识
        /// </summary>
        public string AppFlag { get; set; }
        /// <summary>
        /// 群组标识
        /// </summary>
        public string CommunityFlag { get; set; }
    }
}
