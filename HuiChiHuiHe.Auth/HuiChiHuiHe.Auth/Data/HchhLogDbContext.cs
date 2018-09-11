using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuiChiHuiHe.Auth
{
    public class HchhLogDbContext : DbContext
    {
        public HchhLogDbContext(DbContextOptions<HchhLogDbContext> options)
                : base(options)
        {

        }

        public DbSet<TaskLog> TaskLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TaskLog>().ToTable("tasklog");
            base.OnModelCreating(builder);
        }

        public T AddLog<T>(T t) where T : class
        {
            this.Add<T>(t);
            SaveChanges();
            return t;
        }
    }
}
