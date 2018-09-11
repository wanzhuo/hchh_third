using System;
using ZRui.Web.BLL.ServerDto;

namespace ZRui.Web.ShopMemberTopUpAPIModel
{



    public class GetTopUpModel : ShopIdModel
    {

        public int Id { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 固定充值金额
        /// </summary>
        public decimal FixationTopUpAmountM { get; set; }

        /// <summary>
        /// 赠送金额
        /// </summary>
        public decimal PresentedAmountM { get; set; }
    }


    public class GetCustomTopUpModel : ShopIdModel
    {

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }



        /// <summary>
        /// 起充金额
        /// </summary>
        public decimal StartAmountM { get; set; }

        /// <summary>
        /// 满足金额
        /// </summary>
        public decimal MeetAmountM { get; set; }


        /// <summary>
        /// 额外赠送（百分比）
        /// </summary>
        public double Additional { get; set; }



        /// <summary>
        /// 是否启用自定义充值赠送
        /// </summary>
        public bool IsShowCustomTopUpSet { get; set; }
    }
}