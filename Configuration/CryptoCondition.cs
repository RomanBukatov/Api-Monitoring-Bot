namespace ApiMonitoringBot.Configuration;

public record CryptoCondition
{
    public required string Symbol { get; init; }
    public PriceCondition? Price { get; init; }
}