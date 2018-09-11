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
        /// 编号
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 人均消费
        /// </summary>
        public string UsePerUser { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 地址附加信息
        /// </summary>
        public string AddressGuide { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 营业时间/开放时间
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// 一些说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 省市区最小一级的编码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 省市区（例如：广东省东莞市万江区）
        /// </summary>
        public string AreaText { get; set; }
        /// <summary>
        /// 是否自助点餐
        /// </summary>
        public bool IsSelfHelp { get; set; }

        /// <summary>
        /// 广告图（轮播）
        /// </summary>
        public string Banners { get; set; }
    }

    public class BannerModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Link { get; set; }


        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 排序
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
        /// 配送方式 商家配送 = 0,达达配送 = 1
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