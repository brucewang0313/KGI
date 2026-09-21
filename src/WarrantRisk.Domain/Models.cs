namespace WarrantRisk.Domain;

public sealed class Warrant
{
    public string WarrantId { get; set; } = string.Empty;
    public decimal StrikePrice { get; set; }
    public decimal ConversionRatio { get; set; }
    public string WarrantType { get; set; } = string.Empty;
    public int PositionQty { get; set; }
}

public sealed class TrialLog
{
    public int LogId { get; set; }
    public string WarrantId { get; set; } = string.Empty;
    public decimal MarketPrice { get; set; }
    public decimal TheoryPrice { get; set; }
    public decimal HedgeQty { get; set; }
    public DateTime CreatedTime { get; set; }
}

public sealed record TrialRequest(decimal MarketPrice);
public sealed record TrialResult(string WarrantId, decimal MarketPrice, decimal TheoryPrice, decimal HedgeQty, decimal Delta);

public static class WarrantCalculator
{
    public static TrialResult Calculate(Warrant warrant, decimal marketPrice)
    {
        if (marketPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(marketPrice), "標的股價必須大於 0。");
        }

        if (warrant.ConversionRatio < 0 || warrant.PositionQty < 0)
        {
            throw new InvalidOperationException("權證基本資料不可為負值。");
        }

        bool isCall = warrant.WarrantType.Equals("CALL", StringComparison.OrdinalIgnoreCase);
        bool isPut = warrant.WarrantType.Equals("PUT", StringComparison.OrdinalIgnoreCase);
        if (!isCall && !isPut)
        {
            throw new InvalidOperationException("權證類型必須是 CALL 或 PUT。");
        }

        decimal intrinsic = isCall
            ? marketPrice - warrant.StrikePrice
            : warrant.StrikePrice - marketPrice;
        decimal theoryPrice = Math.Max(0m, intrinsic * warrant.ConversionRatio);

        decimal delta;
        if (marketPrice == warrant.StrikePrice)
        {
            delta = 0.5m;
        }
        else if ((isCall && marketPrice > warrant.StrikePrice)
                 || (isPut && marketPrice < warrant.StrikePrice))
        {
            delta = 0.8m;
        }
        else
        {
            delta = 0.2m;
        }

        decimal hedgeQty = warrant.PositionQty * warrant.ConversionRatio * delta;
        return new TrialResult(
            warrant.WarrantId,
            marketPrice,
            decimal.Round(theoryPrice, 4),
            decimal.Round(hedgeQty, 2),
            delta);
    }
}
