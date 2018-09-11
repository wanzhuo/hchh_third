using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Core.Printer.Data;

namespace ZRui.Web.Core.Printer.Test
{

    public static class TPrintDbFactory
    {
        public static PrintDbContext MarkShopDb()
        {
            DbContextOptionsBuilder<PrintDbContext> builder = new DbContextOptionsBuilder<PrintDbContext>();
            builder.UseMySql("Server=47.106.166.191;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;");
            return new PrintDbContext(builder.Options);
        }

    }

    public static class TShopDbContxtFactory
    {
        public static ShopDbContext MarkShopDb()
        {
            DbContextOptionsBuilder<ShopDbContext> builder = new DbContextOptionsBuilder<ShopDbContext>();
            builder.UseMySql("Server=47.106.166.191;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;");
            return new ShopDbContext(builder.Options);
        }

    }


    public static class TPrintDbContextFactory
    {
        public static PrintDbContext MarkShopDb()
        {
            DbContextOptionsBuilder<PrintDbContext> builder = new DbContextOptionsBuilder<PrintDbContext>();
            builder.UseMySql("Server=47.106.166.191;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;");
            return new PrintDbContext(builder.Options);
        }

    }
}
