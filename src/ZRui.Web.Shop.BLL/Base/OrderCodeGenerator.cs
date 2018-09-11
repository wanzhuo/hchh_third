using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web.BLL
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderCategory
    {
        /// <summary>
        /// 自助点餐
        /// </summary>
        SelfOrder = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        ScanCode = 2,
        /// <summary>
        /// 外卖
        /// </summary>
        Takeout = 3,
        /// <summary>
        /// 预约
        /// </summary>
        Subscribe = 4,
        /// <summary>
        /// 拼团
        /// </summary>
        Conglomeration=5

    }
    public class OrderCodeGenerator
    {
        /// <summary>
        /// 产生订单编码
        /// </summary>
        /// <param name="category">订单类型</param>
        /// <param name="shopId">商户id</param>
        /// <returns></returns>
        public static string Generate(OrderCategory category, int shopId)
        {
            if (shopId > 99999)
            {
                throw new Exception("商户id不能超过6位");
            }
            string categoryStr = ((int)category).ToString("00");
            string shopIdStr = shopId.ToString("000000");
            string timeStr = DateTime.Now.ToString("yyMMddhh");
            int count = 0;
            ShopDbContext shopDb = DbContextFactory.ShopDb;
            if (category == OrderCategory.Conglomeration)
            {
                count = shopDb.ConglomerationOrder.Where(p => p.ShopId == shopId && p.CreateTime >= DateTime.Today).Count();
            }
            else
            {               
                count = shopDb.ShopOrders.Where(p => p.ShopId == shopId && p.AddTime >= DateTime.Today).Count();
            }
            string countStr = (count+1).ToString("0000");
            string randomStr = new Random().Next(999).ToString("000");

            return categoryStr + shopIdStr + timeStr + countStr + randomStr;

        }
    }
}
