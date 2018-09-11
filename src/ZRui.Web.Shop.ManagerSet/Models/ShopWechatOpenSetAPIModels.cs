using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopWechatOpenSetAPIModels
{
    /// <summary>
    /// ShopId��ͨ�ò�����
    /// </summary>
    public class ShopIdArgsModel
    {
        public int? ShopId { get; set; }
    }
    /// <summary>
    /// ��ȡ������Ŀ
    /// </summary>
    public class GetCacheItemArgsModel : ShopIdArgsModel
    {
        public string ShortKey { get; set; }
    }
    /// <summary>
    /// ��/���΢���û�ΪС���������� �Ĳ�����
    /// </summary>
    public class TesterArgsModel : ShopIdArgsModel
    {
        public string WechatId { get; set; }
    }
    /// <summary>
    /// �޸Ŀ���Domain�Ĳ�����
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
        /// ���캯��
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
    /// ʹ��ָ����ģ��Ĳ�����
    /// </summary>
    public class UseTemplateArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// ģ��Id
        /// </summary>
        public int TemplateId { get; set; }
        /// <summary>
        ///  ��չJson������ʹ��"{}"����
        /// </summary>
        public string ExtJson { get; set; }
        /// <summary>
        /// �û��İ汾
        /// </summary>
        public string UserVersion { get; set; }
        /// <summary>
        /// �û�������
        /// </summary>
        public string UserDesc { get; set; }
    }

    public class GetOAuthQrcodeArgsModel : ShopIdArgsModel
    {

    }

    /// <summary>
    /// ���������ύ�Ĵ�����ύ��˲�����
    /// </summary>
    public class SubmitAuditArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// �ύ������һ���б�������д1�������д5��
        /// </summary>
        public List<Senparc.Weixin.Open.WxaAPIs.SubmitAuditPageInfo> Items { get; set; }
        public string user_version { get; set; }
    }
    /// <summary>
    /// ��ѯĳ��ָ���汾�����״̬������
    /// </summary>
    public class GetAuditStatusArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// �ύ���ʱ��õ����id
        /// </summary>
        public int AuditId { get; set; }
    }

    /// <summary>
    /// �޸�С�������ϴ���Ŀɼ�״̬������
    /// </summary>
    public class ChangeVisitStatusArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// ���ÿɷ���״̬��������Ĭ�Ͽɷ��ʣ�closeΪ���ɼ���openΪ�ɼ�
        /// </summary>
        public Senparc.Weixin.Open.ChangVisitStatusAction Action { get; set; }
    }

    /// <summary>
    /// ����С����ɨ��ͨ���Ӷ�ά���С�������� ֮ ���ӻ��޸Ķ�ά�����
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