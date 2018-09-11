using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web
{
    /// <summary>
    /// 客户金额缓存
    /// </summary>
    public partial class MemberAmountCache
    {
        public virtual Int32 Id { get; set; }
        public virtual Int32 MemberId { get; set; }
        public virtual Int64 AvailAmount { get; set; }
        public virtual Int64 TotalRecharge { get; set; }
    }
}