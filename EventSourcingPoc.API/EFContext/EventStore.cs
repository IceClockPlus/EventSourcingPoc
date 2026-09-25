using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.Events;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingPoc.API.EFContext
{
    public interface IDomainEvent{}

    public class EventStore(EventStoreContext db, EventTypeMap eventTypeMap)
    {
        public async Task AppendAsync(Guid streamId, int expectedVersion, IReadOnlyList<IDomainEvent> events, CancellationToken ct)
        {
            var version = expectedVersion;
            foreach(var e in events)
            {
                db.Events.Add(new EventRecord
                {
                    StreamId = streamId,
                    Version = ++version,
                    EventType = eventTypeMap.Nameof(e.GetType()),
                    Data = System.Text.Json.JsonSerializer.Serialize(e, e.GetType())
                });
            }
            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch(DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException("Concurrency conflict occurred while appending events.", ex);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Concurrency conflict occurred while appending events."); 
            }
        }

        /// <summary>
        /// Reads the event stream for a given stream ID, returning a list of domain events in the order they were appended.
        /// </summary>
        /// <param name="streamId">Stream ID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of domain events</returns>
        public async Task<IReadOnlyList<IDomainEvent>> ReadStreamAsync(Guid streamId, CancellationToken ct)
        {
            var eventRecords = await db.Events
                .AsNoTracking()
                .Where(e => e.StreamId == streamId)
                .OrderBy(e => e.Version)
                .ToListAsync(ct);

            return eventRecords.Select(er =>
            {
                var eventType = eventTypeMap.Typeof(er.EventType);
                return (IDomainEvent)System.Text.Json.JsonSerializer.Deserialize(er.Data, eventType)!;
            }).ToList();
        }

        /// <summary>
        /// Checks if the given DbUpdateException is caused by a unique constraint violation.
        /// </summary>
        /// <remarks>This can happens when try to add a duplicate version of an event of a stream.</remarks>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            // Check if the exception is due to a unique constraint violation
            return ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2627 or 2601 }; // SQL Server error code for unique constraint violation
        }
    }


    public class EventTypeMap
    {
        private readonly Dictionary<string, Type> _eventTypeMap = new()
        {
            ["GuaranteeRegistered"] = typeof(GuaranteeRegistered),
            ["CustomerRegistered"] = typeof(CustomerRegistered),
            ["CustomerGuaranteeLineUsed"] = typeof(CustomerGuaranteeLineUsed),
            ["CustomerGuaranteeLineUpdated"] = typeof(CustomerGuaranteeLineUpdated),
            ["CustomerGuaranteeLineReleased"] = typeof(CustomerGuaranteeLineReleased)
        };

        public Type Typeof(string name) => _eventTypeMap.TryGetValue(name, out var type) ? type : throw new ArgumentException($"Event type '{name}' not found in the map.");
        public string Nameof(Type type) => _eventTypeMap.FirstOrDefault(kvp => kvp.Value == type).Key ?? throw new ArgumentException($"Event type '{type}' not found in the map.");
    }
}