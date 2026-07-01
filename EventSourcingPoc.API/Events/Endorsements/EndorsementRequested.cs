namespace EventSourcingPoc.API.Events.Endorsements
{
    /// <summary>
    /// Event triggered when an endorsement is requested, containing all the necessary details for the endorsement request.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="GuaranteeId"></param>
    /// <param name="EndDate"></param>
    /// <param name="NewAmount"></param>
    /// <param name="Currency"></param>
    /// <param name="SupplierName"></param>
    /// <param name="SupplierAddressStreet"></param>
    /// <param name="SupplierAddressLocation"></param>
    /// <param name="SupplierAddressRegion"></param>
    /// <param name="BeneficiaryName"></param>
    /// <param name="BeneficiaryAddressStreet"></param>
    /// <param name="BeneficiaryAddressLocation"></param>
    /// <param name="BeneficiaryAddressRegion"></param>
    /// <param name="Price"></param>
    public record EndorsementRequested(
        Guid Id,
        Guid GuaranteeId,
        DateOnly? EndDate,
        decimal? NewAmount,
        string? Currency,
        string? SupplierName,
        string? SupplierAddressStreet,
        string? SupplierAddressLocation,
        string? SupplierAddressRegion,
        string? BeneficiaryName,
        string? BeneficiaryAddressStreet,
        string? BeneficiaryAddressLocation,
        string? BeneficiaryAddressRegion,
        decimal Price
    );   
}
