using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat
{
    public class WechatOpenCoreDbContext : DbContext
    {
        public WechatOpenCoreDbContext(DbContextOptions<WechatOpenCoreDbContext> options)
            : base(options)
        { }

        public DbSet<WechatOpenAuthorizer> WechatOpenAuthorizers { get; set; }
        public DbSet<WechatOpenTicket> WechatOpenTickets { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WechatOpenAuthorizer>().ToTable("WechatOpenAuthorizer");
            builder.Entity<WechatOpenTicket>().ToTable("WechatOpenTicket");
            base.OnModelCreating(builder);
        }
    }
}
