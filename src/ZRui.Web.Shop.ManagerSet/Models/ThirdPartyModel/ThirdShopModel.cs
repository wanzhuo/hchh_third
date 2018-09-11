using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Data;
using static ZRui.Web.Data.ThirdShop;

namespace ZRui.Web.Models.ThirdPartyModel
{
    public class ThirdShopIdModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
    }
    /// <summary>
    /// 第三方门店
    /// </summary>
    public class ThirdShopArgs
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }

        //public ThirdShopsModel Models { get; set; }
        public List<ThirdShopModel> ThirdShopModel { get; set; }



    }


    public class ThirdShopsModel
    {
        public List<ThirdShopModel> ThirdShopModel { get; set; }
    }

    /// <summary>
    /// 第三方门店
    /// </summary>
    public class ThirdShopModel
    {
        /// <summary>
        /// 门店名称
        /// </summary>
        public string station_name { get; set; }
        /// <summary>
        /// 业务类型(食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5)
        /// </summary>
        public BusinessType business { get; set; }
        /// <summary>
        /// 城市名称(如,上海)
        /// </summary>
        public string city_name { get; set; }
        /// <summary>
        /// 区域名称(如,浦东新区)
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string station_address { get; set; }
        /// <summary>
        /// 门店经度
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// 门店纬度
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contact_name { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 门店编码,可自定义,但必须唯一;若不填写,则系统自动生成
        /// </summary>
        public string origin_shop_id { get; set; }
        /// <summary>
        /// 联系人身份证
        /// </summary>
        public string id_card { get; set; }
        /// <summary>
        /// 达达商家app账号(若不需要登陆app,则不用设置)
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 达达商家app密码(若不需要登陆app,则不用设置)
        /// </summary>
        public string password { get; set; }
        public string statusstr { get; set; }
        /// <summary>
        /// 门店状态
        /// </summary>
        public ShopStatus status { get; set; }



    }

    public class ThirdShopRechargeDto
    {
        public double Amount { get; set; }
        public DateTime AddTime { get; set; }
    }


    /// <summary>
    /// 更新门店
    /// </summary>
    public class ThirdShopUpdateModel : ThirdShopIdModel
    {
        /// <summary>
        /// 门店编码(必传)
        /// </summary>
        public string origin_shop_id { get; set; }
        /// <summary>
        /// 新的门店编码(非必传)
        /// </summary>
        public string new_shop_id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string station_name { get; set; }
        /// <summary>
        ///业务类型(食品小吃-1, 饮料-2, 鲜花-3, 文印票务-8, 便利店-9, 水果生鲜-13, 同城电商-19, 医药-20, 蛋糕-21, 酒品-24, 小商品市场-25, 服装-26, 汽修零配-27, 数码-28, 小龙虾-29, 其他-5)
        /// </summary>  
        public BusinessType business { get; set; }
        /// <summary>
        ///城市名称(如, 上海)
        /// </summary>
        public string city_name { get; set; }
        /// <summary>
        ///区域名称(如, 浦东新区)
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string station_address { get; set; }

        /// <summary>
        /// 门店经度
        /// </summary>
        public double lng { get; set; }

        /// <summary>
        /// 门店纬度
        /// </summary>
        public double lat { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contact_name { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        ///门店状态（1-门店激活，0-门店下线）
        /// </summary>
        public ShopStatus status { get; set; }


    }
    /// <summary>
    /// 门店详情
    /// </summary>
    public class ThirdShopDetail : ThirdShopIdModel
    {
        /// <summary>
        ///门店编码
        /// </summary>
        public string origin_shop_id { get; set; }
    }

    /// <summary>
    /// 发单
    /// </summary>
    public class ThirdShopAddOrderModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 门店编号，门店创建后可在门店列表和单页查看
        /// </summary>
        public string shop_no { get; set; }
        /// <summary>
        /// 第三方订单ID
        /// </summary>
        public string origin_id { get; set; }
        /// <summary>
        /// 订单所在城市的code
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public double cargo_price { get; set; }
        /// <summary>
        /// 是否需要垫付 1:是 0:否 (垫付订单金额，非运费)
        /// </summary>
        public int is_prepay { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string receiver_name { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string receiver_address { get; set; }
        /// <summary>
        /// 收货人地址维度（高德坐标系）
        /// </summary>
        public double receiver_lat { get; set; }
        /// <summary>
        /// 收货人地址经度（高德坐标系）
        /// </summary>
        public double receiver_lng { get; set; }
        /// <summary>
        /// 回调URL
        /// </summary>
        public string callback { get; set; }
        /// <summary>
        /// 收货人手机号（手机号和座机号必填一项）
        /// </summary>
        public string receiver_phone { get; set; }
        /// <summary>
        /// 收货人座机号（手机号和座机号必填一项）
        /// </summary>
        public string receiver_tel { get; set; }
        /// <summary>
        /// 小费（单位：元，精确小数点后一位）
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string info { get; set; }
        /// <summary>
        /// 订单商品类型：食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5
        /// </summary>
        public int cargo_type { get; set; }
        /// <summary>
        /// 订单重量（单位：Kg）
        /// </summary>
        public double cargo_weight { get; set; }
        /// <summary>
        /// 订单商品数量
        /// </summary>
        public int cargo_num { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string invoice_title { get; set; }
        /// <summary>
        /// 订单来源标示（该字段可以显示在达达app订单详情页面，只支持字母，最大长度为10）
        /// </summary>
        public string origin_mark { get; set; }
        /// <summary>
        /// 订单来源编号（该字段可以显示在达达app订单详情页面，支持字母和数字，最大长度为30）
        /// </summary>
        public string origin_mark_no { get; set; }
        /// <summary>
        /// 是否使用保价费（0：不使用保价，1：使用保价； 同时，请确保填写了订单金额（cargo_price））
        // 商品保价费(当商品出现损坏，可获取一定金额的赔付)
        //保费=配送物品实际价值* 费率（5‰），配送物品价值及最高赔付不超过10000元， 最高保费为50元（物品价格最小单位为100元，不足100元部分按100元认定，保价费向上取整数， 如：物品声明价值为201元，保价费为300元*5‰=1.5元，取整数为2元。）
        //若您选择不保价，若物品出现丢失或损毁，最高可获得平台30元优惠券。 （优惠券直接存入用户账户中）。
        /// </summary>
        public int is_use_insurance { get; set; }
        /// <summary>
        /// 收货码（0：不需要；1：需要。收货码的作用是：骑手必须输入收货码才能完成订单妥投）
        /// </summary>
        public int is_finish_code_needed { get; set; }
        /// <summary>
        /// 预约发单时间（预约时间unix时间戳(10位),精确到分;整10分钟为间隔，并且需要至少提前20分钟预约。）
        /// </summary>
        public int delay_publish_time { get; set; }
        /// <summary>
        /// 是否选择直拿直送（0：不需要；1：需要。选择直拿只送后，同一时间骑士只能配送此订单至完成，同时，也会相应的增加配送费用）
        /// </summary>
        public int is_direct_delivery { get; set; }
    }

    public class ThirdShopRechargeModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        public decimal amount { get; set; }
        public string category { get; set; }
        public string notify_url { get; set; }

    }
    /// <summary>
    /// 接单Model
    /// </summary>
    public class ThirdOrdersModel
    {
        /// <summary>
        /// 返回达达运单号，默认为空
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 添加订单接口中的origin_id值
        /// </summary>
        public string order_id { get; set; }
        public string OrderStatusStr { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus order_status { get; set; }
        /// <summary>
        /// 订单取消原因,其他状态下默认值为空字符串
        /// </summary>
        public string cancel_reason { get; set; }

        public string CancelFromStr { get; set; }
        /// <summary>
        /// 订单取消原因来源(1:达达配送员取消；2:商家主动取消；3:系统或客服取消；0:默认值)
        /// </summary>
        public CancelFrom cancel_from
        {
            get; set;

        }
        /// <summary>
        /// 更新时间,时间戳
        /// </summary>
        public int update_time { get; set; }
        /// <summary>
        /// 对client_id, order_id, update_time的值进行字符串升序排列，再连接字符串，取md5值
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 达达配送员id，接单以后会传
        /// </summary>
        public int dm_id { get; set; }
        /// <summary>
        /// 配送员姓名，接单以后会传
        /// </summary>
        public string dm_name { get; set; }
        /// <summary>
        /// 配送员手机号，接单以后会传
        /// </summary>
        public string dm_mobile { get; set; }
        public string ThirdTypeStr { get; set; }
        public string ThirdUpdateTime { get; set; }
        /// <summary>
        /// 配送类型 1 达达配送 2 其它配送
        /// </summary>
        public ThirdType third_type { get; set; }

    }

    /// <summary>
    /// 接单Model
    /// </summary>
    public class ThirdOrderModel
    {
        /// <summary>
        /// 返回达达运单号，默认为空
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 添加订单接口中的origin_id值
        /// </summary>
        public string order_number { get; set; }
        public string OrderStatusStr { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus order_status { get; set; }
        /// <summary>
        /// 订单取消原因,其他状态下默认值为空字符串
        /// </summary>
        public string cancel_reason { get; set; }

        public string CancelFromStr { get; set; }
        /// <summary>
        /// 订单取消原因来源(1:达达配送员取消；2:商家主动取消；3:系统或客服取消；0:默认值)
        /// </summary>
        public CancelFrom cancel_from
        {
            get; set;

        }
        /// <summary>
        /// 更新时间,时间戳
        /// </summary>
        public int update_time { get; set; }
        /// <summary>
        /// 对client_id, order_id, update_time的值进行字符串升序排列，再连接字符串，取md5值
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 达达配送员id，接单以后会传
        /// </summary>
        public int dm_id { get; set; }
        /// <summary>
        /// 配送员姓名，接单以后会传
        /// </summary>
        public string dm_name { get; set; }
        /// <summary>
        /// 配送员手机号，接单以后会传
        /// </summary>
        public string dm_mobile { get; set; }
        public string ThirdTypeStr { get; set; }
        public string ThirdUpdateTime { get; set; }
        /// <summary>
        /// 配送类型 1 达达配送 2 其它配送
        /// </summary>
        public ThirdType third_type { get; set; }

    }


    /// <summary>
    /// 订单信息查询
    /// </summary>
    public class OrderInfoQueryModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        public string order_id { get; set; }
    }

    public class FormalCancel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 第三方订单编号
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 取消原因ID
        /// </summary>
        public int cancel_reason_id { get; set; }
        /// <summary>
        /// 取消原因(当取消原因ID为其他时，此字段必填)
        /// </summary>
        public string cancel_reason { get; set; }
    }

    public class ThirdShopRechargeQueryModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 查询运费账户类型（1：运费账户；2：红包账户，3：所有），默认查询运费账户余额
        /// </summary>
        public int category { get; set; }
    }
    public class ThirdMoneyReportModel
    {

        public int ShopId { get; set; }
        public string OrderNumber { get; set; }
        /// <summary>
        /// 运费/违约金
        /// </summary>
        public double Amount { get; set; }

        public DateTime AddTime { get; set; }
        public string ProduceTypeStr { get; set; }
        /// <summary>
        /// 费用产生类型
        /// </summary>
        public ProduceType ProduceType { get; set; }
    }


}
