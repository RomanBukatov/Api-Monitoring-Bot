namespace ApiMonitoringBot.BackgroundServices
{
    public class MonitoringService : BackgroundService
    {
        private readonly ILogger<MonitoringService> _logger;

        public MonitoringService(ILogger<MonitoringService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
