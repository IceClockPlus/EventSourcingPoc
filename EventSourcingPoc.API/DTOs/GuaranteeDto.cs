namespace EventSourcingPoc.API.DTOs
{

    public record GuaranteeDto(
        Guid Id,
        DateTime Start,
        DateTime End,
        decimal Amount,
        string CurrencyAmount,
        string? TenderId,
        GuaranteeBondDto Bond
    );
    public record GuaranteeBondDto(int Id, string Name);
}
