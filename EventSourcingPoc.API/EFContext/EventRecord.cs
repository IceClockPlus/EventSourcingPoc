namespace EventSourcingPoc.API.EFContext
{
    public class EventRecord
    {
        public long GlobalSequence { get; set; }
        public Guid StreamId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string? Metadata { get; set; }
        public DateTime Timestamp { get; set; }
    }
}