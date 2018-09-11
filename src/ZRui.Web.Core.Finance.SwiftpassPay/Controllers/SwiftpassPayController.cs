//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using ZRui.Web.Core.Finance.PayBase;
//using ZRui.Web.Core.Wechat;
//using ZRui.Web.Core.Printer.Data;
//using static ZRui.Web.Models.SwiftpassPayModel;
//using ZRui.Web.BLL;
//using ZRui.Web.Core.Finance.SwiftpassPay;

//namespace ZRui.Web.Controllers
//{
//    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
//    [Route("[controller]/[action]")]
//    public class SwiftpassPayController : Controller
//    {

//        private readonly ILogger _logger;
//        readonly IHostingEnvironment hostingEnvironment;
//        SwiftpassPayOptions options;
//        FinanceDbContext db;
//        PrintDbContext printDbContext;
//        WechatCoreDbContext wechatCoreDb;
//        ShopDbContext shopdb;
//        PayProxyFactory proxyFactory;
//        WechatTemplateSendOptions woptions;
//        public SwiftpassPayController(ICommunityService communityService
//            , IOptions<SwiftpassPayOptions> options
//            //   , FinanceDbContext db
//            ,PayProxyFactory proxyFactory
//            , WechatCoreDbContext wechatCoreDb
//            , PrintDbContext printDbContext
//            , ShopDbContext shopdb
//            , ILoggerFactory loggerFactory
//            , IOptions<WechatTemplateSendOptions> woptions

//            , IHostingEnvironment hostingEnvironment)
//        {
//            this.hostingEnvironment = hostingEnvironment;
//            this.woptions = woptions.Value;
//            this.options = options.Value;
//            this.db = ZRui.Web.BLL.DbContextFactory.FinanceDbContext;
//            this.shopdb = shopdb;
//            this.printDbContext = printDbContext;
//            this.wechatCoreDb = wechatCoreDb;
//            this.proxyFactory = proxyFactory;
//            _logger = loggerFactory.CreateLogger<SwiftpassPayController>();
//        }
//        [HttpPost]
//        public ActionResult Notify()
//        {
//            string body = new StreamReader(Request.Body).ReadToEnd();
//            byte[] requestData = Encoding.UTF8.GetBytes(body);
//            Stream inputStream = new MemoryStream(requestData);
//            using (StreamReader sr = new StreamReader(inputStream))
//            {
//                var xml = sr.ReadToEnd();

//                ZRui.Web.BLL.DbContextFactory.LogDbContext.Add<TaskLog>(new TaskLog() { AddTime = DateTime.Now, ExeResult = xml });
//                ZRui.Web.BLL.DbContextFactory.LogDbContext.SaveChanges();

//                var notify = new SwiftpassPayResponseHandler(options, xml);
//                ShopPayInfo shopPayInfo = shopdb.Query<ShopPayInfo>()
//                   .Where(m => !m.IsDel)
//                   .Where(m => m.AppId == notify.Appid)
//                   .Where(m => m.MchId == notify.Mchid)
//                   .FirstOrDefault();
//                if (shopPayInfo == null) return Content("failure1");

//                PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
//                if (notify.isTenpaySign())
//                {
//                    if (notify.Status == 0 && notify.ResultCode == 0 && notify.TradeState != "NOTPAY")
//                    {
//                        //此处可以在添加相关处理业务，校验通知参数中的商户订单号out_trade_no和金额total_fee是否和商户业务系统的单号和金额是否一致，一致后方可更新数据库表中的记录。  
//                        var rechange = db.Query<MemberTradeForRechange>()
//                            .Where(m => m.TradeNo == notify.OutTradeNo)
//                            .FirstOrDefault();
//                        if (rechange == null) throw new Exception("指定的OutTradeNo不存在");
//                        if (rechange.TotalFee != notify.TotalFee) throw new Exception("指定的金额不对应");

//                        var result = payProxy.GetPayResult(rechange);
//                        _logger.LogInformation($"获取到result{result.Xml}");

//                        //
//                        if (result.TradeState != "NOTPAY")
//                        {
//                            if (rechange.Status == MemberTradeForRechangeStatus.未完成)
//                            {

//                                if (rechange.TotalFee != result.TotalFee) throw new Exception("指定的金额不对应");
//                                rechange.OutBank = result.Xml;
//                                rechange.MechanismTradeNo = result.TransactionId;
//                                switch (result.TradeState)
//                                {
//                                    case "SUCCESS":
//                                        rechange.SetFinish(printDbContext, shopdb, db, woptions, _logger);
//                                        break;
//                                    case "CLOSED":
//                                        rechange.Status = MemberTradeForRechangeStatus.取消;
//                                        break;
//                                    default:
//                                        db.SetMemberTradeForRechangeFail(rechange, result.TradeState);
//                                        break;
//                                }
//                            }
//                        }
//                        db.SaveChanges();

//                        return Content("success");
//                    }
//                    else
//                    {
//                        return Content("failure1");
//                    }
//                }
//                else
//                {
//                    return Content("failure2");
//                }
//            }
//        }
//    }
//}
