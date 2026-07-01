using EventSourcingPoc.API.Events;
using EventSourcingPoc.API.Events.Guarantees;
using ImTools;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace EventSourcingPoc.API.Domain
{
    /// <summary>
    /// Aggregate that represents the guarantee
    /// </summary>
    public class GuaranteeAggregate : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public GuaranteeInformation Information { get; private set; } = null!;
        public DateRange CurrentDateCoverage { get; private set; } = null!;
        public Money AmountCoverage { get; private set; } = null!;
        public Money RemainingAmount { get; private set; } = null!;
        public GuaranteeBond Bond { get; private set; } = null!;
        public GuaranteeCode? Code { get; private set; }
        public GuaranteeStatus Status {  get; private set; }

        /// <summary>
        /// En este contexto, el "Proveedor" se refiere a la parte que solicita la garantía, es decir, la empresa o individuo que necesita la garantía para respaldar su participación en un proceso de licitación o contrato. El "Beneficiario" es la parte que se beneficia de la garantía, generalmente la entidad que requiere la garantía como parte de los requisitos del proceso de licitación o contrato. En este caso, el proveedor es quien solicita la garantía y el beneficiario es quien recibe la protección que ofrece la garantía en caso de incumplimiento por parte del proveedor.
        /// </summary>
        public LegalParty Supplier { get; private set; } = null!;

        /// <summary>
        /// En este contexto, el "Beneficiario" se refiere a la parte que se beneficia de la garantía, es decir, la entidad que requiere la garantía como parte de los requisitos del proceso de licitación o contrato. El beneficiario es quien recibe la protección que ofrece la garantía en caso de incumplimiento por parte del proveedor. En este caso, el proveedor es quien solicita la garantía y el beneficiario es quien recibe la protección que ofrece la garantía en caso de incumplimiento por parte del proveedor.
        /// </summary>
        public LegalParty Beneficiary { get; private set; } = null!;
        public GuaranteeInsurance Insurance { get; private set; } = null!;
        public GuaranteeBroker? Broker { get; private set; }
        public int Version { get; private set; }

        private readonly List<object> _uncommittedEvents = new();
        public IReadOnlyCollection<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        //private void RaiseEvent(object @event)
        //{
        //    _uncommittedEvents.Add(@event);
        //    switch (@event)
        //    {
        //        case GuaranteeRequested e: Apply(e); break;
        //        //case GuaranteePriceConfirmed e: Apply(e); break;
        //        default:
        //            throw new InvalidOperationException();
        //    }
        //}

        public bool IsSupplierInfoCompleted => !string.IsNullOrWhiteSpace(Supplier.Name)
            && !string.IsNullOrWhiteSpace(Supplier.Address.Street)
            && !string.IsNullOrWhiteSpace(Supplier.Address.Location)
            && !string.IsNullOrWhiteSpace(Supplier.Address.Region);

        public bool IsBeneficiaryInfoCompleted => !string.IsNullOrWhiteSpace(Beneficiary.Name)
            && !string.IsNullOrWhiteSpace(Beneficiary.Address.Street)
            && !string.IsNullOrWhiteSpace(Beneficiary.Address.Location)
            && !string.IsNullOrWhiteSpace(Beneficiary.Address.Region);

        public bool IsEnabledToIssue => Status == GuaranteeStatus.Paid && IsBeneficiaryInfoCompleted && IsSupplierInfoCompleted;


        public void Apply(GuaranteeRequested @event)
        {
            Id = @event.Id;
            Information = new GuaranteeInformation(@event.TenderId, @event.Gloss, null);
            CurrentDateCoverage = new(@event.Start, @event.End);
            Bond = new GuaranteeBond(@event.Bond.Id, @event.Bond.Name);
            Status = GuaranteeStatus.Draft;
            Supplier = new LegalParty(
                TaxId: @event.Supplier.TaxId,
                Name: @event.Supplier.Name,
                Address: new Address(@event.Supplier.AddressStreet, @event.Supplier.AddressLocation, @event.Supplier.AddressRegion)
            );
            Beneficiary = new LegalParty(
                TaxId: @event.Beneficiary.TaxId,
                Name: @event.Beneficiary.Name,
                Address: new Address(@event.Beneficiary.AddressStreet, @event.Beneficiary.AddressLocation, @event.Beneficiary.AddressRegion)

            );
            Insurance = new GuaranteeInsurance(
                Id: @event.Insurance.Id,
                Name: @event.Insurance.Name,
                LegacyId: @event.Insurance.LegacyId
            );
            Broker = @event.Broker != null ? new GuaranteeBroker(@event.Broker.Id, @event.Broker.Name) : null;
            AmountCoverage = @event.InitialAmountCoverage;
            Version = @event.Version;
        }

        public void Apply(GuaranteeInformationUpdated @event)
        {
            Information = Information with
            {
                TenderId = @event.TenderId ?? Information.TenderId,
                Gloss = @event.Gloss ?? Information.Gloss,
            };
        }

        public void Apply(GuaranteeSupplierInformationUpdated @event)
        {

            Supplier = Supplier with
            {
                Name = @event.Name ?? Supplier.Name,
                Address = Supplier.Address with
                {
                    Street = @event.AddressStreet ??  Supplier.Address.Street,
                    Location = @event.AddressLocation ?? Supplier.Address.Location,
                    Region = @event.AddressRegion ?? Supplier.Address.Region,
                }
            };
        }

        public void Apply(GuaranteeBeneficiaryInformationUpdated @event)
        {
            Beneficiary = Beneficiary with
            {
                Name = @event.Name ?? Beneficiary.Name,
                Address = Beneficiary.Address with
                {
                    Street = @event.AddressStreet ?? Beneficiary.Address.Street,
                    Location = @event.AddressLocation ?? Beneficiary.Address.Location,
                    Region = @event.AddressRegion ?? Beneficiary.Address.Region,
                }

            };
        }

        public void Apply(GuaranteeIssued @event)
        {
            Status = GuaranteeStatus.Issued;
            Code = new GuaranteeCode(@event.CertificateNumber, @event.Code);
        }

        /// <summary>
        /// Applies the changes from the EndorsementApplied event to the GuaranteeAggregate. This method updates the relevant properties of the aggregate based on the changes specified in the event, such as gloss, end date, supplier information, and beneficiary information. It also increments the version of the aggregate to reflect the applied changes.
        /// </summary>
        /// <param name="event"></param>
        public void Apply(EndorsementApplied @event)
        {
            if(@event.Changes.Gloss is not null)
            {
                Information = Information with
                {
                    Gloss = @event.Changes.Gloss
                };
            }

            if(@event.Changes.EndDate is not null)
            {
                CurrentDateCoverage = CurrentDateCoverage with
                {
                    End = new DateTime(@event.Changes.EndDate.Value.Year, @event.Changes.EndDate.Value.Month, @event.Changes.EndDate.Value.Day)
                };
            }

            if(@event.Changes.Supplier is not null)
            {
                Supplier = Supplier with
                {
                    Name = @event.Changes.Supplier.Name ?? Supplier.Name,
                    Address = Supplier.Address with
                    {
                        Street = @event.Changes.Supplier.AddressStreet ?? Supplier.Address.Street,
                        Location = @event.Changes.Supplier.AddressLocation ?? Supplier.Address.Location,
                        Region = @event.Changes.Supplier.AddressRegion ?? Supplier.Address.Region,
                    }
                };
            }

            if(@event.Changes.Beneficiary is not null)
            {
                Beneficiary = Beneficiary with
                {
                    Name = @event.Changes.Beneficiary.Name ?? Beneficiary.Name,
                    Address = Beneficiary.Address with
                    {
                        Street = @event.Changes.Beneficiary.AddressStreet ?? Beneficiary.Address.Street,
                        Location = @event.Changes.Beneficiary.AddressLocation ?? Beneficiary.Address.Location,
                        Region = @event.Changes.Beneficiary.AddressRegion ?? Beneficiary.Address.Region,
                    }
                };
            }

            Version++;
        }
    }
    
    /// <summary>
    /// Object value representing the insurance party, which includes the tax ID, name, and an optional legacy ID from the legacy system.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Name"></param>
    /// <param name="LegacyId"></param>
    public record GuaranteeInsurance(int Id, string Name, int? LegacyId);
    public record GuaranteeBroker(int Id, string Name);

    public record GuaranteeInformation(string? TenderId, string Gloss, string? EndorsementNumber);
    public record GuaranteeCode(long Number, string Code);
    public record GuaranteeBond(int Id, string Name);
    public record LegalParty(string TaxId, string Name, Address Address);
    public record Address(string Street, string Location, string Region);

    public record Money(decimal Amount, Currency Currency)
    {
        public string ToString(string culture = "es")
        {
            CultureInfo info = new(culture);
            string formattedAmount = Amount.ToString("N", info);
            // Mostrar el monto de acuerdo a su moneda
            return Currency switch
            {
                Currency.CLP => $"{formattedAmount} CLP",
                Currency.UF => $"{formattedAmount} UF",
                Currency.USD => $"{formattedAmount} USD",
                _ => throw new ArgumentOutOfRangeException(nameof(Currency), $"Not expected currency value: {Currency}"),
            };
        }
    };
    public record DateRange(DateTime Start, DateTime End);

    public enum Currency
    {
        CLP = 0,
        UF = 1,
        USD = 2
    }

    public enum GuaranteeStatus
    {
        Draft = 0,
        Pending = 1,
        Paid = 2,
        Issued = 3,
        Finalized = 4,
        Cancelled = 5,
        RiskEvaluation = 6,
        RiskRejected = 7,
    }
}