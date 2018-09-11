using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.Core.Printer;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Printer.Models;

namespace ZRui.Web.ShopManager.PrintExtension
{
    public static class PrintExtension
    {

        public static void PrintOrder(this PrintDbContext printDbContext, ShopDbContext db, ShopOrder shopOrder, string shopname)
        {
            try
            {
                if (shopOrder == null)
                    throw new Exception("订单错误");
                List<ShopOrderItem> items = db.Query<ShopOrderItem>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopOrderId == shopOrder.Id)
                    .ToList();
                if (items.Count == 0) return;
                List<OrderInfo> orderInfo = items.Select(s => new OrderInfo()
                {
                    Name = $"{s.CommodityName}({s.SkuSummary})",
                    Price = Math.Round(s.SalePrice / 100d, 2),
                    Count = s.Count
                }).ToList();
                string stringorderlist = JsonConvert.SerializeObject(orderInfo);

                List<Printer> printers = printDbContext.Query<Printer>()
                    .Where(s => !s.IsDel)
                    .Where(s => s.ShopID == shopOrder.ShopId && s.IsEnable).ToList();
                if (printers.Count == 0) return;

                PrintParameter parameter = GetPrintParameter(db, shopOrder);
                parameter.ShopName = shopname;
                parameter.List = orderInfo;

                foreach (var item in printers)
                {
                    //StringBuilder postData = new StringBuilder("sn=" + item.SN);
                    parameter.SN = item.SN;
                    parameter.Times = item.Times + "";
                    PrintModel model = printDbContext.Query<PrintModel>().FirstOrDefault(s => s.ID == item.ModelID);
                    if (model == null) model = printDbContext.Set<PrintModel>().Find(2);
                    parameter.ModelContent = model.ModelContent;
                    PrinterBase @base = PrinterFactory.Create(item.PrinterType);//)
                    string temp = @base.PrinterRequest(parameter, item);
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
                    record.Remark = shopOrder.Remark;
                    printDbContext.AddTo(record);
                    #endregion
                }
                shopOrder.IsPrint = true;
            }
            catch (Exception e)
            {
                //_logger.LogError("打印机错误：{0}", e.Message);
            }
        }

        static PrintParameter GetPrintParameter(ShopDbContext db, ShopOrder shopOrder)
        {
            PrintParameter parameter = new PrintParameter();
            //是否外卖
            if (shopOrder.IsTakeOut)
            {
                parameter.Title = "外卖单";
                var memberAddress = db.Query<MemberAddress>()
                    .Where(m => !m.IsDel && m.IsUsed)
                    .Where(m => m.MemberId == shopOrder.MemberId)
                    .FirstOrDefault();
                if (memberAddress == null)
                {
                    parameter.Address = "用户未填写地址";
                    var memberPhone = db.Query<MemberPhone>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.State == MemberPhoneState.已绑定)
                        .Where(m => m.MemberId == shopOrder.MemberId)
                        .FirstOrDefault();
                    parameter.Mobile = memberPhone?.Phone;
                }
                else
                {
                    shopOrder.memberAddressId = memberAddress.Id;
                    parameter.Address = memberAddress.Detail;
                    parameter.Mobile = memberAddress.Phone;
                    parameter.OrderName = memberAddress.Name;
                }
            }
            else
            {
                string shoppart;
                if (shopOrder.ShopPartId == null)
                    shoppart = "无桌号";
                else
                    shoppart = db.Query<ShopPart>()
                        .Where(m => m.Id == shopOrder.ShopPartId)
                        .FirstOrDefault()
                        .Title;
                parameter.Title = shoppart + "的点餐单";
            }
            parameter.ShopID = shopOrder.ShopId;
            parameter.TotalMoney = Math.Round(shopOrder.Amount / 100d, 2);
            parameter.Remark = shopOrder.Remark;
            parameter.OrderID = shopOrder.Id + "";
            return parameter;
        }
    }
}
