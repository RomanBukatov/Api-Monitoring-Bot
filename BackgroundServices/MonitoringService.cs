using ApiMonitoringBot.Clients;
using ApiMonitoringBot.Configuration;
using Microsoft.Extensions.Options;

namespace ApiMonitoringBot.BackgroundServices
{
    public class MonitoringService : BackgroundService
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly BybitClient _bybitClient;
        private readonly TelegramClient _telegramClient;
        private readonly MonitoringSettings _monitoringSettings;

        public MonitoringService(
            ILogger<MonitoringService> logger,
            BybitClient bybitClient,
            TelegramClient telegramClient,
            IOptions<MonitoringSettings> monitoringOptions)
        {
            _logger = logger;
            _bybitClient = bybitClient;
            _telegramClient = telegramClient;
            _monitoringSettings = monitoringOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервис мониторинга запущен.");

            // Отправим сообщение о старте
            await _telegramClient.SendMessageAsync("🤖 Бот запущен и начинает мониторинг.", stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Получение данных из API...");

                var ticker = await _bybitClient.GetTickerAsync("BTCUSDT", stoppingToken);

                if (ticker is not null)
                {
                    // Форматируем decimal с двумя знаками после запятой
                    var message = $"🪙 {ticker.Symbol}: ${ticker.LastPrice:F2}";
                    await _telegramClient.SendMessageAsync(message, stoppingToken);
                    _logger.LogInformation("Данные по BTCUSDT отправлены в Telegram.");
                }
                else
                {
                    // Новое логирование
                    _logger.LogWarning("Не удалось получить данные по BTCUSDT. Пропускаем итерацию.");
                }

                var delay = TimeSpan.FromSeconds(_monitoringSettings.CheckIntervalSeconds);
                _logger.LogInformation("Следующая проверка через {Delay} секунд.", _monitoringSettings.CheckIntervalSeconds);
                await Task.Delay(delay, stoppingToken);
            }

            _logger.LogInformation("Сервис мониторинга остановлен.");
        }
    }
}
