using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record WeatherInfo { [JsonPropertyName("description")] public required string Description { get; init; } }