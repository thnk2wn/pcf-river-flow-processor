using Microsoft.EntityFrameworkCore;

namespace RiverFlowApi.Data
{
    public class RiverDbContext : DbContext
    {
        public RiverDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsgsGauge>(e =>
            {
                e.HasKey(g => g.UsgsGaugeId);
                e.Property(g => g.UsgsGaugeId).HasMaxLength(8);
                e.Property(g => g.GaugeName).HasMaxLength(50);
                e.Property(g => g.StateCode).HasMaxLength(2);
            });

            modelBuilder.Entity<UsgsRiverSection>(e =>
            {
               e.HasKey(rs => new { rs.UsgsGaugeId, rs.RiverName });
               e.Property(rs => rs.UsgsGaugeId).HasMaxLength(8);
               e.Property(rs => rs.RiverName).HasMaxLength(50);
            });

            modelBuilder.Entity<State>(e =>
            {
                e.HasKey(s => s.StateCode);
                e.Property(s => s.StateCode).HasMaxLength(2);
                e.Property(s => s.Name).HasMaxLength(20);
                e.Property(s => s.Region).HasMaxLength(20);
                e.Property(s => s.Division).HasMaxLength(20);
            });
        }

        public DbSet<State> States { get; set; }

        public DbSet<UsgsGauge> Gauges { get; set; }

        public DbSet<UsgsRiverSection> RiverSections { get; set; }
    }
}