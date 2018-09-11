using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Printer.PrintAPIArgsModel
{
    public class AddPrinter
    {
        public int? shopId { get; set; }
        public string sn { get; set; }
        public string skey { get; set; }
        public Data.PrinterType? printerType { get; set; }
        public Data.PrintWay? printWay { get; set; }
        public string PrinterName { get; set; }
    }


    public class SetPrinter
    {
        public int? shopId { get; set; }
        public string sn { get; set; }
        public string skey { get; set; }
        public Data.PrinterType? printerType { get; set; }
    }


    public class GetPrinterListArgsModel
    {
        public int? shopId { get; set; }
    }

    public class GetPrinterTypeListRtn
    {
        public string name { get; set; }
        public int value { get; set; }
    }
}
