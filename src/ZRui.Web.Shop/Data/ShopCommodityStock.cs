using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    /// <summary>
    /// 店铺的商品规格及价格
    /// </summary>
    public class ShopCommodityStock : EntityBase
    {
        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的店铺的Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联的Sku
        /// </summary>
        [ForeignKey("SkuId")]
        public ShopBrandCommoditySku Sku { get; set; }
        /// <summary>
        /// 关联的Sku的Id
        /// </summary>
        public int SkuId { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 成本价,单位是分
        /// </summary>
        public int CostPrice { get; set; }
        /// <summary>
        /// 销售价,单位是分
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 市场价,单位是分
        /// </summary>
        public int MarketPrice { get; set; }
    }

    /// <summary>
    /// 店铺的商品规格及价格的数据库扩展
    /// </summary>
    public static class ShopCommodityStockDbContextExtention
    {
        /// <summary>
        /// 添加（减少）库存
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="id">库存Id</param>
        /// <param name="count">变动的量，负数为减少</param>
        public static void AddStock(this DbContext db,int id,int count)
        {
            var stock = db.Query<ShopCommodityStock>()
                .Where(m => m.Id == id)
                .FirstOrDefault();
            if (stock == null) throw new Exception("库存不存在");
            stock.Stock += count;
        }
    }
}
