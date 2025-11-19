using ApiMonitoringBot.Configuration;
using ApiMonitoringBot.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ApiMonitoringBot.Clients;

public class WeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherClient> _logger;
    private readonly string _apiKey;

    public WeatherClient(HttpClient httpClient, ILogger<WeatherClient> logger, IOptions<ApiKeysSettings> apiKeys)
    {
        _httpClient = httpClient;
        _logger = logger;
        // Если ключа нет, кидаем ошибку сразу при старте
        _apiKey = apiKeys.Value.OpenWeatherMap ?? throw new InvalidOperationException("API key for OpenWeatherMap is not set.");
    }

    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        try
        {
            // Запрашиваем погоду: метрическая система (Цельсий), язык русский
            return await _httpClient.GetFromJsonAsync<WeatherResponse>(
                $"data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=ru", 
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при запросе погоды для города {City}", city);
            return null;
        }
    }
}