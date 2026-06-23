using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace EventSourcingPoc.API.EFContext
{
    public class GuaranteeContext : DbContext
    {
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Bond> Bonds { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

    public class Insurance
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public int? LegacyId { get; set; }
        public decimal EnterpriseFactor { get; set; }
        public decimal ExecutiveFactor { get; set; }
        public decimal ExecutiveRenewalFactor { get; set; }
    }

    public class Bond
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? LegacyId { get; set; }
    }

    public class BondConfiguration : IEntityTypeConfiguration<Bond>
    {
        public void Configure(EntityTypeBuilder<Bond> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }

    public class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
    {
        public void Configure(EntityTypeBuilder<Insurance> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
