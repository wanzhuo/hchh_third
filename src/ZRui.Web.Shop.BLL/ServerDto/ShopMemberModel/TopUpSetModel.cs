using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
   public  class TopUpSetModel : CreateModelBaseB
    {
        /// <summary>
        /// 关联的商铺
        /// </summary>
        public Shop Shop { get; set; }

        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 固定充值金额
        /// </summary>
        public int FixationTopUpAmount { get { return (int)(FixationTopUpAmountM * 100); } }

        /// <summary>
        /// 赠送金额
        /// </summary>
        public int PresentedAmount { get { return (int)(PresentedAmountM * 100); } }


        /// <summary>
        /// 固定充值金额
        /// </summary>
        public decimal FixationTopUpAmountM { get; set; }

        /// <summary>
        /// 赠送金额
        /// </summary>
        public decimal PresentedAmountM { get; set; }
    }
}
