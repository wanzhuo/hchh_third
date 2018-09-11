using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Printer
{
    public class OrderInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 下单数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public double Money
        {
            get
            {
                return this.Price * this.Count;
            }
        }


        /// <summary>
        /// 套错内容信息
        /// </summary>
        public List<ComboConten> ComboConten { get; set; }
    }


    /// <summary>
    /// 套餐内容
    /// </summary>
    public class ComboConten
    {

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

    }
}
