using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺
    /// </summary>
    public class Shop : EntityBase
    {
        /// <summary>
        /// 关联的商铺品牌
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// 关联的商铺品牌的Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 人均消费
        /// </summary>
        public string UsePerUser { get; set; }
        /// <summary>
        /// 省市区最小一级的编码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 省市区（例如：广东省东莞市万江区）
        /// </summary>
        public string AreaText { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 地址指引
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
        /// GeoHash
        /// </summary>
        public string GeoHash { get; set; }
        /// <summary>
        /// 自助点餐或者扫码点餐
        /// </summary>
        public bool IsSelfHelp { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 营业时间/开放时间
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// 评分值
        /// </summary>
        public int ScoreValue { get; set; }
        /// <summary>
        /// 一些说明
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
        /// <summary>
        /// 设置是否显示
        /// </summary>
        public bool IsShowApplets { get; set; }
        /// <summary>
        /// 广告图（轮播）
        /// </summary>
        public string Banners { get; set; }
        /// <summary>
        /// 店铺手机
        /// </summary>
        public string Phone { get; set; }
        public ShopBusinessType BusinessType { get; set; }
    }


    public enum ShopBusinessType
    {
        餐饮 = 0,
        批发 = 1
    }
}
