using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopOrderSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("orderName")]
        public string OrderName { get; set; }
        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        public string SearchId { get; set; }
    }

    public class GetShopOrderViewArgsModel
    {
        public int orderid { get; set; }
        public string openid { get; set; }
    }

    public class SetTakeoutStatusArgsModel
    {
        public int? Id { get; set; }
        public Status? status { get; set; }
    }

    public class GetShopOrderViewResultModel : ShopOrder
    {
        public string ShopPartName { get; set; }
        public double OrderAmount { get; set; }
        public double PayAmount { get; set; }
        public string Address { get; set; }
        public string TakeWay { get; set; }
        public string Headimgurl { get; set; }

        public string NickName { get; set; }
        public string Name { get; set; }
        public DateTime PickupTime { get; set; }
        public List<ShopOrderItemInfo> ShopOrderItems { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public string PayWay { get; set; }
    }

    public class ShopOrderItemInfo
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double Amount { get; set; }
        public string SkuSummary { get; set; }
    }

    public class GetPagedListModel : GetListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }


    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopOrder
    {
        public string ShopPartTitle { get; set; }
        public string Address { get; set; }
        public string TakeOutPhone { get; set; }
        public string TakeOutName { get; set; }
        public Status? TakeOutStatus { get; set; }
    }


    public class GetOrderItemsArgsModel
    {
        public int OrderId { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public OrderTypeE OrderType { get; set; }
    }

    public class SetStatusArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ��״̬
        /// </summary>
        public ShopOrderStatus Status { get; set; }
    }


    #region �����б�����ӵĴ���
    /// <summary>
    /// GetPagedList����ʵ��
    /// </summary>
    public class GetPagedListResulrModel
    {
        public OrderTypeE OrderType { get; set; }
        public int OrderId { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ����������Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// �������û�Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// �绰
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// ʵ��֧�����
        /// </summary>
        public int? Payment { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// ֧��ʱ��
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string ShopPartTitle { get; set; }

        /// <summary>
        /// ״̬
        /// </summary>
        public ShopOrderStatus Status { get; set; }
        /// <summary>
        /// ����״̬����
        /// </summary>
        public string StatusStr { get; set; }




        /// <summary>
        /// ������ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ��ݵ绰
        /// </summary>
        public string TakeOutPhone { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string TakeOutName { get; set; }
        /// <summary>
        /// ����״̬
        /// </summary>
        public Status TakeOutStatus { get; set; }

        /// <summary>
        /// �µ���ʽ
        /// </summary>
        public TakeWay TakeWay { get; set; }

        /// <summary>
        /// ƴ��״̬
        /// </summary>
        public ConglomerationSetUpStatus ConglomerationSetUpStatus { get; set; }
    }


    /// <summary>
    /// ���ض����б�ʵ��
    /// </summary>
    public class ConglomerationOrderModel : ModelBase
    {
        /// <summary>
        /// �������
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// �������ͣ�1���ᣬ2��ݣ�
        /// </summary>
        public ConsignmentType Type { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string TypeStr { get; set; }


        /// <summary>
        /// �����
        /// </summary>
        public string PickupCode { get; set; }

        /// <summary>
        /// Ӧ�����
        /// </summary>
        public int Amount { get; set; }
        public decimal AmountM { get; set; }

        /// <summary>
        /// ʵ��֧�����
        /// </summary>
        public int? Payment { get; set; }
        public decimal PaymentM { get; set; }

        /// <summary>
        /// ֧��ʱ��
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// �˿�ʱ��
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime FinishTime { get; set; }




        /// <summary>
        /// ����ƴ�Ż
        /// </summary>
        public ConglomerationOrderActivityModel ConglomerationActivity { get; set; }


        /// <summary>
        /// ״̬
        /// </summary>
        public ShopOrderStatus Status { get; set; }

        /// <summary>
        /// ״̬���ַ�����
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// �����Ϣ
        /// </summary>
        public ConglomerationOrderConglomerationExpressModel ConglomerationExpress { get; set; }


        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? SuccessfulTime { get; set; }


        /// <summary>
        /// �����û���Ϣ
        /// </summary>
        public OrderShopMember OrderShopMember { get; set; }


        public int MemberId { get; set; }


        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public string PayWay { get; set; }
    }

    /// <summary>
    /// ���������ڵĻ��Ϣʵ��
    /// </summary>
    public class ConglomerationOrderActivityModel
    {
        /// <summary>
        /// �����
        /// </summary>

        public string ActivityName { get; set; }


        public string Deliverys { get; set; }
    }


    /// <summary>
    /// �����Ϣ
    /// </summary>
    public class ConglomerationOrderConglomerationExpressModel
    {
        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// �ֻ�����
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// �Ա�
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// ��ϸ
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        /// ��ݶ�����
        /// </summary>
        public string ExpressSingle { get; set; }

        ///// <summary>
        ///// ���״̬
        ///// </summary>
        //public Status Status { get; set; }

        /// <summary>
        /// ���ͷ�
        /// </summary>
        public int ActivityDeliveryFee { get; set; }
        public decimal ActivityDeliveryFeeM { get; set; }


    }



    /// <summary>
    /// ƴ�Żҵ��ʵ��
    /// </summary>
    public class ActivityModel : ModelBase
    {
        /// <summary>
        ///����ͼ
        /// </summary>

        public string CoverPortal { get; set; }

        /// <summary>
        /// �����
        /// </summary>

        public string ActivityName { get; set; }

        /// <summary>
        /// ����
        /// </summary>

        public string Intro { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// �����ʱ��
        /// </summary>

        public DateTime ActivityEndTime { get; set; }

        /// <summary>
        /// ���ʼʱ��
        /// </summary>

        public DateTime ActivityBeginTime { get; set; }

        /// <summary>
        /// �ع���
        /// </summary>
        public int BrowseNumber { get; set; }

        /// <summary>
        /// ���ͷ�
        /// </summary>
        public int ActivityDeliveryFee { get; set; }

        public decimal ActivityDeliveryFeeM { get { return ActivityDeliveryFee / 100.00M; } }

        /// <summary>
        /// ����������Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// �г��۸�
        /// </summary>
        public int MarketPrice { get; set; }
        /// <summary>
        /// ƴ�ŵ���ʱʣ����ӣ���λ�֣�
        /// </summary>
        public int ConglomerationCountdown { get; set; }

        public decimal MarketPriceM { get { return ConglomerationCountdown / 100.00M; } }


    }





    public class GetPagedListRequestModel : GetPagedListBaseModel
    {

        public OrderTypeE OrderType { get; set; }
        public string SearchId { get; set; } = "";
    }
    public enum OrderTypeE
    {
        ɨ���Ͷ��� = 1,
        �������� = 2,
        ������Ͷ��� = 3,
        ƴ�Ŷ��� = 4
    }
    #endregion



    public class GetOrderInfoResultModel
    {
        public SelfHelpInfo SelfHelp { get; set; }
        public object Items { get; set; }
        public OtherFee OtherFee { get; set; }
        public MoneyOffRule MoneyOffRule { get; set; }
        public TakeOutInfo TakeOutInfo { get; set; }
        public decimal? PayMent { get; set; }
        public string CreateTime { get; set; }
        public string PayTime { get; set; }
        public string Code { get; set; }
        public string ShopPartName { get; set; }
        public string Remark { get; set; }

        public OrderShopMember OrderShopMember { get; set; }



        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public string PayWay { get; set; }
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

    /// <summary>
    /// �����ڻ�Ա��Ϣ
    /// </summary>
    public class OrderShopMember
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// �Ա�
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// �ֻ�
        /// </summary>
        public string Phone { get; set; }

    }

}