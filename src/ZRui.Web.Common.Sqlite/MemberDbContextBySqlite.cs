using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Common
{
    public class MemberDbContextBySqlite : DbContext
    {
        static object _lockObject = new object();
        protected IHostingEnvironment hostingEnvironment;
        string path;

        public MemberDbContextBySqlite(IHostingEnvironment hostingEnvironment, int memberId, string dbName)
        {
            this.hostingEnvironment = hostingEnvironment;

            this.path = System.IO.Path.Combine(this.hostingEnvironment.ContentRootPath, "App_Data", "Members", $"member{memberId}", $"{dbName}.db");
            if (!System.IO.File.Exists(this.path))
            {
                lock (_lockObject)
                {
                    if (!System.IO.File.Exists(this.path))
                    {
                        var sourcePath = System.IO.Path.Combine(this.hostingEnvironment.ContentRootPath, "App_Data", $"{dbName}.db");
                        var distDir = System.IO.Path.GetDirectoryName(this.path);
                        if (!System.IO.Directory.Exists(distDir))
                        {
                            System.IO.Directory.CreateDirectory(distDir);
                        }
                        System.IO.File.Copy(sourcePath, this.path);
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(string.Format("Data Source={0}", path));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
