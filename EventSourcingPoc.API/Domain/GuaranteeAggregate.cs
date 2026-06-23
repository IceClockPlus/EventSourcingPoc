using EventSourcingPoc.API.Events;
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
        public InsuranceParty Insurance { get; private set; } = null!;

        private readonly List<object> _uncommittedEvents = new();
        public IReadOnlyCollection<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        private void RaiseEvent(object @event)
        {
            _uncommittedEvents.Add(@event);
            switch (@event)
            {
                case GuaranteeRequested e: Apply(e); break;
                //case GuaranteePriceConfirmed e: Apply(e); break;
                default:
                    throw new InvalidOperationException();
            }
        }


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
            AmountCoverage = @event.InitialAmountCoverage;
        }

        public void Apply(GuaranteeIssued @event)
        {
            Status = GuaranteeStatus.Issued;
        }
    }
    
    /// <summary>
    /// Object value representing the insurance party, which includes the tax ID, name, and an optional legacy ID from the legacy system.
    /// </summary>
    /// <param name="TaxId"></param>
    /// <param name="Name"></param>
    /// <param name="LegacyId"></param>
    public record InsuranceParty(string TaxId, string Name, int? LegacyId);
    public record GuaranteeInformation(string? TenderId, string Gloss, string? EndorsementNumber);
    public record GuaranteeCode(long Number, string Code);
    public record GuaranteeBond(int Id, string Name);
    public record LegalParty(string TaxId, string Name, Address Address);
    public record Address(string Street, string Location, string Area);

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

    public enum Insurance
    {
        Internal = 0
    }

    public enum EndorsementType
    {
        InitialIssuance = 0,
        Extension = 1,
        ValueAdjustment = 2,
        Correction = 3,
    }

    //public enum GuaranteeBond
    //{
    //    BidBond = 0,
    //    PerformanceBond = 1,
    //    DefectLiabilityBond = 2,
    //    PerformanceAndDefectLiabilityBond = 3,
    //    MaintenanceAndOperationalPerformanceBond = 4,
    //    AdvancePaymentBond = 5,
    //    RetentionMoneyBond = 6         
    //}

    public enum GuaranteeStatus
    {
        Draft = 0,
        Pending = 1,
        Paid = 2,
        Issued = 3,
        Finalized = 4,
        Cancelled = 5,
    }

    public static class GuaranteeExtensionMethods
    {
        private static readonly ResourceManager _resourceManager = new("EventSourcingPoc.API.Resources.GuaranteeResources", Assembly.GetExecutingAssembly());
        public static string EnumValue(this GuaranteeBond purpose, string culture = "es")
        {
            //Crear cultura
            string llaveRecurso = purpose.ToString();
            CultureInfo cultureInfo = new CultureInfo(culture);

            // Obtener el nombre de la finalidad de la garantía desde el recurso utilizando la cultura especificada
            var nombreFinalidad = _resourceManager.GetString(llaveRecurso, cultureInfo);
            return nombreFinalidad ?? throw new ArgumentNullException($"No se encontró el recurso para la finalidad de garantía: {purpose} con cultura: {culture}");
        }
    }
}