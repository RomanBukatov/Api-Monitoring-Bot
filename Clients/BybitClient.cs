using ApiMonitoringBot.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApiMonitoringBot.Clients;

public class BybitClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BybitClient> _logger;

    public BybitClient(HttpClient httpClient, ILogger<BybitClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BybitTicker?> GetTickerAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            // Используем новый GetFromJsonAsync с генераторами исходного кода для производительности
            var response = await _httpClient.GetFromJsonAsync<BybitResponse>(
                $"v5/market/tickers?category=linear&symbol={symbol}",
                cancellationToken);

            return response?.Result?.List?.FirstOrDefault();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка парсинга JSON при запросе тикера {Symbol} из Bybit API", symbol);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при запросе тикера {Symbol} из Bybit API", symbol);
            return null;
        }
    }
}