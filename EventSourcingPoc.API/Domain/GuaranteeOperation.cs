namespace EventSourcingPoc.API.Domain
{

    public interface IEvalution
    {

    }

    /// <summary>
    /// GarantiaOperation representa una operación relacionada con una garantía, como la creación, emisión, pago, etc.
    /// </summary>
    public abstract class GuaranteeOperation
    {
        public Money Price { get; private set; }
        public OperationStatus Status { get; private set; }
    }

    /// <summary>
    /// GarantiaApplication representa la solicitud de una garantía, incluyendo detalles como el período de cobertura, el cliente, el beneficiario y el propósito de la garantía.
    /// </summary>
    public class GuaranteeApplication : GuaranteeOperation
    {
        public string? TenderId { get; private set; }
        public DateRange CoveragePeriod { get; private set; }
        public Money Amount { get; private set; }
        public LegalParty Customer { get; private set; }
        public LegalParty Beneficiary { get; private set; }
        public string Gloss { get; private set; }
        public GuaranteeBond Purpose { get; private set; }
    }

    /// <summary>
    /// GarantiaEndorsement representa una endoso de garantía, que puede incluir detalles como el período de cobertura, el endosante y el propósito del endoso.
    /// </summary>
    public class GuaranteeEndorsement : GuaranteeOperation
    {
        public DateRange CoveragePeriod { get; private set; }
        public Guid EndorserId { get; private set; }
    }

    public enum OperationStatus
    {
        Draft = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Paid = 4,
        Canceled = 5,
    }
}
