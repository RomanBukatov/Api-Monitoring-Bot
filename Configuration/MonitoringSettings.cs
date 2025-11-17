namespace ApiMonitoringBot.Configuration;

public record MonitoringSettings
{
    public const string SectionName = "Monitoring";
    public int CheckIntervalSeconds { get; init; } = 60;
    public List<MonitoringRule> Rules { get; init; } = [];
}