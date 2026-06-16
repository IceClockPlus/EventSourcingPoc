using EventSourcingPoc.API.Domain;

namespace EventSourcingPoc.API.Events
{
    public record GuaranteeRequested(
        Guid Id,
        string TenderId,
        string Gloss,
        GuaranteeBond Bond,
        LegalParty Supplier,
        LegalParty Beneficiary,
        DateRange InitialDateCoverage,
        Money InitialAmountCoverage,
        Money Price
    );

    public record GuaranteePaid(
        Money PaidPrice
    );

    public record GuaranteeIssued(
        DateTime IssueDate
    );

}
