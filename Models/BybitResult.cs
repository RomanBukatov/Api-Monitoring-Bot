using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record BybitResult
{
    [JsonPropertyName("list")]
    public required List<BybitTicker> List { get; init; }
}