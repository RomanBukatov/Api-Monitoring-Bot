using ApiMonitoringBot.Configuration;
using ApiMonitoringBot.Handlers.Monitoring;
using MediatR;
using Microsoft.Extensions.Options;

namespace ApiMonitoringBot.BackgroundServices;

public class MonitoringService : BackgroundService
{
    private readonly ILogger<MonitoringService> _logger;
    private readonly IMediator _mediator;
    private readonly MonitoringSettings _monitoringSettings;

    public MonitoringService(
        ILogger<MonitoringService> logger,
        IMediator mediator,
        IOptions<MonitoringSettings> monitoringOptions)
    {
        _logger = logger;
        _mediator = mediator;
        _monitoringSettings = monitoringOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Сервис мониторинга запущен.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Публикую уведомление CheckRules для запуска проверки.");

            // Просто публикуем уведомление. Всю работу сделает MediatR и его обработчик.
            await _mediator.Publish(new CheckRules(), stoppingToken);

            var delay = TimeSpan.FromSeconds(_monitoringSettings.CheckIntervalSeconds);
            _logger.LogInformation("Следующая проверка через {Delay} секунд.", _monitoringSettings.CheckIntervalSeconds);
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("Сервис мониторинга остановлен.");
    }
}
