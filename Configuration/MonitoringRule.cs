namespace ApiMonitoringBot.Configuration;

public record MonitoringRule
{
    public required string RuleName { get; init; }
    public required RuleConditions Conditions { get; init; }
    public required string MessageTemplate { get; init; }
}