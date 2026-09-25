using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSourcingPoc.API.Projections
{
    public class ProjectionContext(DbContextOptions<ProjectionContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectionCheckpointConfiguration());
        }        

        public DbSet<CustomerSummary> CustomerSummaries { get; set; }
        public DbSet<ProjectionCheckpoint> ProjectionCheckpoints { get; set; }

        internal class ProjectionCheckpointConfiguration : IEntityTypeConfiguration<ProjectionCheckpoint>
        {
            public void Configure(EntityTypeBuilder<ProjectionCheckpoint> builder)
            {
                builder.HasKey(p => p.ProjectionName);
                builder.Property(p => p.ProjectionName).IsRequired().HasMaxLength(100);
                builder.Property(p => p.LastProcessedSequenceNumber).IsRequired();
            }
        }


        internal class CustomerSummaryConfiguration : IEntityTypeConfiguration<CustomerSummary>
        {
            public void Configure(EntityTypeBuilder<CustomerSummary> builder)
            {
                builder.HasKey(c => c.TaxId);
                builder.HasIndex(c => c.StreamId).IsUnique();
                builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
                builder.Property(c => c.Balance).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(c => c.UsedBalance).IsRequired().HasColumnType("decimal(18,2)");
            }
        }

    }    

    /// <summary>
    /// Represents a summary of a customer for projection purposes.
    /// </summary>
    public class CustomerSummary
    {
        /// <summary>
        /// Gets or sets the tax ID of the customer.
        /// </summary>
        public string TaxId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the stream ID associated with the customer.
        /// </summary>
        public Guid StreamId { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the balance of the customer.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the used balance of the customer.
        /// </summary>
        public decimal UsedBalance { get; set; }
    }

    /// <summary>
    /// Represents a checkpoint for projections.
    /// </summary>
    public class ProjectionCheckpoint
    {
        /// <summary>
        /// Gets or sets the name of the projection.
        /// </summary>
        public string ProjectionName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the last processed event sequence number.
        /// </summary>
        public long LastProcessedSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last event processed by the projection.
        /// </summary>
        public DateTime LastEventAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last update to the projection checkpoint.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the status of the projection checkpoint.
        /// </summary>
        public string Status { get; set; } = default!;

        /// <summary>
        /// Gets or sets the last error encountered by the projection, if any.
        /// </summary>
        public string? LastError { get; set; }
    }
}