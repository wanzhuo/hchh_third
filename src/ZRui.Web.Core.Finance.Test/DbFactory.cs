using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Finance.WechatPay;

namespace ZRui.Web.Core.Finance.Test
{

    public static class TShopDbContxtFactory
    {
        public static ShopDbContext MarkShopDb()
        {
            DbContextOptionsBuilder<ShopDbContext> builder = new DbContextOptionsBuilder<ShopDbContext>();
            builder.UseMySql("Server=120.79.31.209;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;");
            return new ShopDbContext(builder.Options);
        }

        public static FinanceDbContext MarkFinanceDb()
        {
            DbContextOptionsBuilder<FinanceDbContext> builder = new DbContextOptionsBuilder<FinanceDbContext>();
            builder.UseMySql("Server=120.79.31.209;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;");
            return new FinanceDbContext(builder.Options);
        }

    }


    public class FactoryUtil
    {
        public static PayProxyFactory GetPayProxyFactory()
        {
            WechatPayOptions wechatPayOptions = new WechatPayOptions()
            {
                OrderUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder",
                OrderQueryUrl = "https://api.mch.weixin.qq.com/pay/orderquery",
                NotifyUrl = "https://wxapi.91huichihuihe.com/WechatPay/Notify"
            };
            return new PayProxyFactory(wechatPayOptions, null, new TestLoggerFactory());

        }
    }

}
