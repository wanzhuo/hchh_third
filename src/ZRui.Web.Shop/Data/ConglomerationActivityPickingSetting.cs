using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团活动配货设置
    /// </summary>
    public class ConglomerationActivityPickingSetting : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 配送方式 1自提，2快递
        /// </summary>
        public ConsignmentType Type { get; set; }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }



       

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        [ForeignKey("ConglomerationActivityId")]
        public ConglomerationActivity ConglomerationActivity { get; set; }


        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string PickingSettingName { get; set; }



        

    }
}
