using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
    /// <summary>
    /// 会员卡信息
    /// </summary>
   public class ShopMemberCardInfoModel : CreateModelBaseB
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
        /// 特权说明
        /// </summary>
        public string PrivilegeExplain { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime? ValidityBeginTime { get; set; }


        /// <summary>
        /// 有效结束日期
        /// </summary>
        public DateTime? ValidityEndTime { get; set; }


        /// <summary>
        /// 是否永久有效
        /// </summary>
        public bool IsValidityLong { get; set; }

        /// <summary>
        /// 使用需知
        /// </summary>
        public string UsedKnow { get; set; }

        /// <summary>
        /// 卡封面
        /// </summary>
        public string CardCover { get; set; }
        /// <summary>
        /// 服务电话
        /// </summary>
        public string ServePhone { get; set; }
    }
}
