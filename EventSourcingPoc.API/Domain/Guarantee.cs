using EventSourcingPoc.API.Events;
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

        private readonly List<GuaranteeEndorsement> _endorsement = new();
        public IReadOnlyList<GuaranteeEndorsement> Endorsements => _endorsement.AsReadOnly();


        private readonly List<object> _uncommittedEvents = new();
        public IReadOnlyCollection<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        private void RaiseEvent(object @event)
        {
            _uncommittedEvents.Add(@event);
            switch (@event)
            {
                case GuaranteeCreated e: Apply(e); break;
                case GuaranteePriceConfirmed e: Apply(e); break;
                default:
                    throw new InvalidOperationException();
            }
        }

        //public void ConfirmPrice(Money money, int endorsementSequence)
        //{
        //    var @event = new GuaranteePriceConfirmed(endorsementSequence, money);
        //    RaiseEvent(@event);
        //}

        public Guarantee(){}

        public void Apply(GuaranteeCreated @event)
        {
            Id = @event.Id;
            TenderId = @event.TenderId;
            Purpose = @event.Purpose;
            Status = GuaranteeStatus.Draft;
            Supplier = @event.Supplier;
            Beneficiary = @event.Beneficiary;
            Gloss = @event.Gloss;
            //var initialEndorsement = new GuaranteeEndorsement(
            //    Sequence: 0,
            //    EndorsementNumber: 0,
            //    Cost: @event.Cost,
            //    CurrentAmountCoverage = @event.InitialAmountCoverage,
            //    CurrentDateCoverage = @event.InitialDateCoverage,
            //    IssuedAt: DateTime.UtcNow,
            //    Documents: []
            //);
            //_endorsement.Add(initialEndorsement);
            //UpdateCurrentState(initialEndorsement);
        }

        public void Apply(GuaranteePriceConfirmed @event)
        {
            // Apply change on the sequence endorsement selected
            var idx = _endorsement.FindIndex(e => e.Sequence == @event.Sequence);
            if( idx < 0 )
                throw new ArgumentNullException("Endoso de garantia no encontrado");
            var endorsement = _endorsement[idx];

            var updatedEndorsement = endorsement with { Cost =  @event.Cost };
            _endorsement[idx] = updatedEndorsement;
        }

        private void UpdateCurrentState(GuaranteeEndorsement endorsement)
        {
            CurrentDateCoverage = endorsement.DateCoverage;
            CurrentAmountCoverage = endorsement.AmountCoverage;
        }
    }

    public record GuaranteeNumber(long Number, string Code);

    public record GuaranteeEndorsement(
        int Sequence,
        int EndorsementNumber,
        Money Cost,
        Money AmountCoverage,
        DateRange DateCoverage,
        DateTime IssuedAt,
        IEnumerable<GuaranteeDocument> Documents
    );

    public record LegalParty(string TaxId, string Name, Address Address);
    public record Address(string Street, string Location, string Area);

    public record GuaranteeDocument(string Type, string Uri);

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