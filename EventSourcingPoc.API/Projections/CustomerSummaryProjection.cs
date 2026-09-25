using EventSourcingPoc.API.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingPoc.API.Projections
{

    public interface IProjection
    {
        string Name { get;}
        Task<int> ProcessBatchAsync(EventStoreContext eventStoreContext, ProjectionContext projectionContext, CancellationToken cancellationToken);
    }

    public class CustomerSummaryProjection : IProjection
    {
        public string Name => "CustomerSummaryProjection";

        public async Task<int> ProcessBatchAsync(EventStoreContext eventStoreContext, ProjectionContext projectionContext, CancellationToken cancellationToken)
        {
            // Retrieve the last processed sequence number from the projection checkpoint
            var checkpoint = await projectionContext.ProjectionCheckpoints.FindAsync([Name], cancellationToken) ?? throw new InvalidOperationException($"Projection checkpoint for '{Name}' not seeded.");

            var batch = await eventStoreContext.Events
                .Where(e => e.GlobalSequence > checkpoint.LastProcessedSequenceNumber)
                .OrderBy(e => e.GlobalSequence)
            .Take(100).ToListAsync(cancellationToken);

            if(batch.Count == 0) return 0;

            var strategy = projectionContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await projectionContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    
                    checkpoint.LastProcessedSequenceNumber = batch[^1].GlobalSequence;
                    checkpoint.LastEventAt = batch[^1].Timestamp;
                    checkpoint.UpdatedAt = DateTime.UtcNow;
                    checkpoint.Status = "Running";
                    checkpoint.LastError = null;

                    await projectionContext.SaveChangesAsync(cancellationToken);
                    await tx.CommitAsync(cancellationToken);

                }catch
                {
                    await tx.RollbackAsync(cancellationToken);
                    throw;
                }
            });
            
            return batch.Count;
        }
    }
}