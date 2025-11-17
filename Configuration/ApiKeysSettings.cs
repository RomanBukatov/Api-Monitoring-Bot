namespace ApiMonitoringBot.Configuration;

public record ApiKeysSettings
{
    public const string SectionName = "ApiKeys";
    public required string OpenWeatherMap { get; init; }
}