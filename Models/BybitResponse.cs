using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record BybitResponse
{
    [JsonPropertyName("result")]
    public required BybitResult Result { get; init; }
}