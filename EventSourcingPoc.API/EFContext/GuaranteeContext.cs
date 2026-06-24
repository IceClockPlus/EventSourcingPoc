using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace EventSourcingPoc.API.EFContext
{
    public class GuaranteeContext : DbContext
    {
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Bond> Bonds { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public GuaranteeContext(DbContextOptions<GuaranteeContext> options):base(options)
        {
                
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

    public class Insurance
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? LegacyId { get; set; }
        public decimal EnterpriseFactor { get; set; }
        public decimal ExecutiveFactor { get; set; }
        public decimal ExecutiveRenewalFactor { get; set; }
        public long CertificateNumberCounter { get; set; }
    }

    public class Broker
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class Bond
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? LegacyId { get; set; }
    }

    public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
    {
        public void Configure(EntityTypeBuilder<Broker> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }

    public class BondConfiguration : IEntityTypeConfiguration<Bond>
    {
        public void Configure(EntityTypeBuilder<Bond> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(100);
        }
    }

    public class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
    {
        public void Configure(EntityTypeBuilder<Insurance> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(100);
        }
    }
}
