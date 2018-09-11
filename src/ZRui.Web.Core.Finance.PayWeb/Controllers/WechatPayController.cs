using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using ZRui.Web.Core.Finance.WechatPay;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.BLL.Third;

namespace ZRui.Web.Controllers
{
    public class WechatPayController : Controller
    {
        private readonly ILogger _logger;
        readonly IHostingEnvironment hostingEnvironment;
        WechatTemplateSendOptions options;
        FinanceDbContext db;
        PrintDbContext printDbContext;
        WechatCoreDbContext wechatCoreDb;
        ShopDbContext shopDb;
        PayProxyFactory proxyFactory;
        ThirdConfig thirdConfig;
        public WechatPayController(ICommunityService communityService
            , IOptions<WechatTemplateSendOptions> options
             , IOptions<ThirdConfig> poptions
            , PayProxyFactory proxyFactory
            , FinanceDbContext db
            , WechatCoreDbContext wechatCoreDb
            , PrintDbContext printDbContext
            , ShopDbContext shopDb
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.options = options.Value;
            this.db = db;
            this.shopDb = shopDb;
            this.printDbContext = printDbContext;
            this.wechatCoreDb = wechatCoreDb;
            this.proxyFactory = proxyFactory;
            this.thirdConfig = poptions.Value;
            _logger = loggerFactory.CreateLogger<WechatPayController>();
        }

        public ActionResult Notify()
        {
            string body = new StreamReader(Request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputStream = new MemoryStream(requestData);
            using (StreamReader sr = new StreamReader(inputStream))
            {
                var xml = sr.ReadToEnd();
                var notify = new WechatPayResponseHandler(xml);
                ShopPayInfo shopPayInfo = shopDb.Query<ShopPayInfo>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.AppId == notify.Appid)
                    .Where(m => m.MchId == notify.Mchid)
                    .FirstOrDefault();
                if (shopPayInfo == null) return Fail();
                PayProxyBase payProxy = proxyFactory.GetProxy(shopPayInfo);
                if (notify.isTenpaySign(payProxy.MakeSign))
                {
                    if (notify.ReturnCode == "SUCCESS" && notify.ResultCode == "SUCCESS")
                    {
                        try
                        {
                            //此处可以在添加相关处理业务，校验通知参数中的商户订单号out_trade_no和金额total_fee是否和商户业务系统的单号和金额是否一致，一致后方可更新数据库表中的记录。  
                            var rechange = db.Query<MemberTradeForRechange>()
                                .Where(m => m.TradeNo == notify.OutTradeNo)
                                .FirstOrDefault();
                            if (rechange == null) return Fail();
                            if (rechange.TotalFee != notify.TotalFee) return Fail();

                            var result = payProxy.GetPayResult(rechange);
                            if(result.ReturnCode == "SUCCESS" && result.ResultCode == "SUCCESS"
                                         && result.TradeState == "SUCCESS")
                            {
                                rechange.SetFinish(printDbContext,thirdConfig, shopDb, db, options, result, _logger);
                                db.SaveChanges();
                                shopDb.SaveChanges();
                                return Success();
                            }
                            else
                            {
                                _logger.LogError("微信支付通知错误,查询支付结果：{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));
                                return Fail();
                            }

                        }
                        catch (Exception e)
                        {
                            _logger.LogError("微信支付通知错误：{0}\n{1}", e.Message, e.StackTrace);
                            return Fail();
                        }

                    }
                    return Fail();
                }
                else
                {
                    return Fail();
                }
            }
        }


        ContentResult Success()
        {
            return Content("<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>");
        }

        ContentResult Fail()
        {
            return Content("<xml><return_code><![CDATA[FAIL]]></return_code></xml>");
        }
    }
}
