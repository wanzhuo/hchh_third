using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Printer.Data;
using static ZRui.Web.Models.SwiftpassPayModel;
using ZRui.Web.BLL;
using ZRui.Web.Core.Finance.SwiftpassPay;
using ZRui.Web.BLL.Servers;
using System.Threading.Tasks;
using ZRui.Web.BLL.Third;

namespace ZRui.Web.Controllers
{

    public class SwiftpassPayController : Controller
    {

        private readonly ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        // SwiftpassPayOptions options;
        FinanceDbContext db;
        PrintDbContext printDbContext;
        WechatCoreDbContext wechatCoreDb;
        ShopDbContext shopdb;
        PayProxyFactory proxyFactory;
        WechatTemplateSendOptions woptions;
        MemberDbContext _memberDbContext;
        ShopConglomerationOrderOptions _shopConglomerationOrderServer;
        ThirdConfig thirdConfig;
        public SwiftpassPayController(ICommunityService communityService
            //, IOptions<SwiftpassPayOptions> options
            , FinanceDbContext db
            , IOptions<ShopConglomerationOrderOptions> shopConglomerationOrderServer
            , PayProxyFactory proxyFactory
            , WechatCoreDbContext wechatCoreDb
            , PrintDbContext printDbContext
            , ShopDbContext shopdb
            , MemberDbContext memberDbContext
            , ILoggerFactory loggerFactory
            , IOptions<WechatTemplateSendOptions> woptions
            , IOptions<ThirdConfig> toptions


            , IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.woptions = woptions.Value;
            //this.options = options.Value;
            // this.db = ZRui.Web.BLL.DbContextFactory.FinanceDbContext;
            this.db = db;
            this.shopdb = shopdb;
            this.printDbContext = printDbContext;
            this.wechatCoreDb = wechatCoreDb;
            this.proxyFactory = proxyFactory;
            this._memberDbContext = memberDbContext;

            _shopConglomerationOrderServer = shopConglomerationOrderServer.Value;
            _logger = loggerFactory.CreateLogger<SwiftpassPayController>();
            this.thirdConfig = toptions.Value;
        }


        public ActionResult Notify()
        {
            _logger.LogInformation("=====================支付回调开始======================");

            //var logDb = ZRui.Web.BLL.DbContextFactory.LogDbContext;
            //logDb.Add<TaskLog>(new TaskLog() { AddTime = DateTime.Now, TaskName = "进入Notify" });

            string body = new StreamReader(Request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputStream = new MemoryStream(requestData);


            using (StreamReader sr = new StreamReader(inputStream))
            {
                try
                {
                    var xml = sr.ReadToEnd();
                    _logger.LogInformation($"获取到result{xml}");
                    //logDb.Add<TaskLog>(new TaskLog() { AddTime = DateTime.Now, TaskName = "Notify", ExeResult = xml });
                    //logDb.SaveChanges();


                    var notify = new SwiftpassPayResponseHandler(null, xml);
                    ShopPayInfo shopPayInfo = shopdb.Query<ShopPayInfo>()
                       .Where(m => !m.IsDel)
                       .Where(m => m.AppId == notify.SwiftpassAppid)
                       .Where(m => m.MchId == notify.SwiftpassMchid)
                       .FirstOrDefault();
                    if (shopPayInfo == null) return Content("failure1");

                    PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
                    // if (notify.isTenpaySign())
                    //{
                    if (notify.Status == 0 && notify.ResultCode == 0 && notify.TradeState != "NOTPAY")
                    {

                        _logger.LogInformation("支付成功");

                        //此处可以在添加相关处理业务，校验通知参数中的商户订单号out_trade_no和金额total_fee是否和商户业务系统的单号和金额是否一致，一致后方可更新数据库表中的记录。  
                        var rechange = db.Query<MemberTradeForRechange>()
                            .Where(m => m.TradeNo == notify.OutTradeNo)
                            .FirstOrDefault();
                        if (rechange == null) throw new Exception("指定的OutTradeNo不存在");
                        if (rechange.TotalFee != notify.TotalFee) throw new Exception("指定的金额不对应");
                        rechange.OutBank = notify.Xml;
                        rechange.MechanismTradeNo = notify.TransactionId;
                        if (notify.getAllParameters()["result_code"].ToString() == "0")
                        {
                            rechange.SetFinish(printDbContext, shopdb, db, woptions, thirdConfig, _logger);
                            db.SaveChanges();
                            shopdb.SaveChanges();
                            _logger.LogInformation("回调成功返回success");
                            if (rechange.ConglomerationOrderId.HasValue)
                            {
                                ShopConglomerationOrderOptions.MemberInform(shopdb, _memberDbContext, rechange.ConglomerationOrderId.Value, _logger);
                                ShopConglomerationActivityOptions.NotifyOkRemoveList(rechange, shopdb, _logger);
                            }
                            _logger.LogInformation("=====================支付回调结束======================");
                            return Content("success");
                        }
                        else
                        {
                            _logger.LogInformation("回调失败返回failure1");
                            _logger.LogInformation("=====================支付回调结束======================");
                            return Content("failure1");
                        }
                        //var result = payProxy.GetPayResult(rechange);
                        //_logger.LogInformation($"获取到result{result.Xml}");

                        //
                        //if (result.TradeState != "NOTPAY")
                        //{
                        //    if (rechange.Status == MemberTradeForRechangeStatus.未完成)
                        //    {

                        //        if (rechange.TotalFee != result.TotalFee) throw new Exception("指定的金额不对应");
                        //        rechange.OutBank = result.Xml;
                        //        rechange.MechanismTradeNo = result.TransactionId;
                        //        switch (result.TradeState)
                        //        {
                        //            case "SUCCESS":
                        //                rechange.SetFinish(printDbContext, shopdb, db, woptions, _logger);
                        //                break;
                        //            case "CLOSED":
                        //                rechange.Status = MemberTradeForRechangeStatus.取消;
                        //                break;
                        //            default:
                        //                db.SetMemberTradeForRechangeFail(rechange, result.TradeState);
                        //                break;
                        //        }
                        //    }
                        //}

                    }
                    else
                    {
                        _logger.LogInformation($"=====================支付回调结束======================");
                        return Content("failure1");
                    }
                    // }
                    //else
                    //{
                    //  return Content("failure2");
                    //}
                }
                catch (Exception e)
                {

                    _logger.LogInformation($"回调错误{e}");
                    return Content("failure1");
                }
            }
        }
    }
}
