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

            const int gaugeLength = 15;

            modelBuilder.Entity<State>(e =>
            {
                e.HasKey(s => s.StateCode);
                e.Property(s => s.StateCode).HasMaxLength(2);
                e.Property(s => s.Name).HasMaxLength(20).IsRequired();
                e.Property(s => s.Region).HasMaxLength(20).IsRequired();
                e.Property(s => s.Division).HasMaxLength(20).IsRequired();

                e.HasData(dataReader.States);
            });

            modelBuilder.Entity<River>(e =>
            {
                e.HasKey(r => r.RiverId);

                e.Property(r => r.RiverSection).HasMaxLength(50);
                e.Property(r => r.StateCode).HasMaxLength(2).IsRequired();

                e.HasOne(r => r.State)
                 .WithMany()
                 .HasForeignKey(r => r.StateCode);

                e.HasData(dataReader.Rivers);
            });

            modelBuilder.Entity<Gauge>(e =>
            {
                e.HasKey(g => g.UsgsGaugeId);
                e.Property(g => g.UsgsGaugeId).HasMaxLength(gaugeLength);
                e.Property(g => g.Name).HasMaxLength(50).IsRequired();
                e.Property(g => g.StateCode).HasMaxLength(2).IsRequired();

                e.HasOne(g => g.State)
                 .WithMany()
                 .HasForeignKey(g => g.StateCode);

                e.HasData(dataReader.Gauges);
            });

            modelBuilder.Entity<RiverGauge>(e =>
            {
               e.HasKey(rg => new { rg.RiverId, rg.UsgsGaugeId });
               e.Property(rg => rg.UsgsGaugeId).HasMaxLength(gaugeLength);

               e.HasData(dataReader.RiverGauges);
            });

            modelBuilder.Entity<GaugeValue>(e =>
            {
                e.HasKey(gv => new { gv.AsOfUTC, gv.UsgsGaugeId, gv.Code });

                e.Property(gv => gv.UsgsGaugeId).HasMaxLength(gaugeLength).IsRequired();
                e.Property(gv => gv.Code).HasMaxLength(5);

                e.HasOne(gv => gv.Gauge)
                 .WithMany(g => g.Values)
                 .HasForeignKey(gv => gv.UsgsGaugeId);

                e.HasOne(gv => gv.Variable)
                 .WithMany()
                 .HasForeignKey(gv => gv.Code);
            });

            modelBuilder.Entity<Variable>(e =>
            {
                e.HasKey(v => v.Code);
                e.Property(v => v.Code).HasMaxLength(5).IsRequired();

                e.Property(v => v.Name).HasMaxLength(50).IsRequired();
                e.Property(v => v.Description).HasMaxLength(50).IsRequired();
                e.Property(v => v.Unit).HasMaxLength(10).IsRequired();
            });
        }

        public DbSet<State> State { get; set; }

        public DbSet<River> River { get; set; }

        public DbSet<Gauge> Gauge { get; set; }

        public DbSet<RiverGauge> RiverGauge { get; set; }

        public DbSet<GaugeValue> GaugeValue { get; set; }

        public DbSet<Variable> Variable { get; set; }
    }
}