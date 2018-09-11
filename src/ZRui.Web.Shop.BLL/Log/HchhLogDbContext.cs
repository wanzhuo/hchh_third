using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.BLL.Log;

namespace ZRui.Web.BLL
{
    public class HchhLogDbContext : DbContext
    {
        public HchhLogDbContext(DbContextOptions<HchhLogDbContext> options)
                : base(options)
        {

        }

        public DbSet<TaskLog> TaskLog { get; set; }
        public DbSet<PayOrRefundLog> PayOrRefundLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TaskLog>().ToTable("tasklog");
            builder.Entity<PayOrRefundLog>().ToTable("payorrefundlog");
            base.OnModelCreating(builder);
        }

        public int AddTaskLog(TaskLog  log )
        {
            Add(log);
           return  SaveChanges();
        }
        public int AddPayOrRefundLog(PayOrRefundLog log)
        {
            Add(log);
            return SaveChanges();
        }
    }
}
