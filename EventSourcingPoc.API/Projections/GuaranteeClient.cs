using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.Events;
using Marten.Events.Aggregation;

namespace EventSourcingPoc.API.Projections
{
    public class GuaranteeClient
    {
        public Guid Id { get; set; }
        public string? TenderId { get; set; }
        public string? Gloss { get; set; }
        public string? GuaranteeCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Money AmountCoverage { get; set; } = null!;
        public GuaranteeStatus Status { get; set; }
        public LegalParty Supplier { get; set; } = null!;
        public LegalParty Beneficiary { get; set; } = null!;
        public GuaranteeInsurance Insurance { get; set; } = null!;
    }

    public partial class GuaranteeClientProjection : SingleStreamProjection<GuaranteeClient, Guid>
    {
        public static GuaranteeClient Create(GuaranteeRequested @event)
        {
            return new GuaranteeClient
            {
                Id = @event.Id,
                TenderId = @event.TenderId,
                Gloss = @event.Gloss,
                Start = @event.Start,
                End = @event.End,
                AmountCoverage = @event.InitialAmountCoverage,
                Status = GuaranteeStatus.Draft,
            };
        }
        
        public void Apply(GuaranteeIssued guaranteeIssued, GuaranteeClient guaranteeClient)
        {
            guaranteeClient.Status = GuaranteeStatus.Issued;
        }
    }
}