using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.Printers;

namespace ZRui.Web.Core.Printer
{
   public class PrinterFactory
    {
        /// <summary>
        /// 根据打印机品牌返回相应类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PrinterBase Create(Data.PrinterType pType)
        {
            switch (pType)
            {
                case Data.PrinterType.FEIE:
                    return new FeiePrinter();
                default:
                    return new PrinterBase();
            }
        }
    }
}
