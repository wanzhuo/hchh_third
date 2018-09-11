using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌的商品
    /// </summary>
    public class ShopBrandCommodity : EntityBase
    {
        /// <summary>
        /// 关联的类别
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual ShopBrandCommodityCategory Category { get; set; }
        /// <summary>
        /// 关联的类别Id
        /// </summary>
        public virtual int CategoryId { get; set; }
        /// <summary>
        /// 关联的商铺品牌
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// 关联的商铺品牌Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 月销量
        /// </summary>
        public int SalesForMonth { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int Upvote { get; set; }
        /// <summary>
        /// 就餐方式    弃用
        /// </summary>
        public DiningWay DiningWay { get; set; }
        /// <summary>
        /// 是否扫码点餐
        /// </summary>
        public bool IsScanCode { get; set; }
        /// <summary>
        /// 是否启用会员价
        /// </summary>
        public bool UseMemberPrice { get; set; }
        /// <summary>
        /// 是否外卖
        /// </summary>
        public bool IsTakeout { get; set; }
        /// <summary>
        /// 是否自助点餐
        /// </summary>
        public bool IsSelfOrder { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool IsRecommand { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
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
    }

    public enum DiningWay
    {
        所有 = 0,
        堂食 = 1,
        外卖 = 2,
        自助 = 3
    }
}
