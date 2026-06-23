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
        LegalPartyCreateGuaranteeCommand Supplier,
        LegalPartyCreateGuaranteeCommand Beneficiary,
        GuaranteeBond Bond
    );

    public record LegalPartyCreateGuaranteeCommand(
        string TaxId,
        string Name,
        string Street,
        string City,
        string State
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
                Start: command.Start,
                End: command.End,
                InitialAmountCoverage: new Money(command.Amount, Currency.CLP),
                Price: new Money(command.Cost, Currency.CLP),
                Bond: command.Bond,
                Supplier: new LegalPartyInfo(
                    command.Supplier.TaxId,
                    command.Supplier.Name,
                    command.Supplier.Street,
                    command.Supplier.City,
                    command.Supplier.State
                ),
                Beneficiary: new LegalPartyInfo(
                    command.Beneficiary.TaxId,
                    command.Beneficiary.Name,
                    command.Beneficiary.Street,
                    command.Beneficiary.City,
                    command.Beneficiary.State
                ),
                Gloss: command.Gloss
            );
            _session.Events.Append(id, @event);
            await _session.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
