using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record BybitTicker
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    // Используем JsonConverter для автоматического парсинга строки в decimal
    [JsonPropertyName("lastPrice")]
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal LastPrice { get; init; }
}