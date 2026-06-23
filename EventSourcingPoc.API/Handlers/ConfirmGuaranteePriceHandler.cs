using EventSourcingPoc.API.Domain;
using Marten;

namespace EventSourcingPoc.API.Handlers
{
    public record ConfirmGuaranteePriceCommand(
        Guid GuaranteeId,
        decimal Cost
    );
   
    public class ConfirmGuaranteePriceHandler(IDocumentSession session)
    {
        private readonly IDocumentSession _session = session;
        public async Task Handle(ConfirmGuaranteePriceCommand command, CancellationToken cancellationToken )
        {
            var guarantee = await _session.Events.AggregateStreamAsync<GuaranteeAggregate>(command.GuaranteeId, token: cancellationToken);
            if (guarantee == null) throw new ArgumentNullException();
            //guarantee.ConfirmPrice(money, lastEndorsement.Sequence);

            var uncommittedEvents = guarantee.GetUncommittedEvents();
            _session.Events.Append(command.GuaranteeId, uncommittedEvents);
            await _session.SaveChangesAsync(cancellationToken);
            guarantee.ClearUncommittedEvents();
        }
    }
}
