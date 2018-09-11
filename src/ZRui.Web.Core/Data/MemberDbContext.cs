using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    public class MemberDbContext: DbContext
    {
        public MemberDbContext(DbContextOptions<MemberDbContext> options)
            : base(options)
        { }

        MemberDbContext(DbContextOptions options)
           : base(options)
        { }

        public DbSet<Member> Members { get; set; }
        public DbSet<MemberPhone> MemberPhones { get; set; }
        public DbSet<MemberSMSValiCodeTask> MemberSMSValiCodeTasks { get; set; }
        public DbSet<MemberLogin> MemberLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().ToTable("Member");
            builder.Entity<MemberPhone>().ToTable("MemberPhone");
            builder.Entity<MemberSMSValiCodeTask>().ToTable("MemberSMSValiCodeTask");
            builder.Entity<MemberLogin>().ToTable("MemberLogin");

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public static MemberDbContext Create(DbContextOptions options)
        {
            return new MemberDbContext(options);
        }
    }
}
