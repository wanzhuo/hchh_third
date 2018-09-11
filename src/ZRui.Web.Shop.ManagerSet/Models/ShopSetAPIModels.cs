using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopSetAPIModels
{
    public class GetShopsArgsModel
    {
    }

    public class GetShopItem
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string ShopFlag { get; set; }
    }

    public class UpdateArgsModel
    {
        /// <summary>
        /// ���
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// �˾�����
        /// </summary>
        public string UsePerUser { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ��ַ������Ϣ
        /// </summary>
        public string AddressGuide { get; set; }
        /// <summary>
        /// γ��
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// Ӫҵʱ��/����ʱ��
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// һЩ˵��
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// ʡ������Сһ���ı���
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// ʡ���������磺�㶫ʡ��ݸ��������
        /// </summary>
        public string AreaText { get; set; }
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsSelfHelp { get; set; }

        /// <summary>
        /// ���ͼ���ֲ���
        /// </summary>
        public string Banners { get; set; }
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

    public class GetSingleModel : Shop
    {

    }

    public class SetIsOpenArgsModels
    {
        public int? ShopId { get; set; }
        public bool IsOpen { get; set; }
    }

    public class GetShopTakeOutInfoArgsModel
    {
        public int? ShopId { get; set; }

        public TakeDistributionType TakeDistributionType { get; set; }
    }

    public class GetShopTakeOutInfoModel
    {
        public int ShopId { get; set; }
        public int Scope { get; set; }
        public int MinAmount { get; set; }
        public int BoxFee { get; set; }
        public bool IsOpen { get; set; }
        public int DeliveryFee { get; set; }
        public bool AutoTakeOrdre { get; set; }
        public bool AutoPrint { get; set; }

    }

    public class SetShopTakeOutInfoArgsModel
    {
        public int ShopId { get; set; }
        public int scope { get; set; }
        public double MinAmount { get; set; }
        public double BoxFee { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double DeliveryFee { get; set; }
        public bool AutoTakeOrdre { get; set; }
        public bool AutoPrint { get; set; }
        /// <summary>
        /// ���ͷ�ʽ �̼����� = 0,������� = 1
        /// </summary>
        public TakeDistributionType TakeDistributionType { get; set; }

    }

    public class SetUsedShopTakeOutArgsModel
    {
        public int ShopId { get; set; }
        public bool IsUsed { get; set; }
    }


    public class SetOpenShopTakeOutArgsModel
    {
        public int ShopId { get; set; }
        public bool IsOpen { get; set; }
    }


    public class ShopIdArgModel
    {
        public int? ShopId { get; set; }
    }

    public class SetShopSelfHelpIsSelfHelpArgModel : ShopIdArgModel
    {
        public bool IsSelfHelp { get; set; }
    }

    public class SetShopSelfHelpHasBoxFeeArgModel : ShopIdArgModel
    {
        public bool HasBoxFee { get; set; }
    }

    public class SetShopSelfHelpBoxFeeArgModel : ShopIdArgModel
    {
        public int BoxFee { get; set; }
    }


    public class SetShopSelfHelpInfoArgsModel
    {
        public int? ShopId { get; set; }
        public int BoxFee { get; set; }
        public bool HasTakeOut { get; set; }
    }
}