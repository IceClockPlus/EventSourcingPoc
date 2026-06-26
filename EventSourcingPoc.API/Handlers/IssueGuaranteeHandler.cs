using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.Events;
using EventSourcingPoc.API.Services;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record IssueGuaranteeCommand(Guid GuaranteeId, DateTime IssueDate);
    public class IssueGuaranteeHandler(IDocumentSession session, IInsuranceService insuranceService)
    {
        private readonly IDocumentSession _session = session;
        private readonly IInsuranceService _insuranceService = insuranceService;

        public async Task Handle(IssueGuaranteeCommand command, CancellationToken cancellationToken)
        {
            // Rehydrate guarantee aggregate
            var guarantee = await _session.Events.AggregateStreamAsync<GuaranteeAggregate>(command.GuaranteeId, token: cancellationToken);
            if(guarantee == null ) 
                throw new KeyNotFoundException($"La garantia con ID {command.GuaranteeId} no existe");

            if(guarantee.Code == null)
            {
                long newCertificateNumber = await _insuranceService.GenerateNewCertificateNumber(guarantee.Insurance.Id, cancellationToken);
                Random random = new Random();
                int prefNum = random.Next(1, 10000);
                string code = "W" + prefNum.ToString("D4") + "-" + newCertificateNumber.ToString("D6");
                GuaranteeIssued guaranteeIssueEvent = new GuaranteeIssued(
                    IssueDate: command.IssueDate,
                    CertificateNumber: newCertificateNumber,
                    Code: code
                );
                _session.Events.Append(command.GuaranteeId, guaranteeIssueEvent);
                await _session.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
