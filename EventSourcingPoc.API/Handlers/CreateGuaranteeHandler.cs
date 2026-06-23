using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.DTOs;
using EventSourcingPoc.API.Events;
using EventSourcingPoc.API.Services;
using EventSourcingPoc.API.Wrappers;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record CreateGuaranteeCommand(
        string TenderId,
        string Gloss,
        DateTime Start,
        DateTime End,
        decimal Amount,
        Currency CurrencyAmount,
        decimal Price,
        Currency CurrencyPrice,
        LegalPartyCreateGuaranteeCommand Supplier,
        LegalPartyCreateGuaranteeCommand Beneficiary,
        int Bond
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
        private readonly IBondsService _bondsService;
        private readonly IDocumentSession _session;
        public CreateGuaranteeHandler(IDocumentSession session, IBondsService bondsService)
        {
            _session = session;
            _bondsService = bondsService;
        }

        public async Task<Result<GuaranteeDto>> Handle(CreateGuaranteeCommand command, CancellationToken cancellationToken)
        {
            Guid id = Guid.CreateVersion7();

            var bond = await _bondsService.GetById((int)command.Bond, cancellationToken);
            if (bond == null) return Result<GuaranteeDto>.Failure("Finalidad no encontrada");

            GuaranteeRequested @event = new GuaranteeRequested(
                Id: id,
                TenderId: command.TenderId,
                Start: command.Start,
                End: command.End,
                InitialAmountCoverage: new Money(command.Amount, command.CurrencyAmount),
                Price: new Money(command.Price, command.CurrencyPrice),
                Bond: new GuaranteeRequestBond(bond.Id, bond.Name),
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
            return Result<GuaranteeDto>.Ok(
                new GuaranteeDto(Id: id, Start: command.Start, End: command.End, Amount: command.Amount, command.CurrencyAmount.ToString(), TenderId: command.TenderId, Bond: new GuaranteeBondDto(bond.Id, bond.Name))
            );
        }
    }
}
