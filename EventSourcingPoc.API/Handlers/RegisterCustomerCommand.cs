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
        private readonly ILogger<RegisterCustomerCommandHandler> _logger;

        public RegisterCustomerCommandHandler(EventStore eventStore, ILogger<RegisterCustomerCommandHandler> logger)
        {
            _eventStore = eventStore;
            _logger = logger;
        }

        public async Task<Result> Handle(RegisterCustomeCommand command, CancellationToken cancellationToken)
        {
            var customerId = Guid.NewGuid();
            var customer = Customer.Register(customerId, command.TaxId, command.Name);

            await _eventStore.AppendAsync(customerId, customer.Version, customer.DequeueUncommittedEvents(), cancellationToken);
            _logger.LogInformation("Customer registered with ID: {CustomerId}", customerId);
            return Result.Success();
        }
    }
}