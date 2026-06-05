namespace EventSourcingPoc.API.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un endoso asociado a las garantias
    /// </summary>
    public class Endorsement
    {
        public Guid Id { get; set; }
        public DateRange DateCoverage { get; set; }
        public Money Amount { get; set; }
        public string? TenderId { get; set; }
        public string Gloss { get; set; }
    }
}
