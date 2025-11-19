using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record WeatherMain { [JsonPropertyName("temp")] public decimal Temperature { get; init; } }