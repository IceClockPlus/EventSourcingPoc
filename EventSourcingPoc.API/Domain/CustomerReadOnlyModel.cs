namespace EventSourcingPoc.API.Domain
{
    /// <summary>
    /// Represents a read-only model of a customer, containing only the essential information needed for display or querying purposes.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="TaxId"></param>
    /// <param name="Name"></param>
    public record CustomerReadOnlyModel(Guid Id, string TaxId, string Name);
}