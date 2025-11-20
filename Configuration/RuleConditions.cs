namespace ApiMonitoringBot.Configuration;

public record RuleConditions
{
    public CryptoCondition? Crypto { get; init; }
    public WeatherCondition? Weather { get; init; } // Добавили
}