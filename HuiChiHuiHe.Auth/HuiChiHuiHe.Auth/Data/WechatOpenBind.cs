using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HuiChiHuiHe.Auth
{
    public class WechatOpenBind
    {
        [Key]
        public string AppId { get; set; }
        public DateTime AddTime { get; set; }
        public string OpenAppId { get; set; }
        public bool IsBind { get; set; }
        public string BindDesc { get; set; }


    }
}
