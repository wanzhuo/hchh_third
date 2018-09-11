﻿using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺品牌角色
    /// </summary>
    public class ShopBrandActor : EntityBase
    {
        /// <summary>
        /// 关联的商铺品牌
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// 关联的商铺品牌的Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// 关联的MemberId
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public ShopBrandActorType ActorType { get; set; }
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
    /// 商铺品牌的角色
    /// </summary>
    public enum ShopBrandActorType
    {
        超级管理员 = 10000
    }
}