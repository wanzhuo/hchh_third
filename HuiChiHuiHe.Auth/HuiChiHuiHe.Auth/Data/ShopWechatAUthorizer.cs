using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HuiChiHuiHe.Auth
{
    public class ShopWechatOpenAuthorizer 
    {   

        [Key]
        public int Id { get; set; }

        public int ShopId { get; set; }
 
        public int WechatOpenAuthorizerId { get; set; }
        public string ReleaseTemplateUserVersion { get; set; }
        public int? CurrentTemplateId { get; set; }
        public string CurrentTemplateUserVersion { get; set; }
        public string CurrentTemplateUserDesc { get; set; }
        public string CurrentTemplateExtJson { get; set; }
        public int? CurrentAuditId { get; set; }
        public int? CurrentAuditStatus { get; set; }
        public string CurrentAuditFailReason { get; set; }
        public bool IsRelease { get; set; }
        public bool IsDel { get; set; }
    }
}
