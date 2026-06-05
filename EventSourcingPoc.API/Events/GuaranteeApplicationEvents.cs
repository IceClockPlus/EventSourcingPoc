using EventSourcingPoc.API.Domain;

namespace EventSourcingPoc.API.Events
{

    public record GuaranteeApplicationSubmitted(
        Guid Id,
        string TenderId,
        string Gloss,
        GuaranteePurpose Purpose,
        LegalParty Supplier,
        LegalParty Beneficiary,
        DateRange InitialDateCoverage,
        Money InitialAmountCoverage,
        Money Cost
    );

    public record GuaranteeEndorsementRequested(

    );
}
