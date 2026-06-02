using EventSourcingPoc.API.Domain;
using Marten;
using static EventSourcingPoc.API.Events.GuaranteeEvents;

namespace EventSourcingPoc.API.Handlers
{
    public record CreateGuaranteeCommand(
        string TenderId,
        DateTime Start,
        DateTime End,
        decimal Amount,
        decimal Cost,
        Guid CustomerId,
        Guid BeneficiaryId,
        GuaranteePurpose Purpose
    );
    
    public class CreateGuaranteeHandler
    {
        private readonly IDocumentSession _session;
        public CreateGuaranteeHandler(IDocumentSession session)
        {
            _session = session;
        }

        public async Task Handle(CreateGuaranteeCommand command, CancellationToken cancellationToken)
        {
            Guid id = Guid.CreateVersion7();
            GuaranteeCreated @event = new GuaranteeCreated(
                Id: id,
                TenderId: command.TenderId,
                InitialDateCoverage: new DateRange(command.Start, command.End),
                InitialAmountCoverage: new Money(command.Amount, Currency.CLP),
                Cost: new Money(command.Cost, Currency.CLP),
                Purpose: command.Purpose,
                CustomerId: command.CustomerId,
                BeneficiaryId: command.BeneficiaryId
            );
            _session.Events.Append(id, @event);
            await _session.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
