using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.Data;

namespace ZRui.Web
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        { }

        public DbSet<MemberAmount> MemberAmounts { get; set; }
        public DbSet<MemberAmountCache> MemberAmountCaches { get; set; }
        public DbSet<MemberAmountChangeLog> MemberAmountChangeLogs { get; set; }
        public DbSet<MemberTradeForRechange> MemberTradeForRechanges { get; set; }
        public DbSet<PlatformAmount> PlatformAmounts { get; set; }
        public DbSet<PlatformAmountChangeLog> PlatformAmountChangeLogs { get; set; }

        public DbSet<MemberTradeForRefund> memberTradeForRefunds { get; set; }

        public DbSet<OfflinePayRecording> OfflinePayRecording { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MemberAmount>().ToTable("MemberAmount").Property(m=>m.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Entity<MemberAmountCache>().ToTable("MemberAmountCache");
            builder.Entity<MemberAmountChangeLog>().ToTable("MemberAmountChangeLog");
            builder.Entity<MemberTradeForRechange>().ToTable("MemberTradeForRechange").Property(m => m.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Entity<PlatformAmount>().ToTable("PlatformAmount").Property(m => m.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Entity<PlatformAmountChangeLog>().ToTable("PlatformAmountChangeLog");
            builder.Entity<MemberTradeForRefund>().ToTable("MemberTradeForRefund");
            builder.Entity<OfflinePayRecording>().ToTable("OfflinePayRecording");
            base.OnModelCreating(builder);
        }
    }
}
