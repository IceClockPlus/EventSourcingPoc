using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace EventSourcingPoc.API.EFContext
{
    public class EventStoreContext : DbContext
    {
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Bond> Bonds { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<CustomerLookupModel> CustomerLookups { get; set; }
        public DbSet<EventRecord> Events { get; set; }
        public EventStoreContext(DbContextOptions<EventStoreContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BrokerConfiguration());
            modelBuilder.ApplyConfiguration(new BondConfiguration());
            modelBuilder.ApplyConfiguration(new InsuranceConfiguration());
            modelBuilder.ApplyConfiguration(new EventRecordConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerLookupConfiguration());
        }

        internal class BrokerConfiguration : IEntityTypeConfiguration<Broker>
        {
            public void Configure(EntityTypeBuilder<Broker> builder)
            {
                builder.HasKey(p => p.Id);
            }
        }

        internal class BondConfiguration : IEntityTypeConfiguration<Bond>
        {
            public void Configure(EntityTypeBuilder<Bond> builder)
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Name).HasMaxLength(100);
            }
        }

        internal class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
        {
            public void Configure(EntityTypeBuilder<Insurance> builder)
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Name).HasMaxLength(100);
            }
        }

        internal class EventRecordConfiguration : IEntityTypeConfiguration<EventRecord>
        {
            public void Configure(EntityTypeBuilder<EventRecord> builder)
            {
                builder.HasKey(p => p.GlobalSequence);
                builder.Property(p => p.GlobalSequence).ValueGeneratedOnAdd();
                builder.Property(p => p.EventType).HasMaxLength(200);
                builder.Property(p => p.Data).IsRequired();
                builder.Property(p => p.Timestamp).HasDefaultValueSql("SYSUTCDATETIME()");
                builder.HasIndex(p => new { p.StreamId, p.Version }).IsUnique();
            }
        }

        internal class CustomerLookupConfiguration : IEntityTypeConfiguration<CustomerLookupModel>
        {
            public void Configure(EntityTypeBuilder<CustomerLookupModel> builder)
            {
                builder.HasKey(p => p.TaxId);
            }
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

    /// <summary>
    /// Represents a customer projection of a customer
    /// </summary>
    public class CustomerReadModel
    {
        public string TaxId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public Guid StreamId { get; set; }
        public decimal Line { get; set; }
        public decimal UsedLine { get; set; }
        public decimal AvailableLine { get; set; }
    }

    public class CustomerLookupModel
    {
        /// <summary>
        /// Represents the natural identifier of the customer, such as a tax identification number or social security number. This property is required and should be unique for each customer.
        /// </summary>
        public string TaxId { get; set; } = default!;

        /// <summary>
        /// Represents the stream ID of the customer in the event store. This is used to correlate events related to this customer.
        /// </summary>
        public Guid StreamId { get; set; }
    }
}
