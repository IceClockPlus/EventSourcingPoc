using EventSourcingPoc.API.Domain;

namespace EventSourcingPoc.API.Events
{
    public class GuaranteeEvents
    {
        public record GuaranteeCreated(
            Guid Id,
            string TenderId,
            GuaranteePurpose Purpose,
            Guid CustomerId,
            Guid BeneficiaryId,
            DateRange InitialDateCoverage,
            Money InitialAmountCoverage,
            Money Cost
        );

        public record GuaranteePriceConfirmed(
            int Sequence,
            Money Cost
        );

        public record GuranteePaid(
            Money PaidCost
        );

        public record GuaranteeIssued(
            DateTime IssueDate,
            List<GuaranteeDocument> Documents
        );

        public record GuaranteeExtensionRequested(
            DateTime ExtensionDate
        );
        
    }
}
