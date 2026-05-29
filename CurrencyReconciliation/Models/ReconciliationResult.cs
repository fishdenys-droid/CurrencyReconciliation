public class ReconciliationResult
{
    public string CurrencyCode { get; set; } = string.Empty;

    public decimal CbrRate { get; set; }

    public decimal ApiRate { get; set; }

    public decimal SpreadPercent { get; set; }
}