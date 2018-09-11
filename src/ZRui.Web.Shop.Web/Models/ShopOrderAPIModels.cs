using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;
using ZRui.Web.Models;

namespace ZRui.Web.ShopOrderAPIModels
{
    public class GetListForMeArgsModel : GetPagedListBaseModel
    {
        public ShopBookingStatus? Status { get; set; }
        public bool? IsUsed { get; set; }
        public string ShopFlag { get; set; }

        public OrderTypeF orderType { get; set; }

    }
    public enum OrderTypeE
    {
        ɨ���Ͷ��� = 1,
        �������� = 2,
        ������Ͷ��� = 3,
        ƴ�Ŷ��� = 4
    }

      public enum OrderTypeF
    {
        ��Ͷ��� = 5,
        �������� = 2,
    }

    public class GetListForMeModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }
    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopOrder
    {
        public string ShopName { get; set; }
        public string ShopPartTitle { get; set; }
        public string OrderStatus { get; set; }
        public string Cover { get; set; }
        public bool IsPay
        {
            get
            {
                return PayTime.HasValue;
            }
        }
    }

    public class AddArgsModel
    {
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool? IsSelfHelp { get; set; }
        public bool? IsSelfHelpTakeOut { get; set; }
        public bool? IsTakeOut { get; set; }
        /// <summary>
        /// �Ƿ����֧��
        /// </summary>
        public bool IsBalance { get; set; }
        public string ShopPartFlag { get; set; }
        /// <summary>
        /// �µ���ʽ 
        /// </summary>
        public TakeWay? TakeWay { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? PickupTime { get; set; }
        /// <summary>
        /// �����ĵ���
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// �ײͼ۸�
        /// </summary>
        public int? ComboPrice { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// ������б�
        /// </summary>
        public List<AddItem> Items { get; set; }
        /// <summary>
        /// ���ͷ�ʽ
        /// </summary>
       // public TakeDistributionType TakeDistributionType { get; set; }


    }

    public class ThirdAddArgsModel
    {
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool? IsSelfHelp { get; set; }
        public bool? IsSelfHelpTakeOut { get; set; }
        public bool? IsTakeOut { get; set; }
        public string ShopPartFlag { get; set; }
        /// <summary>
        /// �µ���ʽ 
        /// </summary>
        public TakeWay? TakeWay { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? PickupTime { get; set; }
        /// <summary>
        /// �����ĵ���
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// �ײͼ۸�
        /// </summary>
        public int? ComboPrice { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// ������б�
        /// </summary>
        public List<AddItem> Items { get; set; }
        /// <summary>
        /// ���ͷ�ʽ
        /// </summary>
       // public TakeDistributionType TakeDistributionType { get; set; }



    }

    public class AddItem
    {
        public int? comboId { get; set; }
        public string SkuFlag { get; set; }
        public int Count { get; set; }
    }

    public class GetSingleModel
    {
        public string Takeway { get; set; }
        public string Address { get; set; }
        public string ExpectTitle { get; set; }
        public string OrderTitle { get; set; }
        public string SelfHelpNumber { get; set; }
        public string ExpectTime { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string OrderStatus { get; set; }
        public string Title { get; set; }
        public ShopOrder Order { get; set; }
        public ShopOrderOtherFee OtherFee { get; set; }
        public List<ShopOrderItem> Items { get; set; }
    }

    public class SetRemarkArgsModel : IdArgsModel
    {
        public string Remark { get; set; }
    }


    public class GetOrderInfoArgsModel
    {
        public int? shopOrderId { get; set; }

    }

    public class GetOrderInfoResultModel
    {
        public SelfHelpInfo SelfHelp { get; set; }
        public List<CommodityInfo> Items { get; set; }
        public OtherFee OtherFee { get; set; }
        public MoneyOffRule MoneyOffRule { get; set; }
        public TakeOutInfo TakeOutInfo { get; set; }
        public string OrderStatus { get; set; }
        public ShopOrderPayStatus PayStatus { get; set; }
        public decimal? PayMent { get; set; }
        public string CreateTime { get; set; }
        public string PayTime { get; set; }
        public string Code { get; set; }
        public string ShopPartName { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// ���������Ͷ�����Ϣ
        /// </summary>
        public CThirdInfoQuery cThirdInfoQuery { get; set; }
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool IsTakeOut { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public string PayWay { get; set; }

    }

    public class CommodityInfo
    {
        public string Cover { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PrimePrice { get; set; }
        public bool VipPrice { get; set; }
        public int Count { get; set; }
    }

    public class SelfHelpInfo
    {
        public string SelfHelpCode { get; set; }
        public string TakeWay { get; set; }
    }

    public class TakeOutInfo
    {
        public string PickTile { get; set; }
        public string PickUpTime { get; set; }
        public string DiningWay { get; set; }
        public string Address { get; set; }
        public string Person { get; set; }
        public string Phone { get; set; }
    }

    public class MoneyOffRule
    {
        public decimal FullAmount { get; set; }
        public decimal Discount { get; set; }
    }

    public class OtherFee
    {
        public decimal BoxFee { get; set; }
        public decimal DeliveryFee { get; set; }
    }
}