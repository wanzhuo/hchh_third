using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Printer.Models;

namespace ZRui.Web.Core.Printer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (PrintDbContext printDb = TPrintDbFactory.MarkShopDb())
            {
                var parameter = new ZRui.Web.Core.Printer.PrintAPIArgsModel.AddPrinter()
                {
                    shopId = 4,
                    sn = "817513721",
                    skey = "ew64m5j4",
                    printerType = PrinterType.FEIE
                };

                using (ShopDbContext shopDb = TShopDbContxtFactory.MarkShopDb())
                {
                    if (string.IsNullOrEmpty(parameter.sn))
                        Error("SnList(打印机#KEY)参数不能为空");
                    if (!parameter.shopId.HasValue)
                        Error("商铺ID不能为空");
                    if (!parameter.printerType.HasValue)
                        Error("打印机类型不能为空");
                    Shop shop = shopDb.GetSingle<Shop>(parameter.shopId.Value);
                    if (shop == null)
                        Error("商铺不存在");
                    //表示是否已经添加过        
                    Data.Printer printer = printDb.Printer.FirstOrDefault(s => !s.IsDel && s.ShopID == parameter.shopId && s.SN == parameter.sn
                                                   && s.SKey == parameter.skey && s.IsSuccess);
                    bool IsExists = printer != null;
                    if (IsExists)
                        Error("设备已经添加过");
                    PrinterBase @base = PrinterFactory.Create(parameter.printerType.Value);//)
                    string result = @base.AddPrinter(new PrintParameter()
                    {
                        SnList = $"{parameter.sn}#{parameter.skey}#"
                    });
                    //添加之后保存到数据库中
                    Dictionary<string, object> tempdic = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (!tempdic.ContainsKey("data"))
                        Error("添加失败");

                    Data.Printer _printermodel = new Data.Printer();
                    #region 实体赋值
                    _printermodel.ShopID = parameter.shopId.Value;
                    _printermodel.SN = parameter.sn;
                    _printermodel.SKey = parameter.skey;
                    _printermodel.AddTime = DateTime.Now;
                    _printermodel.PrinterType = parameter.printerType.Value;
                    _printermodel.IsEnable = true;
                    //_printermodel.Times = string.IsNullOrEmpty(parameter.Times) || int.TryParse(parameter.Times, out int t) ? 1 : t == 0 ? 1 : t;
                    Dictionary<string, object> datadic = JsonConvert.DeserializeObject<Dictionary<string, object>>(tempdic["data"].ToString());
                    JArray ok = datadic["ok"] as JArray;
                    JArray no = datadic["no"] as JArray;
                    //验证打印机是否在接口添加成功
                    //如果之前添加失败了, 将修改原来的数据，否则重新添加
                    #endregion

                    if (ok.Count > 0)
                    {
                    }
                    else
                    {
                        Error(no[0].Value<string>());
                    }
                }
            }

            void Error(string s) { };
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (ShopDbContext shopDb = TShopDbContxtFactory.MarkShopDb())
            {
                LoggerFactory logger = new LoggerFactory();
                var _logger = logger.CreateLogger<UnitTest1>();
                var conglomerationActivity = shopDb.ConglomerationActivity.Find(91);
                //DbExtention.PrintOrder(printDb, shopDb, order,"单元测试", _logger);
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(conglomerationActivity.Context);
                //byte[] byteArray = System.Text.Encoding.Default.GetBytes("asdasdasdada");
                Console.Write(byteArray.Count());
            }



        }

    }
}
