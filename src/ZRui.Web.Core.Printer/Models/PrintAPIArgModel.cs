using System;
using System.Collections.Generic;

namespace ZRui.Web.Core.Printer.Models
{
    public class PrintParameter
    {
        /// <summary>
        /// 名字(会员卡)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别(会员卡)
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机(会员卡)
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 商铺编号    
        /// </summary>
        public int ShopID { get; set; }
        public string ShopFlag { get; set; }
        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopName { get; set; }
        /// <summary>
        /// 打印机编号(必填) # 打印机识别码(必填) # 备注名称(选填) # 流量卡号码(选填)，多台打印机请换行（\n）添加新打印机信息，每次最多100行(台)
        /// </summary>
        public string SnList { get; set; }

        /// <summary>
        /// 打印机编号,多个打印机 请用#隔开如 123456#654321
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 打印机上的KEY
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 订单号 由打印订单成功之后返回
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 查询时间（YYYY-MM-DD）
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 订单列表[{Name:'xxxx',Price:10,Count:1},{}]
        /// </summary>
        public List<OrderInfo> List { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 应付金额
        /// </summary>
        public double TotalMoney { get; set; }
        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 订餐时间
        /// </summary>
        public string OrderTime
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime { get; set; }
         
        /// <summary>
        /// 二维码地址
        /// </summary>
        public string QRAddress { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 打印联数 默认为1
        /// </summary>
        public string Times { get; set; }
        /// <summary>
        /// 下单人姓名
        /// </summary>
        public string OrderName { get; set; }
        /// <summary>
        /// 打印机品牌
        /// </summary>
        public string PrinterType { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayTypeName { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string ModelContent { get; set; }

        /// <summary>
        /// 是否外卖
        /// </summary>
        public bool IsTakeOut { get; set; }
        /// <summary>
        /// 满减规则
        /// </summary>
        public ShopOrderMoneyOffRuleModel ShopOrderMoneyOffRule { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public double Payment { get; set; }

        /// <summary>
        /// 其他金额
        /// </summary>
        public Dictionary<string, double> ShopOrderOtherFee { get; set; }

        /// <summary>
        /// 外卖下单方式
        /// </summary>
        public string TakeWay { get; set; } = "";
        /// <summary>
        /// 自提\配送时间
        /// </summary>
        public DateTime PickupTime { get; set; }
        public SelfHelpPrintParameter SelfHelpPrintParameter { get; set; }

        public object ShopDayOpenReportAPIModels { get; set; }

        /// <summary>
        /// 配送方式 商家配送 ,达达配送 = 1
        /// </summary>
        public string takeDistributionType { get; set; }

    }


    public class SelfHelpPrintParameter
    {
        /// <summary>
        /// 自助取餐点餐号码
        /// </summary>
        public string SelfHelpNumber { get; set; }
        /// <summary>
        /// 就餐方式
        /// </summary>
        public string DingingWay { get; set; }
    }


    /// <summary>
    /// 满减规则实体
    /// </summary>
    public class ShopOrderMoneyOffRuleModel
    {
        /// <summary>
        /// 关联优惠活动Id
        /// </summary>
        public int MoneyOffId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string MoneyOffName { get; set; }

        /// <summary>
        /// 满足金额
        /// </summary>
        public double FullAmount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double Discount { get; set; }

    }
}
