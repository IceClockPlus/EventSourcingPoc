namespace EventSourcingPoc.API.Contracts
{
    public record RegisterCustomerPayload(
        string TaxId,
        string Name
    );
}