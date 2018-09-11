using Microsoft.EntityFrameworkCore;
using ZRui.Web.Core.Printer.Data;

namespace ZRui.Web.BLL
{
    public class DbContextFactory
    {
        private static DbContextOptions<ShopDbContext> _ShopDbOptions = null;
        private static DbContextOptions<ShopDbContext> ShopDbOptions
        {
            get
            {
                if (_ShopDbOptions == null)
                {
                    string connectString = AppSetting.GetInstance().GetConfig("ConnectionStrings:ShopDbConnection");
                    _ShopDbOptions = new DbContextOptionsBuilder<ShopDbContext>().UseMySql(connectString).Options;
                }
                return _ShopDbOptions;
            }
        }      
        public static ShopDbContext ShopDb
        {
            get
            {
                return new ShopDbContext(ShopDbOptions);
            }
        }


        private static DbContextOptions<PrintDbContext> _PrintDbOptions = null;
        private static DbContextOptions<PrintDbContext> PrintDbOptions
        {
            get
            {
                if (_PrintDbOptions == null)
                {
                    string connectString = AppSetting.GetInstance().GetConfig("ConnectionStrings:ShopDbConnection");
                    _PrintDbOptions = new DbContextOptionsBuilder<PrintDbContext>().UseMySql(connectString).Options;
                }
                return _PrintDbOptions;
            }
        }
        public static PrintDbContext PrintDb
        {
            get
            {
                return new PrintDbContext(PrintDbOptions);
            }
        }


        private static DbContextOptions<HchhLogDbContext> _logDbOptions = null;
        private static DbContextOptions<HchhLogDbContext> LogDbOptions
        {
            get
            {
                if (_logDbOptions == null)
                {
                    string connectString = AppSetting.GetInstance().GetConfig("ConnectionStrings:LogDbConnection");
                    _logDbOptions = new DbContextOptionsBuilder<HchhLogDbContext>().UseMySql(connectString).Options;
                }
                return _logDbOptions;
            }
        }
        public static FinanceDbContext FinanceDbContext
        {
            get
            {
                return new FinanceDbContext(FinanceDbOptions);
            }
        }


        private static DbContextOptions<FinanceDbContext> _FinanceDbOptions = null;
        private static DbContextOptions<FinanceDbContext> FinanceDbOptions
        {
            get
            {
                if (_FinanceDbOptions == null)
                {
                    string connectString = AppSetting.GetInstance().GetConfig("ConnectionStrings:FinanceDbConnection");
                    _FinanceDbOptions = new DbContextOptionsBuilder<FinanceDbContext>().UseMySql(connectString).Options;
                }
                return _FinanceDbOptions;
            }
        }
        public static HchhLogDbContext LogDbContext
        {
            get
            {
                return new HchhLogDbContext(LogDbOptions);
            }
        }
























        private static DbContextOptions<AuthDbContext> _authDbOptions = null;
        private static DbContextOptions<AuthDbContext> AuthDbOptions
        {
            get
            {
                if (_authDbOptions == null)
                {
                    string connectString = AppSetting.GetInstance().GetConfig("ConnectionStrings:AuthDbConnection");
                    _authDbOptions = new DbContextOptionsBuilder<AuthDbContext>().UseMySql(connectString).Options;
                }
                return _authDbOptions;
            }
        }
        public static AuthDbContext AuthDbContext
        {
            get
            {
                return new AuthDbContext(AuthDbOptions);
            }
        }


    }

    
}
