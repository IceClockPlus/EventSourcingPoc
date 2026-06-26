using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.DTOs;
using EventSourcingPoc.API.Events;
using EventSourcingPoc.API.Wrappers;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record UpdateGuaranteeInformationCommand(
        Guid Id,
        string? Gloss,
        string? TenderId,
        UpdateGuaranteePartyInformation? SupplierInfo,
        UpdateGuaranteePartyInformation? BeneficiaryInfo
    );

    public record UpdateGuaranteePartyInformation(string? Name, string? AddressStreet, string? AddressLocation, string? AddressRegion);
    public class UpdateGuaranteeInformationHandler(IDocumentSession _session)
    {
        private readonly IDocumentSession _session = _session;
        public async Task<Result<GuaranteeDto>> Handle(UpdateGuaranteeInformationCommand command, CancellationToken cancellationToken)
        {
            var guarantee = await _session.Events.AggregateStreamAsync<GuaranteeAggregate>(command.Id, token: cancellationToken);
            if (guarantee == null) return Result<GuaranteeDto>.Failure("Garantia no encontrada");
            if (guarantee.Status != GuaranteeStatus.Draft) return Result<GuaranteeDto>.Failure("Garantia bloqueada a modificaciones");
            bool isGuaranteeInfoModification = !string.IsNullOrWhiteSpace(command.Gloss) || !string.IsNullOrWhiteSpace(command.TenderId);
            bool isSupplierInfoModification = command.SupplierInfo != null 
                && (!string.IsNullOrWhiteSpace(command.SupplierInfo.Name) 
                    || !string.IsNullOrWhiteSpace(command.SupplierInfo.AddressStreet)
                    || !string.IsNullOrWhiteSpace(command.SupplierInfo.AddressLocation) || !string.IsNullOrWhiteSpace(command.SupplierInfo.AddressRegion)
            );

            bool isBeneficiaryInfoModification = command.BeneficiaryInfo != null
                && (!string.IsNullOrWhiteSpace(command.BeneficiaryInfo.Name)
                    || !string.IsNullOrWhiteSpace(command.BeneficiaryInfo.AddressStreet)
                    || !string.IsNullOrWhiteSpace(command.BeneficiaryInfo.AddressLocation) || !string.IsNullOrWhiteSpace(command.BeneficiaryInfo.AddressRegion)
            );

            if (isGuaranteeInfoModification)
            {
                GuaranteeInformationUpdated guaranteeInformationUpdated = new(
                    Gloss: command.Gloss,
                    TenderId: command.TenderId
                );
                _session.Events.Append(command.Id, guaranteeInformationUpdated);
            }

            if (isSupplierInfoModification)
            {
                _session.Events.Append(command.Id, 
                    new GuaranteeSupplierInformationUpdated(command.SupplierInfo!.Name, command.SupplierInfo!.AddressStreet, command.SupplierInfo!.AddressLocation, command.SupplierInfo!.AddressRegion));
            }
            if (isBeneficiaryInfoModification)
            {
                GuaranteeBeneficiaryInformationUpdated beneficiaryInformationUpdated = new(
                    Name: command.BeneficiaryInfo!.Name, AddressStreet: command.BeneficiaryInfo!.AddressStreet, AddressLocation: command.BeneficiaryInfo!.AddressLocation, AddressRegion: command.BeneficiaryInfo!.AddressRegion
                );
                _session.Events.Append(command.Id, beneficiaryInformationUpdated);
            }
            await _session.SaveChangesAsync(cancellationToken);
            return Result<GuaranteeDto>.Ok(new GuaranteeDto(
                Id: guarantee.Id,
                Start: guarantee.CurrentDateCoverage.Start,
                End: guarantee.CurrentDateCoverage.End,
                Amount: guarantee.AmountCoverage.Amount,
                CurrencyAmount: guarantee.AmountCoverage.Currency.ToString(),
                TenderId: command.TenderId ?? guarantee.Information.TenderId,
                Bond: new GuaranteeBondDto(guarantee.Bond.Id, guarantee.Bond.Name)
            ));
        }
    }
}
