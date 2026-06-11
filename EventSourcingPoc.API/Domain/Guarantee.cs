using EventSourcingPoc.API.Events;
using ImTools;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace EventSourcingPoc.API.Domain
{
    public class Guarantee : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string? TenderId { get; private set; }
        public string Gloss { get; private set; } = string.Empty;
        public DateRange CurrentDateCoverage { get; private set; }
        public Money CurrentAmountCoverage { get; private set; }
        public GuaranteePurpose Purpose { get; private set; }
        public GuaranteeStatus Status {  get; private set; }
        public LegalParty Supplier { get; private set; }
        public LegalParty Beneficiary { get; private set; }
        public Insurance Insurance { get; private set; }

        private List<GuaranteeDocument> _documents = new();
        public IReadOnlyCollection<GuaranteeDocument> Documents => _documents.AsReadOnly();


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

        public Guarantee() { }

        public void Apply(GuaranteeRequested @event)
        {
            Id = @event.Id;
            TenderId = @event.TenderId;
            Purpose = @event.Purpose;
            Status = GuaranteeStatus.Draft;
            Supplier = @event.Supplier;
            Beneficiary = @event.Beneficiary;
            Gloss = @event.Gloss;
            CurrentAmountCoverage = @event.InitialAmountCoverage;
        }


        public void Apply(GuaranteeIssued @event)
        {
            Status = GuaranteeStatus.Issued;
        }
    }

    public record GuaranteeNumber(long Number, string Code);

    public record GuaranteeDocument(string Type, string Uri);

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

    public enum GuaranteePurpose
    {
        BidGuarantee,
        PerformanceGuarantee,
        AdvancePayment       
    }

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
        public static string EnumValue(this GuaranteePurpose purpose, string culture = "es")
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