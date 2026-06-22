using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.Events;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record CreateGuaranteeCommand(
        string TenderId,
        string Gloss,
        DateTime Start,
        DateTime End,
        decimal Amount,
        decimal Cost,
        Guid CustomerId,
        Guid BeneficiaryId,
        GuaranteeBond Bond
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
            GuaranteeRequested @event = new GuaranteeRequested(
                Id: id,
                TenderId: command.TenderId,
                InitialDateCoverage: new DateRange(command.Start, command.End),
                InitialAmountCoverage: new Money(command.Amount, Currency.CLP),
                Price: new Money(command.Cost, Currency.CLP),
                Bond: command.Bond,
                Supplier: new LegalParty("111111111", "Cliente", new Address("Calle 2", "Santiago", "Metropolitana")),
                Beneficiary: new LegalParty("444444444", "Mandante", new Address("Calle 10", "Santiago", "Metropolitana")),
                Gloss: command.Gloss
            );
            _session.Events.Append(id, @event);
            await _session.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
