using Microsoft.EntityFrameworkCore;

namespace RiverFlowApi.Data
{
    public class RiverDbContext : DbContext
    {
        public RiverDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<State> States { get; set; }

        public DbSet<Gauge> Gauges { get; set; }

        public DbSet<UsgsRiverSection> RiverSections { get; set; }
    }
}