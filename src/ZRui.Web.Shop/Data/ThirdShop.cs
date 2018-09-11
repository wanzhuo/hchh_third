using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Data
{
    /// <summary>
    /// 第三方门店
    /// </summary>
    public class ThirdShop
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ShopId
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 业务类型(食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5)
        /// </summary>
        public BusinessType Business { get; set; }
        /// <summary>
        /// 城市名称(如,上海)
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区域名称(如,浦东新区)
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string StationAddress { get; set; }
        /// <summary>
        /// 门店经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 门店纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 门店编码,可自定义,但必须唯一;若不填写,则系统自动生成
        /// </summary>
        public string OriginShopId { get; set; }
        /// <summary>
        /// 联系人身份证
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 达达商家app账号(若不需要登陆app,则不用设置)
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 达达商家app密码(若不需要登陆app,则不用设置)
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 门店状态
        /// </summary>
        public ShopStatus Status { get; set; }

        public enum BusinessType
        {
            食品小吃 = -1,
            饮料 = -2,
            鲜花 = -3,
            文印票务 = -8,
            便利店 = -9,
            水果生鲜 = -13,
            同城电商 = -19,
            医药 = -20,
            蛋糕 = -21,
            酒品 = -24,
            小商品市场 = -25,
            服装 = -26,
            汽修零配 = -27,
            数码 = -28,
            小龙虾 = -29,
            其他 = -5
        }

        public enum ShopStatus {
            门店下线 = 0,
            门店激活 = 1

        }

    }
}
