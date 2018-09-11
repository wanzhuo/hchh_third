using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Printer.Data
{
    public class PrintRecord
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 打印机编号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 订单编号 该编号由打印成功之后返回
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 订单列表，存储的是JSon格式的数据
        /// </summary>
        public string OrderList { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public float TotalMoney { get; set; }
        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 下单人
        /// </summary>
        public string OrderName { get; set; }
        /// <summary>
        /// 下单人电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }
        /// <summary>
        /// 二维码地址
        /// </summary>
        public string QRAddress { get; set; } 
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
