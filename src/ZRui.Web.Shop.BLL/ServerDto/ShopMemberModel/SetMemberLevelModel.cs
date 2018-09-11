using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
    /// <summary>
    /// 添加等级Model
    /// </summary>
    public class AddMemberLevelModel: CreateModelBaseB
    {
    

      

        public Shop Shop { get; set; }
        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public string MemberLevel { get; set; }

        /// <summary>
        /// 最高积分
        /// </summary>
        public int MaxIntegral { get; set; }

        /// <summary>
        /// 最低积分
        /// </summary>
        public int MinIntegral { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
