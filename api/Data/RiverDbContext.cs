using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<UsgsGauge>(e =>
            {
                e.HasKey(g => g.UsgsGaugeId);
                e.Property(g => g.UsgsGaugeId).HasMaxLength(15);
                e.Property(g => g.GaugeName).HasMaxLength(50);
                e.Property(g => g.StateCode).HasMaxLength(2);

                e.HasData(GetRecords<UsgsGauge>());
            });

            modelBuilder.Entity<UsgsRiverSection>(e =>
            {
               e.HasKey(rs => new { rs.UsgsGaugeId, rs.RiverName });
               e.Property(rs => rs.UsgsGaugeId).HasMaxLength(15);
               e.Property(rs => rs.RiverName).HasMaxLength(50);

               e.HasData(GetRecords<UsgsRiverSection>());
            });

            modelBuilder.Entity<State>(e =>
            {
                e.HasKey(s => s.StateCode);
                e.Property(s => s.StateCode).HasMaxLength(2);
                e.Property(s => s.Name).HasMaxLength(20);
                e.Property(s => s.Region).HasMaxLength(20);
                e.Property(s => s.Division).HasMaxLength(20);

                e.HasData(GetRecords<State>());
            });
        }

        public DbSet<State> States { get; set; }

        public DbSet<UsgsGauge> Gauges { get; set; }

        public DbSet<UsgsRiverSection> RiverSections { get; set; }

        private T[] GetRecords<T>()
        {
            var entity = typeof(T).Name;
            var typeInfo = this.GetType().GetTypeInfo();
            var resource = $"{typeInfo.Namespace}.ImportFiles.{entity}.csv";

            using (var resourceStream = typeInfo.Assembly.GetManifestResourceStream(resource))
            using (var streamReader = new StreamReader(resourceStream))
            using (var csv = new CsvReader(streamReader))
            {
                var records = csv.GetRecords<T>().ToArray();
                return records;
            }
        }
    }
}