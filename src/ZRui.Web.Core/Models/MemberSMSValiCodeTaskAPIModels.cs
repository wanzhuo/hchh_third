using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.MemberSMSValiCodeTaskAPIModels
{
    public class AddArgs
    {
        public string Phone { get; set; }
        public string TaskType { get; set; }
    }
}