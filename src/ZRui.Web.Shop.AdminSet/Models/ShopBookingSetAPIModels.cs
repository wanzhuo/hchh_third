using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBookingSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel : CommunityArgsModel
    {
        public int ShopId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopBooking
    {

    }

    public class SetStatusArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ��״̬
        /// </summary>
        public ShopBookingStatus Status { get; set; }
        /// <summary>
        /// ���ԭ��
        /// </summary>
        public string RefuseReason { get; set; }
    }

    public class SetIsUsedArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// �Ƿ��Ѿ�ʹ��
        /// </summary>
        public bool IsUsed { get; set; }
    }

    public class GetShopBrandsModel
    {
        public List<ShopBrandsItem> Items { get; set; }
    }
    public class ShopBrandsItem
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
    }

    public class GetShopsArgsModel : CommunityArgsModel
    {
        public int ShopBrandId { get; set; }
    }
    public class GetShopsModel
    {
        public List<ShopItem> Items { get; set; }
    }
    public class ShopItem
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
    }
}