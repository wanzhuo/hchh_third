using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    /// <summary>
    /// 拼团快递信息表
    /// </summary>
    public class ShopTemplateMessageInfo : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 已存在用户公众号中的模板Id
        /// </summary>
        public string TemplateId { get; set; }


        /// <summary>
        /// 模板标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 模板内容示例
        /// </summary>
        public string Example { get; set; }


        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }

    }



}
