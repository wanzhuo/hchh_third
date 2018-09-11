using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel
    {
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// γ��
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double? Longitude { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }
    public class BannerModel
    {
        public int Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ·��
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Link { get; set; }


        /// <summary>
        /// �Ƿ���ʾ
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int Sorting { get; set; }



    }

    public class GetPagedListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : Shop
    {

    }

    public class GetSingleArgs
    {
        public string Flag { get; set; }
    }

    public class GetSingleModel : Shop
    {
        public int DiningWay { get; set; }
        public bool IsTopUpDiscount { get; set; }
        public bool HasSelfHelpTakeout { get; set; }
        public int SelfHelpBoxFee { get; set; }
        public string CurrentVersion { get; set; }
    }


    public class GetInfoModel
    {
        public GetSingleModel ShopInfo { get; set; }
        public TakeOutInfo TakeOutInfo { get; set; }
        public ShopMember ShopMember { get; set; }

        public List<BannerModel> BannerModel { get; set; }
    }

    public class TakeOutInfo
    {
        public bool IsUseTakeOut { get; set; }
        public bool IsOpen { get; set; }
        public decimal MinAmount { get; set; }
        public decimal BoxFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}