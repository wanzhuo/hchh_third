using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.EmailServerAPIModels
{
    
    public class SendArgsModel
    {
        public string To { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 是否可以匿名提交
        /// </summary>
        public string Content { get; set; }
    }
}