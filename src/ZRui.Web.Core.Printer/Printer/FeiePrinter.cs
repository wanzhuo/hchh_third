using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Printer.Models;

namespace ZRui.Web.Core.Printer.Printers
{
    public class FeiePrinter : PrinterBase
    {
        private const string USer = "865335864@163.com";
        private const string UKey = "Uw7vWUhz8WqgtvPM";
        private const string Url = "http://api.feieyun.cn/Api/Open/";
        private PrintDbContext printDb;
        public FeiePrinter()
        {

        }
        public override string PrinterRequest(PrintParameter parameter, Data.Printer printer)
        {

            string orderinfo = GetRequsetContent(parameter, printer.PrintWay);
            string order = Uri.UnescapeDataString(orderinfo.ToString());
            StringBuilder postData = new StringBuilder("sn=" + parameter.SN);
            postData.Append($"&content={order}");
            postData.Append($"&times={(string.IsNullOrEmpty(parameter.Times) || !int.TryParse(parameter.Times, out int times) ? 1 + "" : parameter.Times)}");
            // int itime = DateTimeToStamp(System.DateTime.Now);//时间戳秒数
            int stime = PrintTools.GetTotalSeconds(DateTime.Now);
            string sig = PrintTools.Sha1(USer, UKey, stime + "");
            postData.Append("&user=" + USer);
            postData.Append("&stime=" + stime);
            postData.Append("&sig=" + sig);
            postData.Append("&apiname=Open_printMsg");
            string result = PrintRequest.RequestMethod(postData.ToString(), Url, Encoding.UTF8);
            return result;

        }
        protected override string GetRequsetContent(PrintParameter parameter, PrintWay printWay)
        {
            //标签说明： 
            //单标签: s
            //"<BR>"为换行,"<CUT>"为切刀指令(主动切纸,仅限切刀打印机使用才有效果)
            //"<LOGO>"为打印LOGO指令(前提是预先在机器内置LOGO图片),"<PLUGIN>"为钱箱或者外置音响指令
            //成对标签：
            //"<CB></CB>"为居中放大一倍,"<B></B>"为放大一倍,"<C></C>"为居中,<L></L>字体变高一倍
            //<W></W>字体变宽一倍,"<QR></QR>"为二维码,"<BOLD></BOLD>"为字体加粗,"<RIGHT></RIGHT>"为右对齐
            StringBuilder orderinfo = new StringBuilder();

            if (printWay == PrintWay.一菜一单)
            {
                List<OrderInfo> orderlist = parameter.List;
                int i = 1, count = orderlist.Count;
                foreach (OrderInfo item in orderlist)
                {
                    if (parameter.SelfHelpPrintParameter != null)
                    {
                        orderinfo.Append($"<CB>取餐号：{parameter.SelfHelpPrintParameter.SelfHelpNumber}</CB><BR>");
                    }
                    orderinfo.Append($"<CB>{parameter.Title}</CB><BR>");
                    orderinfo.Append($"品名   数量    单价    小计<BR>");
                    orderinfo.Append("--------------------------------<BR>");
                    orderinfo.Append($"({orderlist.IndexOf(item) + 1})<B>{item.Name}</B><BR>");
                    if (item.ComboConten.Count != 0)
                    {
                        foreach (var ComboContenItem in item.ComboConten)
                        {
                            orderinfo.Append($"   {ComboContenItem.Name}<BR>");
                        }

                    }
                    orderinfo.Append($"       {item.Count}       {item.Price}    {item.Money}<BR><BR>");
                    orderinfo.Append($"订单号：{parameter.OrderID}<BR><BR>");
                    orderinfo.Append($"下单时间：{parameter.OrderTime}<BR><BR>");
                    orderinfo.Append($"<B>备注：{parameter.Remark}</B><BR>");
                    if (i != count) orderinfo.Append("<BR>--------------------------------<BR><BR><CUT>");
                    i++;
                }
            }
            else if (printWay == PrintWay.整单)
            {
                #region 生成订单详细
                if (parameter.IsTakeOut)
                {
                    parameter.TakeWay = $"({parameter.TakeWay})";
                }
                //标题
                orderinfo.Append("--------------------------------<BR>");
                var nowTime = DateTime.Now;
                orderinfo.Append($"{nowTime.ToString("yyyy年MM月dd日 ")}{PrintTools.GetDayOfWeekZh(nowTime)}<BR>");
                orderinfo.Append("--------------------------------<BR>");
                orderinfo.Append("<CB>#惠吃惠喝小程序</CB><BR>");
                if (!string.IsNullOrEmpty(parameter.ShopName))
                    orderinfo.Append($"<C><BOLD>{parameter.ShopName}</BOLD></C><BR>");
                if (parameter.SelfHelpPrintParameter != null)
                {
                    orderinfo.Append("<C>取餐号</C><BR>");
                    orderinfo.Append($"<CB>{parameter.SelfHelpPrintParameter.SelfHelpNumber}</CB><BR>");
                    orderinfo.Append($"<BOLD>取餐方式：{parameter.SelfHelpPrintParameter.DingingWay}</BOLD><BR><BR>");
                }
                else if (!string.IsNullOrEmpty(parameter.Title))
                    orderinfo.Append($"<CB>{parameter.Title}{parameter.TakeWay}</CB><BR>");
                if (parameter.Name != null)
                {
                    orderinfo.Append($"会员昵称：{parameter.Name}<BR>");
                    orderinfo.Append($"注册手机：{parameter.Phone}<BR>");
                }
                orderinfo.Append($"订单号：{parameter.OrderID}<BR>");
                orderinfo.Append($"支付时间：{parameter.PayTime.ToString("yyyy-MM-dd  HH:mm:ss")}<BR>");
                if (parameter.IsTakeOut)
                {

                    orderinfo.Append("----------- 外卖点餐 -----------<BR>");

                }
                else if (parameter.SelfHelpPrintParameter != null)
                {
                    orderinfo.Append("----------- 自助点餐 -----------<BR>");
                }
                else
                    orderinfo.Append("----------- 堂食点餐 -----------<BR>");
                //根据集合填充内容
                List<OrderInfo> orderlist = parameter.List;
                foreach (OrderInfo item in orderlist)
                {
                    if (item.Name.Length > 5)
                        orderinfo.Append($"{item.Name}<BR>            *{item.Count}  {item.Price}  {item.Money}<BR>");
                    else
                    {
                        orderinfo.Append($"{item.Name}  *{item.Count}  {item.Price}  {item.Money}<BR>");

                    }
                }

                orderinfo.Append("------------- 其他 -------------<BR>");
                if (parameter.ShopOrderOtherFee != null)
                {
                    foreach (var ShopOrderOtherFeeItem in parameter.ShopOrderOtherFee)
                    {
                        if (ShopOrderOtherFeeItem.Value == 0) continue;
                        orderinfo.Append($"{ShopOrderOtherFeeItem.Key}：{ShopOrderOtherFeeItem.Value}<BR>");
                    }
                }
                parameter.PayTypeName = string.IsNullOrEmpty(parameter.PayTypeName) ? "微信支付" : parameter.PayTypeName;
               
                //orderinfo.Append($"应付：{parameter.TotalMoney}<BR>");
                //orderinfo.Append($"结算方式：<BR>");
                //orderinfo.Append($"{parameter.PayTypeName}：{parameter.TotalMoney}<BR>");
                if (!string.IsNullOrEmpty(parameter.QRAddress))
                    orderinfo.Append($"<QR>{parameter.QRAddress}</QR>");
                if (!string.IsNullOrEmpty(parameter.Number))
                    orderinfo.Append($"<C>{parameter.Number}</C>");

                if (parameter.ShopOrderMoneyOffRule != null)
                {
                    orderinfo.Append($"商户其他优惠：-{parameter.ShopOrderMoneyOffRule.Discount}<BR>");
                }
                orderinfo.Append($"支付方式：{parameter.PayTypeName}<BR>");
                if (!string.IsNullOrWhiteSpace(parameter.takeDistributionType))
                {
                    orderinfo.Append($"配送方式：{parameter.takeDistributionType}<BR>");
                }
                if (!string.IsNullOrEmpty(parameter.OrderName))
                {
                    orderinfo.Append("--------------------------------<BR>");
                    orderinfo.Append($"<B>客户：{parameter.OrderName}</B><BR>");
                }
                if (!string.IsNullOrEmpty(parameter.Mobile))
                {
                    orderinfo.Append("--------------------------------<BR>");
                    orderinfo.Append($"<B>电话：{parameter.Mobile}</B><BR>");
                }
                if (!string.IsNullOrEmpty(parameter.Address))
                {
                    orderinfo.Append("--------------------------------<BR>");
                    orderinfo.Append($"<B>地址：{parameter.Address}</B><BR>");
                }
                orderinfo.Append("--------------------------------<BR>");
                if (parameter.TakeWay.Contains("自提"))
                {
                    orderinfo.Append($"<BOLD><B>预计自提时间：</B></BOLD><BR>");
                    orderinfo.Append($"<BOLD><B>{parameter.PickupTime.ToString("yyyy-MM-dd HH:mm")}</B></BOLD><BR>");
                    orderinfo.Append("--------------------------------<BR>");
                }
                if (parameter.TakeWay.Contains("送货"))
                {
                    orderinfo.Append($"<BOLD><B>期望配送时间：</B></BOLD><BR>");
                    orderinfo.Append($"<BOLD><B>{parameter.PickupTime.ToString("yyyy-MM-dd HH:mm")}</B></BOLD><BR>");
                    orderinfo.Append("--------------------------------<BR>");
                }
                orderinfo.Append($"<BOLD><B>合计：{parameter.Payment}</B></BOLD><BR>");
                orderinfo.Append("--------------------------------<BR>");
                orderinfo.Append($"<B>备注：{parameter.Remark}</B><BR>");
                #endregion
            }
            else
            {
                var ShopName = parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("ShopName").GetValue(parameter.ShopDayOpenReportAPIModels).ToString();
                var Operator = parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("Operator").GetValue(parameter.ShopDayOpenReportAPIModels) == null ? "" :
                    parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("Operator").GetValue(parameter.ShopDayOpenReportAPIModels).ToString();
                var SalesAmount = parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("SalesAmount").GetValue(parameter.ShopDayOpenReportAPIModels).ToString();
                var TakeawayAmount = (decimal)parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("TakeawayAmount").GetValue(parameter.ShopDayOpenReportAPIModels);
                var ScanCodeAmount = (decimal)parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("ScanCodeAmount").GetValue(parameter.ShopDayOpenReportAPIModels);
                var SelfHelpAmount = (decimal)parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("SelfHelpAmount").GetValue(parameter.ShopDayOpenReportAPIModels);
                var FightGroupAmount = (decimal)parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("FightGroupAmount").GetValue(parameter.ShopDayOpenReportAPIModels);
                var FullReductionAmount = (decimal)parameter.ShopDayOpenReportAPIModels.GetType().GetProperty("FullReductionAmount").GetValue(parameter.ShopDayOpenReportAPIModels);

                orderinfo.Append($"<CB>日结清单</CB><BR>");
                orderinfo.Append($"--------------------------------<BR>");
                orderinfo.Append($"门店名称：{ShopName}<BR>");
                orderinfo.Append($"打印时间：{DateTime.Now}<BR>");
                orderinfo.Append($"统计日期：{parameter.StartTime} - {parameter.EndTime}<BR>");
                orderinfo.Append($"操作员：{Operator}<BR>");
                orderinfo.Append($"--------------------------------<BR>");
                orderinfo.Append($"<BOLD>销售总额：</BOLD><RIGHT>{SalesAmount}</RIGHT><BR>");
                orderinfo.Append($"--------------------------------<BR>");
                orderinfo.Append($"销售渠道明细<BR>");
                orderinfo.Append($"--------------------------------<BR>");
                orderinfo.Append($"外卖点餐：{TakeawayAmount}<BR>");
                orderinfo.Append($"扫码点餐：{ScanCodeAmount}<BR>");
                orderinfo.Append($"自助点餐：{SelfHelpAmount}<BR>");
                orderinfo.Append($"拼团：{FightGroupAmount}<BR>");
                orderinfo.Append($"优惠项目明细<BR>");
                orderinfo.Append($"--------------------------------<BR>");
                orderinfo.Append($"满减：{FullReductionAmount}<BR>");


            }

            return orderinfo.ToString();
        }
        public override string AddPrinter(PrintParameter parameter)
        {
            int stime = PrintTools.GetTotalSeconds(DateTime.Now);
            StringBuilder postData = new StringBuilder();
            postData.Append("printerContent=" + parameter.SnList);
            postData.Append("&user=" + USer);
            postData.Append("&stime=" + stime);
            postData.Append("&sig=" + PrintTools.Sha1(USer, UKey, stime + ""));
            postData.Append("&apiname=" + "Open_printerAddlist");
            string result = PrintRequest.RequestMethod(postData.ToString(), Url, Encoding.UTF8);
            return result;
        }
        public override string QureyOrderState(string orderID = "")
        {
            if (string.IsNullOrEmpty(orderID))
                return "参数错误";
            string postData = $"orderid={orderID}";
            int itime = PrintTools.GetTotalSeconds(DateTime.Now);//时间戳秒数
            string stime = itime.ToString();
            string sig = PrintTools.Sha1(USer, UKey, stime);
            postData += ("&user=" + USer);
            postData += ("&stime=" + stime);
            postData += ("&sig=" + sig);
            postData += ("&apiname=" + "Open_queryOrderState");
            string result = PrintRequest.RequestMethod(postData.ToString(), Url, Encoding.UTF8);
            return result;
        }
        public override string QueryOrderInfoByDate(string SN = "", string Date = "")
        {
            StringBuilder postData = new StringBuilder($"sn={SN}");
            postData.Append($"&date={Date}");
            int itime = PrintTools.GetTotalSeconds(DateTime.Now);//时间戳秒数
            //string stime = itime.ToString();
            string sig = PrintTools.Sha1(USer, UKey, itime + "");
            postData.Append($"&user={USer}");
            postData.Append($"&stime={itime + ""}");
            postData.Append($"&sig={sig}" + sig);
            postData.Append("&apiname=Open_queryOrderInfoByDate");
            string result = PrintRequest.RequestMethod(postData.ToString(), Url, Encoding.UTF8);
            return result;
        }
        public override string QueryPrinterStatus(string SN = "")
        {
            if (string.IsNullOrEmpty(SN))
                return "参数错误";
            StringBuilder postData = new StringBuilder($"sn={SN}");
            int itime = PrintTools.GetTotalSeconds(DateTime.Now);//时间戳秒数
            //string stime = itime.ToString();
            string sig = PrintTools.Sha1(USer, UKey, itime + "");
            postData.Append($"&user={USer}");
            postData.Append($"&stime={itime + ""}");
            postData.Append($"&sig={sig}");
            postData.Append("&apiname=Open_queryPrinterStatus");
            string result = PrintRequest.RequestMethod(postData.ToString(), Url, Encoding.UTF8);
            return result;
        }
        public override string GetOrderID(string message = "")
        {
            if (string.IsNullOrEmpty(message))
                return "参数错误";
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            return dic.ContainsKey("data") && dic["msg"].Equals("ok") ? dic["data"].ToString() : "未能成功打印";
        }

    }
}
