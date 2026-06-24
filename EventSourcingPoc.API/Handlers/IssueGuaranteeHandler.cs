using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.Services;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record IssueGuaranteeCommand(Guid GuaranteeId);
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
        }
    }
}
