using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiMonitoringBot.Models;

public record WeatherResponse { [JsonPropertyName("main")] public required WeatherMain Main { get; init; } [JsonPropertyName("weather")] public required List<WeatherInfo> Weather { get; init; } }