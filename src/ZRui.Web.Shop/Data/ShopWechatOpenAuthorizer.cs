using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺关联的第三方授权者
    /// </summary>
    public class ShopWechatOpenAuthorizer : EntityBase
    {
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺品牌的Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联的第三方授权者
        /// </summary>
        [ForeignKey("WechatOpenAuthorizerId")]
        public WechatOpenAuthorizer WechatOpenAuthorizer { get; set; }
        /// <summary>
        /// 关联的第三方授权者的Id
        /// </summary>
        public int WechatOpenAuthorizerId { get; set; }

        public string ReleaseTemplateUserVersion { get; set; }
        /// <summary>
        /// 当前的模板Id
        /// </summary>
        public int? CurrentTemplateId { get; set; }
        /// <summary>
        /// 当前模板的用户版本
        /// </summary>
        public string CurrentTemplateUserVersion { get; set; }
        /// <summary>
        /// 当前模板的用户描述
        /// </summary>
        public string CurrentTemplateUserDesc { get; set; }
        /// <summary>
        /// 当前模板的extjson
        /// </summary>
        public string CurrentTemplateExtJson { get; set; }
        /// <summary>
        /// 当前的审核Id
        /// </summary>
        public int? CurrentAuditId { get; set; }
        /// <summary>
        /// 审核状态，其中0为审核成功，1为审核失败，2为审核中,如果为空，则表示未提交审核
        /// </summary>
        public int? CurrentAuditStatus { get; set; }
        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string CurrentAuditFailReason { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsRelease { get; set; }
    }



}
