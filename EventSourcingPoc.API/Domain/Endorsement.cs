namespace EventSourcingPoc.API.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un endoso asociado a las garantias
    /// </summary>
    public class Endorsement
    {
        public Guid Id { get; private set; }
        public Guid GuaranteeId { get; private set; }
    }
}
