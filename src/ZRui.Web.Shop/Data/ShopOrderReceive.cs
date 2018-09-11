using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺订单下单成功接收者
    /// </summary>
    public class ShopOrderReceiver : EntityBase
    {
        /// <summary>
        /// ShopId
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// ShopFlag
        /// </summary>
        public string ReceiverOpenId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Headimgurl { get; set; }
        /// <summary>
        /// 是否使得
        /// </summary>
        public bool IsUsed { get; set; }
    }
}
