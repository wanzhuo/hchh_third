using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web
{
    /// <summary>
    /// ֻ��Id�Ĳ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdArgsModel<T>: CommunityArgsModel
    {
        /// <summary>
        /// ָ�����͵ı��
        /// </summary>
        public T Id { get; set; }
    }

    /// <summary>
    /// ֻ��Id�Ĳ���
    /// </summary>
    public class IdArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
    }

    public class SetPomodoroTimerCountArgsModel : IdArgsModel
    {
        public int Count { get; set; }
    }
}