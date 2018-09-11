using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopBookingAPIModels
{
    public class GetListForMeArgsModel
    {
        public ShopBookingStatus? Status { get; set; }
        public bool? IsUsed { get; set; }
        public string ShopFlag { get; set; }
    }

    public class GetListForMeModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopBooking
    {
        public string ShopName { get; set; }
    }

    public class AddArgsModel
    {
        /// <summary>
        /// Ԥ���ĵ���
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Users { get; set; }
        /// <summary>
        /// ��ϵ���ǳ�
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// ��ϵ�˵绰
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// �Ͳ�ʱ��
        /// </summary>
        public DateTime DinnerTime { get; set; }
    }

    public class SetRemarkArgsModel : IdArgsModel
    {
        public string Remark { get; set; }
    }

}