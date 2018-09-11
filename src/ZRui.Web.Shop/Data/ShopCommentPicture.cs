using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺评论
    /// </summary>
    public class ShopCommentPicture : EntityBase
    {
        /// <summary>
        /// 关联的商铺评论的Id
        /// </summary>
        public int? ShopCommentId { get; set; }
        /// <summary>
        /// 图片名字
        /// </summary>
        public string SaveFileName { get; set; }
        /// <summary>
        /// 关联的MemberId
        /// </summary>
        public int? MemberId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }
}
