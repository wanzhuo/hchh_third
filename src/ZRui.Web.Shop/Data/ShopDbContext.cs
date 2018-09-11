using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZRui.Web;
using ZRui.Web.Data;

namespace ZRui.Web
{
    /// <summary>
    /// 商铺数据库上下文
    /// </summary>
    public class ShopDbContext: DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options)
        { }

    
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopBrand> ShopBrands { get; set; }
        public DbSet<ShopBrandCommodity> ShopBrandCommoditys { get; set; }
        public DbSet<ShopBrandCommodityParameter> ShopBrandCommodityParameters { get; set; }
        public DbSet<ShopBrandCommodityParameterValue> ShopBrandCommodityParameterValues { get; set; }
        public DbSet<ShopBrandCommoditySku> ShopBrandCommoditySkus { get; set; }
        public DbSet<ShopBrandCommoditySkuItem> ShopBrandCommoditySkuItems { get; set; }
        public DbSet<ShopCommodityStock> ShopCommoditySku { get; set; }
        public DbSet<ShopBrandCommodityCategory> ShopBrandCommodityCategorys { get; set; }
        public DbSet<ShopCallingQueue> ShopCallingQueues { get; set; }
        public DbSet<ShopCallingQueueProduct> ShopCallingQueueProducts { get; set; }
        public DbSet<SettingBase> SettingBases { get; set; }
        public DbSet<CommercialDistrict> CommercialDistricts { get; set; }
        public DbSet<CommercialDistrictShop> CommercialDistrictShops { get; set; }
        public DbSet<ShopBooking> ShopBookings { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }
        public DbSet<ShopActor> ShopActors { get; set; }
        public DbSet<ShopBrandActor> ShopBrandActors { get; set; }

        public DbSet<ShopWechatOpenAuthorizer> ShopWechatOpenAuthorizers { get; set; }
        public DbSet<WechatOpenAuthorizer> WechatOpenAuthorizers { get; set; }

        public DbSet<MemberAmount> MemberAmounts { get; set; }
        public DbSet<MemberAmountCache> MemberAmountCaches { get; set; }
        public DbSet<MemberAmountChangeLog> MemberAmountChangeLogs { get; set; }
        public DbSet<PlatformAmount> PlatformAmounts { get; set; }
        public DbSet<PlatformAmountChangeLog> PlatformAmountChangeLogs { get; set; }
        public DbSet<Member> Member { get; set; }

        public DbSet<MemberLogin> MemberLogins { get; set; }

        public DbSet<ShopPart> ShopParts { get; set; }
        public DbSet<ShopBrandFollower> ShopBrandFollowers { get; set; }
        public DbSet<ShopBrandArticle> ShopBrandArticles { get; set; }

        public DbSet<ArticleCategory> ArticleCategorys { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ShopComment> ShopComments { get; set; }
        public DbSet<ShopCommentPicture> ShopCommentPictures { get; set; }
        public DbSet<ShopPayInfo> ShopPayInfo { get; set; }
        public DbSet<ShopOrderReceiver> ShopOrderReceivers { get; set; }
        public DbSet<ShopOrderMoneyOff> ShopOrderMoneyOffs { get; set; }
        public DbSet<ShopOrderMoneyOffCache> ShopOrderMoneyOffCaches { get; set; }
        public DbSet<ShopOrderMoneyOffRule> ShopOrderMoneyOffRules { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }
        public DbSet<ShopTakeOutInfo> ShopTakeOutInfo { get; set; }
        public DbSet<ShopOrderOtherFee> ShopOrderOtherFees { get; set; }
        public DbSet<ShopBrandCombo> ShopCommodityCombos { get; set; }
        public DbSet<ShopBrandFixComboItem> ShopCommodityComboItems { get; set; }
        public DbSet<ShopOrderItemCombo> ShopOrderItemCombos { get; set; }
        public DbSet<MemberInvoiceTitle> MemberInvoiceTitle { get; set; }
        public DbSet<MemberInvoiceRequest> MemberInvoiceRequest { get; set; }
        public DbSet<ShopComboCategory> ShopComboCategory { get; set; }
        public DbSet<ShopOrderTakeout> ShopOrderTakeouts { get; set; }
        public DbSet<ShopOrderComboItem> ShopOrderComboItems { get; set; }
        public DbSet<CodeForShopOrderSelfHelp> CodeForShopOrderSelfHelps { get; set; }
        public DbSet<ShopSelfHelpInfo> ShopSelfHelpInfos { get; set; }

        public DbSet<BannerConfiguration> BannerConfiguration { get; set; }

        public DbSet<ThirdShop> ThirdShop { get; set; }
        public DbSet<Merchant> Merchant { get; set; }

        public DbSet<ThirdOrder> ThirdOrder { get; set; }

        public DbSet<ThirdRecharge> ThirdRecharge { get; set; }
        public DbSet<ThirdApiData> ThirdApiData { get; set; }

        public DbSet<ThirdMoneyReport> ThirdMoneyReport { get; set; }
        public DbSet<ShopMemberRufund> ShopMemberRufunds { get; set; }


        public DbSet<ConglomerationActivity> ConglomerationActivity { get; set; }
        public DbSet<ConglomerationActivityType> ConglomerationActivityType { get; set; }
        public DbSet<ConglomerationCommodity> ConglomerationCommodity { get; set; }
        public DbSet<ConglomerationOrder> ConglomerationOrder { get; set; }
        public DbSet<ConglomerationSetUp> ConglomerationSetUp { get; set; }
        public DbSet<ConglomerationActivityPickingSetting> ConglomerationActivityPickingSetting { get; set; }

        public DbSet<MemberPayRecording> MemberPayRecording { get; set; }
        public DbSet<ConglomerationParticipation> ConglomerationParticipation { get; set; }
        public DbSet<ConglomerationExpress> ConglomerationExpress { get; set; }
        public DbSet<ShopMember> ShopMembers { get; set; }

        public DbSet<SwiftpassKey> SwiftpassKey { get; set; }

        public DbSet<MemberTradeForRechange> MemberTradeForRechange { get; set; }
        public DbSet<ShopTemplateMessageInfo> ShopTemplateMessageInfo { get; set; }
        public DbSet<ShopMemberLevel> ShopMemberLevel { get; set; }
        public DbSet<ShopMemberSet> ShopMemberSet { get; set; }
        public DbSet<ShopCustomTopUpSet> ShopCustomTopUpSet { get; set; }
        public DbSet<ShopTopUpSet> ShopTopUpSet { get; set; }
        public DbSet<ShopMemberRecharge> ShopMemberRecharges { get; set; }
        public DbSet<ShopMemberConsume> ShopMemberConsumes { get; set; }

        public DbSet<SystemVersion> SystemVersion { get; set; }
        public DbSet<ShopMemberCardInfo> ShopMemberCardInfo { get; set; }
        public DbSet<ShopIntegralRecharge> ShopIntegralRecharge { get; set; }
        public DbSet<ShopServiceUserInfo> ShopServiceUserInfo { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Shop>().ToTable("Shop");
            builder.Entity<ShopBrand>().ToTable("ShopBrand");
            builder.Entity<ShopBrandCommodity>().ToTable("ShopBrandCommodity");
            builder.Entity<ShopBrandCommodityParameter>().ToTable("ShopBrandCommodityParameter");
            builder.Entity<ShopBrandCommodityParameterValue>().ToTable("ShopBrandCommodityParameterValue");
            builder.Entity<ShopBrandCommoditySku>().ToTable("ShopBrandCommoditySku");
            builder.Entity<ShopBrandCommoditySkuItem>().ToTable("ShopBrandCommoditySkuItem");
            builder.Entity<ShopCommodityStock>().ToTable("ShopCommodityStock");
            builder.Entity<ShopBrandCommodityCategory>().ToTable("ShopBrandCommodityCategory");
            builder.Entity<ShopCallingQueue>().ToTable("ShopCallingQueue");
            builder.Entity<ShopCallingQueueProduct>().ToTable("ShopCallingQueueProduct");
            builder.Entity<SettingBase>().ToTable("SettingBase");
            builder.Entity<CommercialDistrict>().ToTable("CommercialDistrict");
            builder.Entity<CommercialDistrictShop>().ToTable("CommercialDistrictShop");
            builder.Entity<ShopBooking>().ToTable("ShopBooking");
            builder.Entity<ShopOrder>().ToTable("ShopOrder");
            builder.Entity<ShopOrderItem>().ToTable("ShopOrderItem");
            builder.Entity<ShopActor>().ToTable("ShopActor");
            builder.Entity<ShopBrandActor>().ToTable("ShopBrandActor");
            builder.Entity<ShopWechatOpenAuthorizer>().ToTable("ShopWechatOpenAuthorizer");
            builder.Entity<WechatOpenAuthorizer>().ToTable("WechatOpenAuthorizer");

            builder.Entity<MemberAmount>().ToTable("MemberAmount").Property(m => m.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Entity<MemberAmountCache>().ToTable("MemberAmountCache");
            builder.Entity<MemberAmountChangeLog>().ToTable("MemberAmountChangeLog");
            builder.Entity<PlatformAmount>().ToTable("PlatformAmount").Property(m => m.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Entity<PlatformAmountChangeLog>().ToTable("PlatformAmountChangeLog");

            builder.Entity<MemberLogin>().ToTable("MemberLogin");

            builder.Entity<ShopPart>().ToTable("ShopPart");

            builder.Entity<ShopBrandFollower>().ToTable("ShopBrandFollower");

            builder.Entity<ShopBrandArticle>().ToTable("ShopBrandArticle");
            builder.Entity<ArticleCategory>().ToTable("ArticleCategory");
            builder.Entity<Article>().ToTable("Article");
            builder.Entity<ShopComment>().ToTable("ShopComment");
            builder.Entity<ShopCommentPicture>().ToTable("ShopCommentPicture");
            builder.Entity<ShopPayInfo>().ToTable("ShopPayInfo");
            builder.Entity<ShopOrderReceiver>().ToTable("ShopOrderReceiver");
            builder.Entity<ShopOrderMoneyOff>().ToTable("ShopOrderMoneyOff");
            builder.Entity<ShopOrderMoneyOffRule>().ToTable("ShopOrderMoneyOffRule");
            builder.Entity<ShopOrderMoneyOffCache>().ToTable("ShopOrderMoneyOffCache");
            builder.Entity<MemberAddress>().ToTable("MemberAddress");
            builder.Entity<ShopTakeOutInfo>().ToTable("ShopTakeOutInfo");
            builder.Entity<ShopOrderOtherFee>().ToTable("ShopOrderOtherFee");
            builder.Entity<ShopBrandCombo>().ToTable("ShopCommodityCombo");
            builder.Entity<ShopBrandFixComboItem>().ToTable("ShopCommodityComboItem");
            builder.Entity<ShopOrderItemCombo>().ToTable("ShopOrderItemCombo");
            builder.Entity<MemberInvoiceTitle>().ToTable("MemberInvoiceTitle");
            builder.Entity<MemberInvoiceRequest>().ToTable("MemberInvoiceRequest");
            builder.Entity<ShopComboCategory>().ToTable("ShopComboCategory");
            builder.Entity<BannerConfiguration>().ToTable("BannerConfiguration");

            builder.Entity<ThirdShop>().ToTable("ThirdShop");
            builder.Entity<ThirdApiData>().ToTable("ThirdApiData");
            builder.Entity<ThirdRecharge>().ToTable("ThirdRecharge");
            builder.Entity<Merchant>().ToTable("Merchant");
            builder.Entity<ThirdOrder>().ToTable("ThirdOrder");
            builder.Entity<ThirdMoneyReport>().ToTable("ThirdMoneyReport");
            builder.Entity<ShopOrderTakeout>().ToTable("ShopOrderTakeout");
            builder.Entity<CodeForShopOrderSelfHelp>().ToTable("CodeForShopOrderSelfHelp");

            builder.Entity<ConglomerationActivity>().ToTable("ConglomerationActivity");
            builder.Entity<ConglomerationActivityType>().ToTable("ConglomerationActivityType");
            builder.Entity<ConglomerationCommodity>().ToTable("ConglomerationCommodity");
            builder.Entity<ConglomerationOrder>().ToTable("ConglomerationOrder");
            builder.Entity<ConglomerationSetUp>().ToTable("ConglomerationSetUp");
            builder.Entity<ShopOrderComboItem>().ToTable("ShopOrderComboItem");
 			builder.Entity<MemberPayRecording>().ToTable("MemberPayRecording");
            builder.Entity<ConglomerationActivityPickingSetting>().ToTable("ConglomerationActivityPickingSetting");
            builder.Entity<ConglomerationParticipation>().ToTable("ConglomerationParticipation");
            builder.Entity<ConglomerationExpress>().ToTable("ConglomerationExpress");
            builder.Entity<ShopSelfHelpInfo>().ToTable("ShopSelfHelpInfo");
            builder.Entity<Member>().ToTable("Member");
            builder.Entity<SwiftpassKey>().ToTable("SwiftpassKey");
            builder.Entity<MemberTradeForRechange>().ToTable("MemberTradeForRechange");
            builder.Entity<ShopTemplateMessageInfo>().ToTable("ShopTemplateMessageInfo");
            builder.Entity<ShopMember>().ToTable("ShopMember");
            builder.Entity<ShopMemberLevel>().ToTable("ShopMemberLevel");
            builder.Entity<ShopMemberSet>().ToTable("ShopMemberSet");
            builder.Entity<ShopMemberRecharge>().ToTable("ShopMemberRecharge");
            builder.Entity<ShopMemberConsume>().ToTable("ShopMemberConsume");
            builder.Entity<ShopMemberRufund>().ToTable("ShopMemberRufund");

            builder.Entity<ShopCustomTopUpSet>().ToTable("ShopCustomTopUpSet");
            builder.Entity<ShopTopUpSet>().ToTable("ShopTopUpSet");

            builder.Entity<SystemVersion>().ToTable("SystemVersion");
            builder.Entity<ShopMemberCardInfo>().ToTable("ShopMemberCardInfo");
            builder.Entity<ShopIntegralRecharge>().ToTable("ShopIntegralRecharge");
            builder.Entity<ShopServiceUserInfo>().ToTable("ShopServiceUserInfo");

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
