
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Controllers;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Printer.Web.Models;
using ZRui.Web.Core.Wechat;
using System.Linq;
using Newtonsoft.Json.Linq;
using ZRui.Web.Core.Printer.Models;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.PrintAPIArgsModel;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
//using ZRui.Web.Extension;

namespace ZRui.Web.Core.Printer.Web.Controllers
{
    /// <summary>
    /// 打印机相关接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class PrintAPIController : PrintAPIControllerBase
    {
        private PrintDbContext printDb;
        private ShopDbContext shopDb;
        private MemberDbContext memberDb;
        public PrintAPIController(PrintDbContext printDb, ShopDbContext shopDb, MemberDbContext memberDb)
        {
            this.printDb = printDb;
            this.shopDb = shopDb;
            this.memberDb = memberDb;
        }
        /// <summary>
        /// 打印数据
        /// </summary>
        /// <param name="parameter">{"Sn":"817513721","User":"865335864@163.com","Ukey":"Uw7vWUhz8WqgtvPM","Title":"测试","List":[{Name:"白菜", Price:52, Count:1},{Name:"白菜",Price:52,Count:1}]}</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult Print([FromBody] PrintParameter parameter)
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
                record.Remark = parameter.Remark;
                printDb.PrintRecord.Add(record);
                result.Append(temp);
                #endregion
            }
            result.Append("]");
            return Success(result.ToString());
        }
        //[HttpPost]
        //[Authorize]
        //public APIResult PrintDayOpenReport([FromBody] PrintParameter parameter)
        //{
        //    Shop shop = parameter.ShopID != 0 ? shopDb.GetSingle<Shop>(parameter.ShopID) : shopDb.Shops.FirstOrDefault(s => s.Flag == parameter.ShopFlag);
        //    if (shop == null)
        //        return Error("商铺不存在");
        //    List<Data.Printer> printers = printDb.Printer.Where(s => s.ShopID == parameter.ShopID && s.IsEnable && s.IsSuccess && !s.IsDel&&s.PrintWay== PrintWay.日营业报表).ToList();
        //    if (printers.Count == 0)
        //        return Error("暂未添加相关打印机");
        //    var loginFlag = User.Identity.Name;
        //    var login = this.memberDb.GetSingleMemberLogin(loginFlag);
        //    if (login == null) throw new Exception("未登录");
        //    if (!login.MemberId.HasValue) throw new Exception("用户未登陆");
        //    var member = memberDb.Set<Member>().FirstOrDefault(r => r.Id == login.MemberId && !r.IsDel);
        //    if (member == null) throw new Exception("操作员不存在");

        //    StringBuilder result = new StringBuilder("[");
        //    foreach (Data.Printer item in printers)
        //    {

        //        //StringBuilder postData = new StringBuilder("sn=" + item.SN);
        //        parameter.SN = item.SN;
        //        var obj = shopDb.ExShopDayOpenReportSumAmount(new ZRui.Web.Models.ShopDayOpenReportAPIArgModels() { ShopId = shop.Id, StartTime = parameter.StartTime, EndTime = parameter.EndTime }, member, shop);
        //        parameter.ShopDayOpenReportAPIModels = obj;
        //        PrinterBase @base = PrinterFactory.Create(item.PrinterType);//)
        //        string temp = @base.PrinterRequest(parameter, item);

        //    }
        //    result.Append("]");
        //    return Success();
        //}

        /// <summary>
        /// 添加打印机
        /// <returns></returns>
        ///</summary>
        [HttpPost]
        [Authorize]
        public APIResult AddPrinter([FromBody]AddPrinter parameter)
        {
            if (string.IsNullOrEmpty(parameter.PrinterName))
                return Error("打印机名称不能为空");
            if (string.IsNullOrEmpty(parameter.sn))
                return Error("SnList(打印机#KEY)参数不能为空");
            if (!parameter.shopId.HasValue)
                return Error("商铺ID不能为空");
            if (!parameter.printerType.HasValue)
                return Error("打印机类型不能为空");
            if (!parameter.printWay.HasValue)
                return Error("打印机方式不能为空");
            Shop shop = shopDb.GetSingle<Shop>(parameter.shopId.Value);
            if (shop == null)
                return Error("商铺不存在");
            //表示是否已经添加过        
            Data.Printer printer = printDb.Printer.FirstOrDefault(s => !s.IsDel && s.SN == parameter.sn
                                           && s.SKey == parameter.skey && s.PrinterType == parameter.printerType);
            bool IsExists = printer != null;
            if (IsExists)
            {
                if (printer.ShopID == shop.Id)
                    return Error("该设备已经添加过");
                else
                    return Error("该设备已经被别的商家添加过");
            }
            PrinterBase @base = PrinterFactory.Create(parameter.printerType.Value);//)
            string result = @base.AddPrinter(new PrintParameter()
            {
                SnList = $"{parameter.sn}#{parameter.skey}#"
            });
            //添加之后保存到数据库中
            Dictionary<string, object> tempdic = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            if (!tempdic.ContainsKey("data"))
                return Error("添加失败");

            Data.Printer _printermodel = new Data.Printer();
            #region 实体赋值
            _printermodel.ShopID = parameter.shopId.Value;
            _printermodel.SN = parameter.sn;
            _printermodel.PrinterName = parameter.PrinterName;
            _printermodel.SKey = parameter.skey;
            _printermodel.AddTime = DateTime.Now;
            _printermodel.PrinterType = parameter.printerType.Value;
            _printermodel.PrintWay = parameter.printWay.Value;
            _printermodel.IsEnable = true;
            _printermodel.IsSuccess = true;
            //_printermodel.Times = string.IsNullOrEmpty(parameter.Times) || int.TryParse(parameter.Times, out int t) ? 1 : t == 0 ? 1 : t;
            Dictionary<string, object> datadic = JsonConvert.DeserializeObject<Dictionary<string, object>>(tempdic["data"].ToString());
            JArray ok = datadic["ok"] as JArray;
            JArray no = datadic["no"] as JArray;
            //验证打印机是否在接口添加成功
            //如果之前添加失败了, 将修改原来的数据，否则重新添加
            #endregion

            if (ok.Count > 0)
            {
                printDb.AddTo(_printermodel);
                printDb.SaveChanges();
                return Success(_printermodel);
            }
            else if (no.Count > 0)
            {
                string mes = no[0].Value<string>();
                Regex regex = new Regex(@".*添加过.*");
                if (regex.IsMatch(mes))
                {
                    printDb.AddTo(_printermodel);
                    printDb.SaveChanges();
                    return Success(_printermodel);
                }
                else
                {
                    return Error(mes);
                }
            }
            else
            {
                return Error("添加打印机到供应商后台失败");
            }

        }


        /// <summary>
        /// 获取商铺所有打印机信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetPrinterList([FromBody]GetPrinterListArgsModel args)
        {
            var list = printDb.Query<Data.Printer>()
                            .Where(m => !m.IsDel)
                            .Where(m => m.ShopID == args.shopId)
                            .Select(m => new
                            {
                                m.ID,
                                m.ShopID,
                                m.SN,
                                m.SKey,
                                m.IsEnable,
                                m.IsSuccess,
                                m.PrintWay,
                                PrinterType = m.PrinterType.ToString(),
                                m.PrinterName
                            })
                            .ToList();
            return Success(list);
        }


        /// <summary>
        /// 获取所有打印机类型
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetPrinterTypeList()
        {
            List<GetPrinterTypeListRtn> list = new List<GetPrinterTypeListRtn>();
            string[] names = Enum.GetNames(typeof(PrinterType));
            var vals = Enum.GetValues(typeof(PrinterType));
            for (int i = 0; i < names.Length; i++)
            {
                list.Add(new GetPrinterTypeListRtn()
                {
                    name = names[i],
                    value = (int)vals.GetValue(i)
                });
            }

            return Success(list);
        }


        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetIsDelete([FromBody]IdArgsModel args)
        {
            var model = printDb.Query<Data.Printer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ID == args.Id)
                    .FirstOrDefault();
            if (model == null) return Error("该打印机不存在");
            model.IsDel = true;
            printDb.SaveChanges();
            return Success();
        }


        /// <summary>
        /// 查询订单是否打印成功
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult QureyOrderState([FromBody]PrintParameter parameter)
        {
            if (string.IsNullOrEmpty(parameter.OrderID))
                return Error("OrderID参数不能为空");
            PrinterBase @base = PrinterFactory.Create(PrinterType.FEIE);//)
            string result = @base.QureyOrderState(parameter.OrderID);
            return Success(result);
        }
        /// <summary>
        /// 查询指定打印机某天的订单统计数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>查询指定打印机某天的订单详情，返回已打印订单数和等待打印数。</returns>
        [HttpPost]
        [Authorize]
        public APIResult QueryOrderInfoByDate([FromBody] PrintParameter parameter)

        {
            if (string.IsNullOrEmpty(parameter.SN))
                return Error("Sn参数不能为空");
            if (string.IsNullOrEmpty(parameter.Date))
                parameter.Date = DateTime.Now.ToString("yyyy-MM-dd");
            PrinterBase @base = PrinterFactory.Create(PrinterType.FEIE);//)
            string result = @base.QueryOrderInfoByDate(parameter.SN, parameter.Date);
            return Success(result);
            // return new EmptyResult();
        }
        /// <summary>
        /// 查询指定打印机状态，返回该打印机在线或离线，正常或异常的信息。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize]
        public APIResult QueryPrinterStatus([FromBody] PrintParameter parameter)
        {
            if (string.IsNullOrEmpty(parameter.SN))
                return Error("Sn参数不能为空");
            PrinterBase @base = PrinterFactory.Create(PrinterType.FEIE);//)
            string result = @base.QueryPrinterStatus(parameter.SN);
            return Success(result);
            // return new EmptyResult();
        }

    }
}

