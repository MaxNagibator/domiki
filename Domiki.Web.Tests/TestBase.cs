using Domiki.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Domiki.Web.Tests.DomiksTests;

namespace Domiki.Web.Tests
{
    public class TestBase
    {
        private Settings _options;

        public TestBase()
        {
            var config = InitConfiguration();
            _options = config.Get<Settings>();
        }

        public ApplicationDbContext GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_options.ConnectionStrings.DefaultConnection);
            return new ApplicationDbContext(optionsBuilder.Options, new MyOperationalStoreOptions());
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
