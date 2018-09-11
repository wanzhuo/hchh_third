using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
    public class ShopCustomTopUpSetModel : CreateModelBaseB
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
        /// 起充金额
        /// </summary>
        public int StartAmount
        {
            get
            {
                try
                {
                    var StartAmountP = double.Parse(StartAmountM);
                    return (int)(StartAmountP * 100);
                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        public string StartAmountM { get; set; }

        /// <summary>
        /// 自定义充值金额
        /// </summary>
        public int MeetAmount
        {
            get
            {
                try
                {
                    var MeetAmountP = double.Parse(MeetAmountM);
                    return (int)(MeetAmountP * 100);
                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }

        public string MeetAmountM { get; set; }

        public double Additional
        {
            get
            {
                try
                {
                    return
              double.Parse(AdditionalS);
                }
                catch (Exception)
                {

                    return 0;
                }


            }
        }

        /// <summary>
        /// 额外赠送（百分比）
        /// </summary>
        public string AdditionalS { get; set; }

    }
}
