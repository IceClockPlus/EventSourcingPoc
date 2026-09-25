using EventSourcingPoc.API.Contracts;
using EventSourcingPoc.API.Domain;
using EventSourcingPoc.API.EFContext;

namespace EventSourcingPoc.API.Handlers
{
    public record RegisterCustomeCommand(
        string TaxId,
        string Name
    ): ICommand;

    internal sealed class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomeCommand>
    {
        private readonly EventStore _eventStore;

        public RegisterCustomerCommandHandler(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Result> Handle(RegisterCustomeCommand command, CancellationToken cancellationToken)
        {
            var customerId = Guid.NewGuid();
            var customer = Customer.Register(customerId, command.TaxId, command.Name);

            await _eventStore.AppendAsync(customerId, customer.Version, customer.DequeueUncommittedEvents(), cancellationToken);

            return Result.Success();
        }
    }
}