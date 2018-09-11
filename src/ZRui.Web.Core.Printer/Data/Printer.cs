using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Printer.Data
{
    public class Printer : EntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商铺编号    
        /// </summary>
        public int ShopID { get; set; }
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }
        /// <summary>
        /// 打印机编号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 打印机key
        /// </summary>
        public string SKey { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        //是否添加成功
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 所属厂商
        /// </summary>
        public PrinterType PrinterType { get; set; }
        /// <summary>
        /// 打印方式
        /// </summary>
        public PrintWay PrintWay { get; set; }
        /// <summary>
        /// 联数
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 模板编号
        /// </summary>
        public int ModelID { get; set; }
    }

    public enum PrinterType
    {
        FEIE = 0
    }

    /// <summary>
    /// 打印方式
    /// </summary>
    public enum PrintWay
    {
        整单 = 0,
        一菜一单 = 1,
        日营业报表 = 2
    }
}
