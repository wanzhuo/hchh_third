using Microsoft.EntityFrameworkCore;

namespace HuiChiHuiHe.Auth
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options)
         : base(options)
        { }


        public DbSet<WechatOpenAuthorizer> WechatOpenAuthorizer { get; set; }
        public DbSet<WechatOpenAuthorizer> ShopWechatOpenAuthorizer { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ShopWechatOpenAuthorizer>().ToTable("ShopWechatOpenAuthorizer");
            builder.Entity<WechatOpenAuthorizer>().ToTable("WechatOpenAuthorizer");
            base.OnModelCreating(builder);
        }
    }
}
