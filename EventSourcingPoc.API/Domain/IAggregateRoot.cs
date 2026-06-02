namespace EventSourcingPoc.API.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<object> GetUncommittedEvents();
        void ClearUncommittedEvents();

    }
}
