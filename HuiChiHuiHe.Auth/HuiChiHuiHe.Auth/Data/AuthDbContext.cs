using Microsoft.EntityFrameworkCore;

namespace HuiChiHuiHe.Auth
{
    public class AuthDbContext:DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
         : base(options)
        { }
        
        public DbSet<ComponentAuthorizer> ComponentAuthorizer { get; set; }
        public DbSet<WechatOpenAuthorizer> WechatOpenAuthorizer { get; set; }
        public DbSet<WechatOpenBind> WechatOpenBind { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WechatOpenBind>().ToTable("WechatOpenBind");
            builder.Entity<ComponentAuthorizer>().ToTable("ComponentAuthorizer");
            builder.Entity<WechatOpenAuthorizer>().ToTable("WechatOpenAuthorizer");
            base.OnModelCreating(builder);         
        }
    }
}
