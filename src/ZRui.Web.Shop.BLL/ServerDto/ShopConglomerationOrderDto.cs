using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.BLL.ServerDto
{
   public class ShopConglomerationOrderDto
    {

        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }

        /// <summary>
        /// 已发起的拼团FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }

        /// <summary>
        /// 关联的商铺fK_Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 配送地址Fk_Id
        /// </summary>
        public int? MemberAddressId { get; set; }


        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 以发起成团
        /// </summary>
        public  ConglomerationSetUp ConglomerationSetUp { get; set; }


        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 自提配送时间
        /// </summary>
        public DateTime? Delivery { get; set; }



        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        /// <summary>
        /// 小程序提交的formId
        /// </summary>
        public string FormId { get; set; }

    }
}
