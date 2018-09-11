using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuiChiHuiHe.Auth
{
    public class DeliveryOrder
    {
        public string Shop_no;           // String  是 门店编号，门店创建后可在门店列表和单页查看
        public string Origin_id;           //    String 是   第三方订单ID
        public string City_code;  // String 是   订单所在城市的code（查看各城市对应的code值）
        public string Cargo_price; // Double  是 订单金额
        public string Is_prepay;// Integer 是 是否需要垫付 1:是 0:否(垫付订单金额，非运费)
        public string  Receiver_name;// String  是 收货人姓名
        public string  Receiver_address;// String  是 收货人地址
        public string  Receiver_lat;//Double  是 收货人地址维度（高德坐标系）
        public string  Receiver_lng;// Double  是 收货人地址经度（高德坐标系）
        public string  Callback;//String  是 回调URL（查看回调说明）
        public string  Receiver_phone;// String  否 收货人手机号（手机号和座机号必填一项）
        public string  Receiver_tel;//String  否 收货人座机号（手机号和座机号必填一项）
        public string  Tips;//Double  否 小费（单位：元，精确小数点后一位）
        public string  Info;//String  否 订单备注
        public string  Cargo_type;// Integer 否 订单商品类型：食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5
        public string  Cargo_weight;//Double  否 订单重量（单位：Kg）
        public string  Cargo_num;//Integer 否 订单商品数量
        public string  Invoice_title;//String  否 发票抬头
        public string  Origin_mark;//String  否 订单来源标示（该字段可以显示在达达app订单详情页面，只支持字母，最大长度为10）
        public string  Origin_mark_no;// String  否 订单来源编号（该字段可以显示在达达app订单详情页面，支持字母和数字，最大长度为30）
        public string  Is_use_insurance;//Integer 否
                                        // 是否使用保价费（0：不使用保价，1：使用保价； 同时，请确保填写了订单金额（cargo_price））
                                        //商品保价费(当商品出现损坏，可获取一定金额的赔付)
                                        //保费=配送物品实际价值* 费率（5‰），配送物品价值及最高赔付不超过10000元， 最高保费为50元（物品价格最小单位为100元，不足100元部分按100元认定，保价费向上取整数， 如：物品声明价值为201元，保价费为300元*5‰=1.5元，取整数为2元。）
                                        // 若您选择不保价，若物品出现丢失或损毁，最高可获得平台30元优惠券。 （优惠券直接存入用户账户中）。
        public string  Is_finish_code_needed;// Integer 否 收货码（0：不需要；1：需要。收货码的作用是：骑手必须输入收货码才能完成订单妥投）
        public string  Delay_publish_time;// Integer 否 预约发单时间（预约时间unix时间戳(10位),精确到分;整10分钟为间隔，并且需要至少提前20分钟预约。）
        public string  Is_direct_delivery;// Integer 否 是否选择直拿直送（0：不需要；1：需要。选择直拿只送后，同一时间骑士只能配送此订单至完成，同时，也会相应的增加配送费用）
    }
}
