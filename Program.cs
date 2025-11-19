using ApiMonitoringBot;
using ApiMonitoringBot.BackgroundServices;
using ApiMonitoringBot.Clients;
using ApiMonitoringBot.Configuration;
using ApiMonitoringBot.Services;

Console.OutputEncoding  = System.Text.Encoding.UTF8;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Регистрируем MediatR
        // Он найдет все обработчики в нашей сборке
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        // Конфигурация
        services.Configure<TelegramSettings>(hostContext.Configuration.GetSection(TelegramSettings.SectionName));
        services.Configure<ApiKeysSettings>(hostContext.Configuration.GetSection(ApiKeysSettings.SectionName));
        services.Configure<MonitoringSettings>(hostContext.Configuration.GetSection(MonitoringSettings.SectionName));

        // Регистрируем HttpClientFactory для наших будущих API клиентов
        services.AddHttpClient();

        // Здесь мы будем регистрировать наши клиенты API
        services.AddHttpClient<BybitClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.bybit.com");
        });

        // Регистрируем клиентов
        services.AddSingleton<TelegramClient>();
        services.AddSingleton<RuleStateService>();

        // Регистрируем WeatherClient как типизированный HTTP клиент
        services.AddHttpClient<WeatherClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org");
        });

        // Регистрируем наш фоновый сервис
        services.AddHostedService<MonitoringService>();
    })
    .Build()
    .Run();