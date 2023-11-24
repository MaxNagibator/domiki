using Domiki.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domiki.Web.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        public DbSet<Domik> Domiks { get; set; }
        public DbSet<Manufacture> Manufactures { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Resource> Resources { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resource>()
                .HasKey(p => new
                {
                    p.PlayerId,
                    p.TypeId
                });

            modelBuilder.Entity<Domik>()
                .HasKey(p => new
                {
                    p.PlayerId,
                    p.Id
                });

            modelBuilder.Entity<Player>()
                .HasIndex(u => u.AspNetUserId)
                .IsUnique();

            modelBuilder.Entity<Player>()
                .Navigation(e => e.Resources)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Resource>()
                .Navigation(e => e.Player)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Manufacture>()
                .HasOne(s => s.Domik)
                .WithMany(x => x.Manufactures)
                .HasForeignKey(e => new { e.DomikPlayerId, e.DomikId});
        }
    }
}