using EventSourcingPoc.API.EFContext;

namespace EventSourcingPoc.API.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();
        public Guid Id { get; protected set; }
        public int Version { get; protected set; } = -1;

        public IReadOnlyList<IDomainEvent> DequeueUncommittedEvents()
        {
            var events = _uncommittedEvents.ToList();
            _uncommittedEvents.Clear();
            return events;
        }
        
        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            _uncommittedEvents.Add(@event);
        }

        public void LoadFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var @event in history)
            {
                ApplyEvent(@event);
                Version++;
            }
        }

        protected abstract void ApplyEvent(IDomainEvent @event);
    }
}