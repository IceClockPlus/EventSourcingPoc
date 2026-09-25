using EventSourcingPoc.API.EFContext;

namespace EventSourcingPoc.API.Domain
{
    public class Customer : AggregateRoot
    {
        public string TaxId { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public GuaranteeLine GuaranteeLine { get; private set; } = new GuaranteeLine(0, 0);
        public bool IsEnabled { get; private set; } = true;

        public static Customer Register(Guid guid, string taxId, string name)
        {
            var customer = new Customer();

            customer.RaiseEvent(new CustomerRegistered(guid, taxId, name));
            return customer;
        }

        public void EnabledCustomer()
        {
            if (!IsEnabled)
            {
                IsEnabled = true;
            }
        }

        public void DisableCustomer()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
            }
        }

        public void UpdateGuaranteeLine(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative.", nameof(amount));

            RaiseEvent(new CustomerGuaranteeLineUpdated(amount));
        }

        public void ReleaseGuaranteeLine(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (GuaranteeLine.AmountUsed < amount)
                throw new InvalidOperationException("Cannot release more than the amount used.");

            RaiseEvent(new CustomerGuaranteeLineReleased(amount));
        }    

        public void UseGuaranteeLine(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (GuaranteeLine.Amount - GuaranteeLine.AmountUsed < amount)
                throw new InvalidOperationException("Insufficient guarantee line available.");

            RaiseEvent(new CustomerGuaranteeLineUsed(amount));
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch(@event)
            {
                case CustomerRegistered e:
                    Id = e.Id;
                    TaxId = e.TaxId;
                    Name = e.Name;
                    IsEnabled = true;
                    break;
                case CustomerGuaranteeLineUsed e:
                    GuaranteeLine = GuaranteeLine with { AmountUsed = GuaranteeLine.AmountUsed + e.AmountUsed };
                    break;
                case CustomerGuaranteeLineUpdated e:
                    GuaranteeLine = GuaranteeLine with { Amount = e.Amount };
                    break;
                case CustomerGuaranteeLineReleased e:
                    GuaranteeLine = GuaranteeLine with { AmountUsed = GuaranteeLine.AmountUsed - e.AmountReleased };
                    break;
                default:
                    throw new InvalidOperationException($"Unknown event type: {@event.GetType().Name}");
            }
        }
    }

    public record GuaranteeLine(decimal Amount, decimal AmountUsed);

    public record CustomerGuaranteeLineUpdated(decimal Amount): IDomainEvent;
    public record CustomerRegistered(Guid Id, string TaxId, string Name) : IDomainEvent;
    public record CustomerGuaranteeLineUsed(decimal AmountUsed) : IDomainEvent;
    public record CustomerGuaranteeLineReleased(decimal AmountReleased) : IDomainEvent;
}