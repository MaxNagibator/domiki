using Microsoft.EntityFrameworkCore;

namespace Domiki.Data
{
    public class DomikiDbContext : DbContext
    {
        public DomikiDbContext(DbContextOptions options)
            : base(options)
        {

        }

    }
}