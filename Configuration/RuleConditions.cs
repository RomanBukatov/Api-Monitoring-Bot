namespace ApiMonitoringBot.Configuration;

public record RuleConditions
{
    public CryptoCondition? Crypto { get; init; }
}