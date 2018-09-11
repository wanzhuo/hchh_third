namespace HuiChiHuiHe.WinService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class record1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shop",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShopBrandId = c.Int(nullable: false),
                        Logo = c.String(unicode: false),
                        Cover = c.String(unicode: false),
                        Name = c.String(unicode: false),
                        Flag = c.String(unicode: false),
                        UsePerUser = c.String(unicode: false),
                        AreaCode = c.String(unicode: false),
                        AreaText = c.String(unicode: false),
                        Address = c.String(unicode: false),
                        AddressGuide = c.String(unicode: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        GeoHash = c.String(unicode: false),
                        Tel = c.String(unicode: false),
                        OpenTime = c.String(unicode: false),
                        ScoreValue = c.Int(nullable: false),
                        Detail = c.String(unicode: false),
                        AddTime = c.DateTime(nullable: false, precision: 0),
                        AddUser = c.String(unicode: false),
                        AddIp = c.String(unicode: false),
                        IsShowApplets = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shop");
        }
    }
}
