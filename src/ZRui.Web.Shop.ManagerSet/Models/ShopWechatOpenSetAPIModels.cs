using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopWechatOpenSetAPIModels
{
    /// <summary>
    /// ShopId的通用参数类
    /// </summary>
    public class ShopIdArgsModel
    {
        public int? ShopId { get; set; }
    }
    /// <summary>
    /// 获取缓存项目
    /// </summary>
    public class GetCacheItemArgsModel : ShopIdArgsModel
    {
        public string ShortKey { get; set; }
    }
    /// <summary>
    /// 绑定/解绑微信用户为小程序体验者 的参数类
    /// </summary>
    public class TesterArgsModel : ShopIdArgsModel
    {
        public string WechatId { get; set; }
    }
    /// <summary>
    /// 修改可用Domain的参数类
    /// </summary>
    public class ModifyDomainArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// RequestDomains
        /// </summary>
        public List<string> RequestDomains { get; set; }
        /// <summary>
        /// WsRequestDomains
        /// </summary>
        public List<string> WsRequestDomains { get; set; }
        /// <summary>
        /// UploadDomains
        /// </summary>
        public List<string> UploadDomains { get; set; }
        /// <summary>
        /// DownloadDomains
        /// </summary>
        public List<string> DownloadDomains { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModifyDomainArgsModel()
        {
            RequestDomains = new List<string>();
            WsRequestDomains = new List<string>();
            UploadDomains = new List<string>();
            DownloadDomains = new List<string>();
        }
    }
    /// <summary>
    /// 使用指定的模板的参数类
    /// </summary>
    public class UseTemplateArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        public int TemplateId { get; set; }
        /// <summary>
        ///  扩展Json，必须使用"{}"包裹
        /// </summary>
        public string ExtJson { get; set; }
        /// <summary>
        /// 用户的版本
        /// </summary>
        public string UserVersion { get; set; }
        /// <summary>
        /// 用户的描述
        /// </summary>
        public string UserDesc { get; set; }
    }

    public class GetOAuthQrcodeArgsModel : ShopIdArgsModel
    {

    }

    /// <summary>
    /// 将第三方提交的代码包提交审核参数类
    /// </summary>
    public class SubmitAuditArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 提交审核项的一个列表（至少填写1项，至多填写5项
        /// </summary>
        public List<Senparc.Weixin.Open.WxaAPIs.SubmitAuditPageInfo> Items { get; set; }
        public string user_version { get; set; }
    }
    /// <summary>
    /// 查询某个指定版本的审核状态参数类
    /// </summary>
    public class GetAuditStatusArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 提交审核时获得的审核id
        /// </summary>
        public int AuditId { get; set; }
    }

    /// <summary>
    /// 修改小程序线上代码的可见状态参数类
    /// </summary>
    public class ChangeVisitStatusArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// 设置可访问状态，发布后默认可访问，close为不可见，open为可见
        /// </summary>
        public Senparc.Weixin.Open.ChangVisitStatusAction Action { get; set; }
    }

    /// <summary>
    /// 设置小程序“扫普通链接二维码打开小程序”能力 之 增加或修改二维码规则
    /// </summary>
    public class QRCodeJumpAddArgsModel : ShopIdArgsModel
    {
        public string Prefix { get; set; }
        public string PermitSubRule { get; set; }
        public string Path { get; set; }
        public string OpenVersion { get; set; }
        public List<string> DebugUrl { get; set; }
        public bool IsEdit { get; set; }
    }

    public class QRCodeJumpPrefixArgsModel : ShopIdArgsModel
    {
        public string Prefix { get; set; }
    }

    public class SetShopOAuthIsUsedArgsModel
    {
        public int ID { get; set; }
        public bool IsUsed { get; set; }
    }
}