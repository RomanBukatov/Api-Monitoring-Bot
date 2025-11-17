using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record BybitTicker
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("lastPrice")]
    public required string LastPrice { get; init; }
}