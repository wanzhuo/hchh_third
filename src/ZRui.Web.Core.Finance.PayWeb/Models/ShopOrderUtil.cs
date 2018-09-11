using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Third;
using ZRui.Web.BLL.Utils;
using ZRui.Web.Core.Finance.PayBase;
using ZRui.Web.Core.Wechat;
using Microsoft.Extensions.DependencyInjection;
using ZRui.Web.Core.Printer.Data;
using Microsoft.Extensions.Options;

namespace ZRui.Web.Utils
{
    internal class ShopOrderUtil
    {
        /// <summary>
        /// 在后台任务中完成订单
        /// </summary>
        /// <param name="shopOrderId"></param>
        public static void SetShopOrderFinishInBackgroup(int shopOrderId)
        {
            BackgroundUtil.Enqueue(()
                => SetShopOrderFinish(shopOrderId));
        }

        /// <summary>
        /// 在后台任务中完成订单
        /// </summary>
        /// <param name="shopOrderId"></param>
        public static void SetShopOrderFinish(int shopOrderId)
        {
            using (ShopDbContext shopDb = DbContextFactory.ShopDb)
            {
                using (var printDbContext = DbContextFactory.PrintDb)
                {
                    ILogger logger = ServiceLocator.Instance.GetService<ILoggerFactory>().CreateLogger<ShopOrderUtil>();
                    ShopOrder shopOrder = shopDb.GetSingle<ShopOrder>(shopOrderId);
                    ThirdConfig thirdConfig = ServiceLocator.Instance.GetService<IOptions<ThirdConfig>>().Value;
                    WechatTemplateSendOptions wechatTemplateSend = ServiceLocator.Instance.GetService<IOptions<WechatTemplateSendOptions>>().Value;
                    shopOrder.PayWay = "会员余额支付";
                    shopOrder.SetShopOrderFinish(printDbContext, shopDb, wechatTemplateSend, thirdConfig, logger);
                }
            }
        }
    }
}
