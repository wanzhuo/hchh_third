using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.ShopManager.ShopMemberSetAPIModels
{

    public class GetMemberListArgsModel
    {
        public int? ShopId { get; set; }

    }

    public class GetBillModel
    {
        public decimal Amount { get; set; }
        public int BillType { get; set; }
        public DateTime BillDateTime { get; set; }
    }


    public class MemberListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public int Credits { get; set; }

        /// <summary>
        /// 最近消费记录（暂时不改变字段名字）
        /// </summary>
        public string AddTime { get; set; }

    }
}
