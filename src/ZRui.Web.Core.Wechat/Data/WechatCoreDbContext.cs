using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat
{
    public class WechatCoreDbContext: DbContext
    {
        public WechatCoreDbContext(DbContextOptions<WechatCoreDbContext> options)
            : base(options)
        { }

        public DbSet<MemberWechat> MemberWechats { get; set; }
        public DbSet<WechatQRScene> WechatQRScenes { get; set; }
        public DbSet<RequestMsgData> RequestMsgDatas { get; set; }
        public DbSet<CustomerSession> CustomerSessions { get; set; }
        public DbSet<CustomerMessage> CustomerMessages { get; set; }
        public DbSet<MemberWeChatBindTask> MemberWeChatBindTasks { get; set; }
        public DbSet<MemberWeChatLoginTask> MemberWeChatLoginTasks { get; set; }
        public DbSet<CustomerPhone> CustomerPhones { get; set; }
        public DbSet<CustomerSmsValiCodeTask> CustomerSmsValiCodeTasks { get; set; }
        public DbSet<RobotMessage> RobotMessages { get; set; }

        public DbSet<MemberLogin> MemberLogins { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MemberWechat>().ToTable("MemberWechat");
            builder.Entity<WechatQRScene>().ToTable("WechatQRScene");
            builder.Entity<RequestMsgData>().ToTable("RequestMsgData");
            builder.Entity<CustomerSession>().ToTable("CustomerSession");
            builder.Entity<CustomerMessage>().ToTable("CustomerMessage");
            builder.Entity<MemberWeChatBindTask>().ToTable("MemberWeChatBindTask");
            builder.Entity<MemberWeChatLoginTask>().ToTable("MemberWeChatLoginTask");
            builder.Entity<CustomerPhone>().ToTable("CustomerPhone");
            builder.Entity<CustomerSmsValiCodeTask>().ToTable("CustomerSmsValiCodeTask");
            builder.Entity<RobotMessage>().ToTable("RobotMessage");

            builder.Entity<MemberLogin>().ToTable("MemberLogin");

            builder.Entity<ShopMember>().ToTable("ShopMember");
            builder.Entity<ConglomerationOrder>().ToTable("ConglomerationOrder");

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
