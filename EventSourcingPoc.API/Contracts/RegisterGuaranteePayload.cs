namespace EventSourcingPoc.API.Contracts
{
    public record RegisterGuaranteePayload(
        string TenderId,
        string Gloss,
        int BondId,
        decimal Amount,
        string Currency,
        decimal Price,
        GuaranteeRequestParty Supplier,
        GuaranteeRequestParty Beneficiary,
        DateTime Start,
        DateTime End,
        int? BrokerId = null
    );

    public record GuaranteeRequestParty(
        string TaxId,
        string Name,
        string AddressStreet,
        string AddressLocation,
        string AddressRegion
    );

}