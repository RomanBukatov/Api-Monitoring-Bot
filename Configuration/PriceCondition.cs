namespace ApiMonitoringBot.Configuration;

public record PriceCondition
{
    public decimal? GreaterThan { get; init; }
    public decimal? LessThan { get; init; }
}