using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{

    /// <summary>
    /// 会员设置
    /// </summary>
    public class UpadteMemberSetting
    {

        /// <summary>
        /// 固定充值
        /// </summary>
        public List<TopUpSetModel> topUpSetModels { get; set; }


        /// <summary>
        /// 自定义充值
        /// </summary>
        public ShopCustomTopUpSetModel shopCustomTopUpSet { get; set; }

        /// <summary>
        /// 会员设置项
        /// </summary>
        public ShopMemberSetModel shopMemberSetModel { get; set; }

        /// <summary>
        ///会员等级更新
        /// </summary>
        public List<AddMemberLevelModel> Source { get; set; }
    }
}
