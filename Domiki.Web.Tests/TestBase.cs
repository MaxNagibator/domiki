using Domiki.Data;
using Domiki.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Domiki.Web.Tests.DomiksTests;

namespace Domiki.Web.Tests
{
    public class TestBase
    {
        protected Settings _options;

        public TestBase()
        {
            var config = InitConfiguration();
            _options = config.Get<Settings>();
        }

        public UnitOfWork GetUow()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_options.ConnectionStrings.DefaultConnection);
            var context = new ApplicationDbContext(optionsBuilder.Options, new MyOperationalStoreOptions());
            var uow = new UnitOfWork(context);
            return uow;
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }
    }
}
