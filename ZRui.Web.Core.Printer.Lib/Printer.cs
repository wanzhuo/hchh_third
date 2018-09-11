using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Data;

namespace ZRui.Web.Core.Printer.Lib
{
  public class Printer
    {
        private PrintDbContext printDb;
        private ShopDbContext shopDb;
        public Printer(PrintDbContext printDb, ShopDbContext shopDb)
        {
            this.printDb = printDb;
            this.shopDb = shopDb;
        }
        public static APIResult Print( PrintParameter parameter)
        {
            if (string.IsNullOrEmpty(parameter.Title))
                return Error("标题是必须的内容");
            if (parameter.List.Count == 0)
                return Error("订单列表格式有误,[{Name:'xxxx',Price:10,Count:1},{}]");
            if (parameter.ShopID == 0 && string.IsNullOrEmpty(parameter.ShopFlag))
                return Error("商铺编号是必须的,ID Flag必须存在其一");
            Shop shop = parameter.ShopID != 0 ? shopDb.GetSingle<Shop>(parameter.ShopID) : shopDb.Shops.FirstOrDefault(s => s.Flag == parameter.ShopFlag);
            if (shop == null)
                return Error("商铺不存在");
            List<OrderInfo> orderlist = parameter.List; //JsonConvert.DeserializeObject<List<OrderInfo>>(parameter.List); //根据集合填充内容
            if (parameter.List == null || orderlist.Count == 0)
                return Error("无任何订单,无需打印");
            string stringorderlist = JsonConvert.SerializeObject(orderlist);
            foreach (OrderInfo item in orderlist)
                parameter.TotalMoney += item.Money;
            List<Data.Printer> printers = printDb.Printer.Where(s => s.ShopID == parameter.ShopID && s.IsEnable && s.IsSuccess).ToList();
            if (printers.Count == 0)
                return Error("暂未添加相关打印机");
            // string[] snarry = parameter.SN.TrimEnd('#').Split('#');
            StringBuilder result = new StringBuilder("[");
            foreach (Data.Printer item in printers)
            {
                //StringBuilder postData = new StringBuilder("sn=" + item.SN);
                parameter.SN = item.SN;
                PrinterBase @base = PrinterFactory.Create(string.IsNullOrEmpty(item.PrinterType) ? "FEIE" : item.PrinterType);//)
                string temp = @base.PrinterRequest(parameter);
                #region 数据库操作
                PrintRecord record = new PrintRecord();
                //处理接口返回数据
                Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp);
                //实体赋值
                record.SN = item.SN;
                record.OrderID = @base.GetOrderID(temp);// dic.ContainsKey("data") && dic["msg"].Equals("ok") ? dic["data"].ToString() : "未能成功打印";
                record.Title = parameter.Title;
                record.OrderList = stringorderlist;
                record.TotalMoney = (float)parameter.TotalMoney;
                record.Address = parameter.Address;
                record.OrderName = parameter.OrderName;
                record.Mobile = parameter.Mobile;
                record.OrderTime = Convert.ToDateTime(parameter.OrderTime);
                record.QRAddress = parameter.QRAddress;
                record.Remark = parameter.Remark;
                printDb.PrintRecord.Add(record);
                result.Append(temp);
                #endregion
            }
            result.Append("]");
            return Success(result.ToString());
        }

    }
}
