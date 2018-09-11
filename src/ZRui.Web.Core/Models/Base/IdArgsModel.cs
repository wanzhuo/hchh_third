using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web
{
    /// <summary>
    /// 只有Id的参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdArgsModel<T>: CommunityArgsModel
    {
        /// <summary>
        /// 指定类型的编号
        /// </summary>
        public T Id { get; set; }
    }

    /// <summary>
    /// 只有Id的参数
    /// </summary>
    public class IdArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
    }

    public class SetPomodoroTimerCountArgsModel : IdArgsModel
    {
        public int Count { get; set; }
    }
}