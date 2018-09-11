using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Common
{
    public class CommunityDbContextBySqlite : DbContext
    {
        static object _lockObject = new object();
        protected string communityFlag;
        protected string appFlag;
        protected IHostingEnvironment hostingEnvironment;
        string path;
        
        public CommunityDbContextBySqlite(IHostingEnvironment hostingEnvironment, string communityFlag, string appFlag, string dbName)
        {
            this.communityFlag = communityFlag;
            this.appFlag = appFlag;
            this.hostingEnvironment = hostingEnvironment;

            this.path = System.IO.Path.Combine(this.hostingEnvironment.ContentRootPath, "App_Data", "Communitys", communityFlag, $"{dbName}s", appFlag + ".db");
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
