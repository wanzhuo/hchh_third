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
                        Error("SnList(��ӡ��#KEY)��������Ϊ��");
                    if (!parameter.shopId.HasValue)
                        Error("����ID����Ϊ��");
                    if (!parameter.printerType.HasValue)
                        Error("��ӡ�����Ͳ���Ϊ��");
                    Shop shop = shopDb.GetSingle<Shop>(parameter.shopId.Value);
                    if (shop == null)
                        Error("���̲�����");
                    //��ʾ�Ƿ��Ѿ���ӹ�        
                    Data.Printer printer = printDb.Printer.FirstOrDefault(s => !s.IsDel && s.ShopID == parameter.shopId && s.SN == parameter.sn
                                                   && s.SKey == parameter.skey && s.IsSuccess);
                    bool IsExists = printer != null;
                    if (IsExists)
                        Error("�豸�Ѿ���ӹ�");
                    PrinterBase @base = PrinterFactory.Create(parameter.printerType.Value);//)
                    string result = @base.AddPrinter(new PrintParameter()
                    {
                        SnList = $"{parameter.sn}#{parameter.skey}#"
                    });
                    //���֮�󱣴浽���ݿ���
                    Dictionary<string, object> tempdic = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (!tempdic.ContainsKey("data"))
                        Error("���ʧ��");

                    Data.Printer _printermodel = new Data.Printer();
                    #region ʵ�帳ֵ
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
                    //��֤��ӡ���Ƿ��ڽӿ���ӳɹ�
                    //���֮ǰ���ʧ����, ���޸�ԭ�������ݣ������������
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
                //DbExtention.PrintOrder(printDb, shopDb, order,"��Ԫ����", _logger);
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(conglomerationActivity.Context);
                //byte[] byteArray = System.Text.Encoding.Default.GetBytes("asdasdasdada");
                Console.Write(byteArray.Count());
            }



        }

    }
}
