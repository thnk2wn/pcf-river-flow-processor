using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RiverFlowApi.Data.Import;

namespace RiverFlowApi.Data
{
    public class RiverDbContext : DbContext
    {
        public RiverDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #if DEBUG
            if (Debugger.IsAttached) optionsBuilder.EnableSensitiveDataLogging();
            #endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #if DEBUG
            if (Debugger.IsAttached)
            {
                var conn = Database.GetDbConnection();
                Trace.WriteLine(conn.ConnectionString);
            }
            #endif

            var dataReader = new RiverImportDataReader().ReadAll();

            modelBuilder.Entity<State>(e =>
            {
                e.HasKey(s => s.StateCode);
                e.Property(s => s.StateCode).HasMaxLength(2);
                e.Property(s => s.Name).HasMaxLength(20);
                e.Property(s => s.Region).HasMaxLength(20);
                e.Property(s => s.Division).HasMaxLength(20);

                e.HasData(dataReader.States);
            });

            modelBuilder.Entity<UsgsGauge>(e =>
            {
                e.HasKey(g => g.UsgsGaugeId);
                e.Property(g => g.UsgsGaugeId).HasMaxLength(15);
                e.Property(g => g.Name).HasMaxLength(50);
                e.Property(g => g.StateCode).HasMaxLength(2);

                e.HasOne(g => g.State)
                 .WithMany()
                 .HasForeignKey(g => g.StateCode);

                e.HasData(dataReader.Gauges);
            });

            modelBuilder.Entity<UsgsRiverSection>(e =>
            {
               e.HasKey(rs => new { rs.UsgsGaugeId, rs.RiverName });
               e.Property(rs => rs.UsgsGaugeId).HasMaxLength(15);
               e.Property(rs => rs.RiverName).HasMaxLength(50);

               e.HasOne(rs => rs.Gauge)
                .WithMany()
                .HasForeignKey(rs => rs.UsgsGaugeId);

               e.HasData(dataReader.Sections);
            });
        }

        public DbSet<State> States { get; set; }

        public DbSet<UsgsGauge> Gauges { get; set; }

        public DbSet<UsgsRiverSection> RiverSections { get; set; }
    }
}