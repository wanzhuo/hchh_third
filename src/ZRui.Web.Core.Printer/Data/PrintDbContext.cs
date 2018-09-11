using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Printer.Data
{
    public class PrintDbContext : DbContext
    {
        public PrintDbContext(DbContextOptions<PrintDbContext> options) : base(options)
        {
             
        }
        public DbSet<Printer> Printer { get; set; }
        public DbSet<PrintRecord> PrintRecord { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Printer>().ToTable("Printer");
            modelBuilder.Entity<PrintRecord>().ToTable("PrintRecord");
            modelBuilder.Entity<PrintModel>().ToTable("PrintModel");
            base.OnModelCreating(modelBuilder);
        }

    }
}
