using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团类型表
    /// </summary>
    public class ConglomerationActivityType : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 拼团人数
        /// </summary>
        public int ConglomerationMembers { get; set; }
        /// <summary>
        /// 拼团价格
        /// </summary>
        public int ConglomerationPrice { get; set; }

        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }

        /// <summary>
        /// 拼团类型描述
        /// </summary>
        public string TypeDescribe { get; set; }

        /// <summary>
        /// 关联拼团活动
        /// </summary>
        [JsonIgnore]
        [ForeignKey("ConglomerationActivityId")]
        public virtual ConglomerationActivity ConglomerationActivity { get; set; }

    }
}
