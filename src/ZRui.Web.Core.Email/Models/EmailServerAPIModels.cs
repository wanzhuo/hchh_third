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
        /// ����
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// �Ƿ���������ύ
        /// </summary>
        public string Content { get; set; }
    }
}