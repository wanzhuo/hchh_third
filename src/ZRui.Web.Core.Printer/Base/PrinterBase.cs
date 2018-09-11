using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Models;

namespace ZRui.Web.Core.Printer.Base
{
   public class PrinterBase
    {
        /// <summary>
        /// 打印方法
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string PrinterRequest(PrintParameter parameter,Data.Printer printer)
        {
            return "[{msg:未添加打印机品牌}]";
        }
        /// <summary>
        /// 添加打印机 
        /// </summary>
        /// <returns></returns>
        public virtual string AddPrinter(PrintParameter parameter)
        {
            return "[{msg:未添加打印机品牌}]";
        }
        /// <summary>
        /// 查询订单是否打印成功
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public virtual string QureyOrderState(string orderID="")
        {
            return string.Empty;
        }
        /// <summary>
        /// 查询指定打印机某天的订单详情，返回已打印订单数和等待打印数。
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        public virtual string QueryOrderInfoByDate(string SN="",string Date="")
        {
            return string.Empty;
        }
        /// <summary>
        /// 查询指定打印机状态，返回该打印机在线或离线，正常或异常的信息。
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public virtual string QueryPrinterStatus(string SN="")
        {
            return string.Empty;
        }
        public virtual string GetOrderID(string message="")
        {
            return "[{msg:未添加打印机品牌}]";
        }
        /// <summary>
        /// 添加打印模板
        /// </summary>
        /// <param name="modelcontent"></param>
        /// <returns></returns>
        public virtual bool AddModel(string modelcontent)

        {
            return false;
        }
        /// <summary>
        /// 根据模板生成请求内容
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual string GetRequsetContent(PrintParameter parameter,Data.PrintWay printWay)
        {
            return string.Empty;
        }
    }
}
