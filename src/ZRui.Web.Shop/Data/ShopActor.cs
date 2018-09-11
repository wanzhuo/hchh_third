using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺角色
    /// </summary>
    public class ShopActor : EntityBase
    {
        /// <summary>
        /// 关联的商铺
        /// </summary>
        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        /// <summary>
        /// 关联的商铺的Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联的MemberId
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public ShopActorType ActorType { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }
    /// <summary>
    /// 商铺角色的类型，目前只有超级管理员
    /// </summary>
    public enum ShopActorType
    {
        超级管理员 = 10000
    }
}
