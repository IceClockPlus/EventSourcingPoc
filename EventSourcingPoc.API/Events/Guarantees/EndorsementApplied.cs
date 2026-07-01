namespace EventSourcingPoc.API.Events.Guarantees
{
    /// <summary>
    /// Event triggered when an endorsement is applied, indicating that the endorsement has been successfully applied to the guarantee.
    /// </summary>
    public record EndorsementApplied(
        DateTime AppliedAt,
        RequestedChanges Changes
    );

    public record RequestedChanges(
        DateOnly? EndDate,
        decimal? Amount,
        string? Gloss,
        SupplerChange? Supplier,
        BeneficiaryChange? Beneficiary
    );

    public record SupplerChange(
        string? Name,
        string? AddressStreet,
        string? AddressLocation,
        string? AddressRegion
    );

    public record BeneficiaryChange(
        string? Name,
        string? AddressStreet,
        string? AddressLocation,
        string? AddressRegion
    );
}
