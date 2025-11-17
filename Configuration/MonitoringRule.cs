namespace ApiMonitoringBot.Configuration;

// Пока оставим правила пустыми, просто как заглушку.
// Мы их наполним позже.
public record MonitoringRule
{
    public required string RuleName { get; init; }
}