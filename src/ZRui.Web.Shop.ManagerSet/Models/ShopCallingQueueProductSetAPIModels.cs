using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopCallingQueueProductSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopCallingQueueProduct
    {

    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ˵��
        /// </summary>
        public string Detail { get; set; }
    }

    public class UpdateArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ˵��
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        public ShopCallingQueueProductStatus Status { get; set; }
    }

    public class SetShopOpenStatusArgsModel : IdArgsModel
    {
        public bool IsOpen { get; set; }
    }
}