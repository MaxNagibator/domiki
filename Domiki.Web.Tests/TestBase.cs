using Domiki.Web.Business;
using Domiki.Web.Business.Core;
using Domiki.Web.Data;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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

        public DomikManager GetDomikManager(UnitOfWork uow, bool calculatorJustFinishMode = true)
        {
            var domikManager = new DomikManager(uow, uow.Context, GetCalculator(calculatorJustFinishMode), new ResourceManager(uow.Context));
            return domikManager;
        }

        public ResourceManager GetResourceManager(UnitOfWork uow)
        {
            var manager = new ResourceManager(uow.Context);
            return manager;
        }

        private ICalculator GetCalculator(bool justFinishMode = true)
        {
            return new TestCalculator(() => GetUow(), (UnitOfWork uow) => { return new CalculatorTick(GetDomikManager(uow)); }, justFinishMode);
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }

        public class MyOperationalStoreOptions : IOptions<OperationalStoreOptions>
        {
            public OperationalStoreOptions Value => new OperationalStoreOptions()
            {
                DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                EnableTokenCleanup = false,
                PersistedGrants = new TableConfiguration("PersistedGrants"),
                TokenCleanupBatchSize = 100,
                TokenCleanupInterval = 3600,
            };
        }
    }
}
