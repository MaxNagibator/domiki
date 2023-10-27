using Domiki.Business.Models;
using Domiki.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

namespace Domiki.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        public DbSet<Domik> Domiks { get; set; }
        public DbSet<Player> Players { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domik>()
                .HasKey(p => new
                {
                    p.PlayerId,
                    p.Id
                });

            modelBuilder.Entity<Player>()
                .HasIndex(u => u.AspNetUserId)
                .IsUnique();
        }
    }
}