using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopStatisticsAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class ShopIdArgsModel
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public int? ShopId { get; set; }
    }
    /// <summary>
    /// ��ȡ�ܼƵķ���ֵ��
    /// </summary>
    public class GetTotalModel
    {
        /// <summary>
        /// ��������
        /// </summary>
        public int OrderCount { get; set; }
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public int CommodityCount { get; set; }
        /// <summary>
        /// �û�����
        /// </summary>
        public int MemberCount { get; set; }
    }


    public class GetCallingQueueModel
    {
        /// <summary>
        /// �ɹ��к�����������
        /// </summary>
        public int CallingSuccessTotal { get; set; }
        /// <summary>
        /// �û�����
        /// </summary>
        public int MemberCount { get; set; }
    }

    public class GetBookingModel
    {
        /// <summary>
        /// ����Ԥ������
        /// </summary>
        public int BookTotal { get; set; }
        /// <summary>
        /// Ԥ�����Ͳ�����
        /// </summary>
        public int BookAndUse { get; set; }
        /// <summary>
        /// �û�����
        /// </summary>
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// ��ȡ��������ͳ�ƵĲ�����
    /// </summary>
    public class GetOrderCountForDayArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// ��ʼ����
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
    /// <summary>
    /// ��ȡ��������ͳ�Ƶķ���ֵ��
    /// </summary>
    public class GetOrderCountForDayModel
    {
        /// <summary>
        /// �б�
        /// </summary>
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// ��
    /// </summary>
    public class RowItem
    {
        /// <summary>
        /// ����
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public int Count { get; set; }
    }


}