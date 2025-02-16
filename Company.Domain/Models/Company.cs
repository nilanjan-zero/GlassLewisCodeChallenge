namespace Company.Domain.Models
{
    public record Company(
        int Id,
        string Name,
        string Exchange,
        string Ticker,
        string ISIN,
        string Website,
        DateTime CreatedOn,
        DateTime? UpdatedOn
    );
}
