using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺评论
    /// </summary>
    public class ShopComment : EntityBase
    {
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺的Id
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 关联的MemberId
        /// </summary>
        public int? MemberId { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public decimal Grade { get; set; }
        /// <summary>
        /// 评论图片
        /// </summary>
        public int? PictureId1 { get; set; }
        public int? PictureId2 { get; set; }
        public int? PictureId3 { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }
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
