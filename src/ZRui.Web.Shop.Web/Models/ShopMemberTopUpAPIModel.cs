using System;
using ZRui.Web.BLL.ServerDto;

namespace ZRui.Web.ShopMemberTopUpAPIModel
{



    public class GetTopUpModel : ShopIdModel
    {

        public int Id { get; set; }

        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// ���ʱ��Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// �̶���ֵ���
        /// </summary>
        public decimal FixationTopUpAmountM { get; set; }

        /// <summary>
        /// ���ͽ��
        /// </summary>
        public decimal PresentedAmountM { get; set; }
    }


    public class GetCustomTopUpModel : ShopIdModel
    {

        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// ���ʱ��Ip
        /// </summary>
        public string AddIp { get; set; }



        /// <summary>
        /// �����
        /// </summary>
        public decimal StartAmountM { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public decimal MeetAmountM { get; set; }


        /// <summary>
        /// �������ͣ��ٷֱȣ�
        /// </summary>
        public double Additional { get; set; }



        /// <summary>
        /// �Ƿ������Զ����ֵ����
        /// </summary>
        public bool IsShowCustomTopUpSet { get; set; }
    }
}