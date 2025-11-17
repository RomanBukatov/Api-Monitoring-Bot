namespace ApiMonitoringBot.Configuration;

public record ApiKeysSettings
{
    public const string SectionName = "ApiKeys";
    public string? OpenWeatherMap { get; init; }
}