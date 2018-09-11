using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.MemberLoginForJwtAPIModels
{
    public class LoginBySmsArgsModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }
}