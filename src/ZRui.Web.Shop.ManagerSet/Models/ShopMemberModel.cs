using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{
    public class ShopMemberModel
    {
         public int Id { get; set; }
        /// <summary>
        /// 关联会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Credits { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal BalanceM { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }


        /// <summary>
        /// 等级信息
        /// </summary>
        public string LevelInfo { get; set; }
    }

    public class ExportMemberListModel
    {
        public int Id { get; set; }
        public string 姓名 { get; set; }
        public string 等级 { get; set; }
        public int 积分 { get; set; }
        public decimal 余额 { get; set; }
        public string 手机号 { get; set; }
        public string 生日 { get; set; }
    }

}
