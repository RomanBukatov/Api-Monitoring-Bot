using ApiMonitoringBot.Clients;
using ApiMonitoringBot.Configuration;
using ApiMonitoringBot.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace ApiMonitoringBot.Handlers.Monitoring;

public class CheckRulesHandler : INotificationHandler<CheckRules>
{
    private readonly ILogger<CheckRulesHandler> _logger;
    private readonly MonitoringSettings _settings;
    private readonly BybitClient _bybitClient;
    private readonly WeatherClient _weatherClient; // Добавили
    private readonly TelegramClient _telegramClient;
    private readonly RuleStateService _ruleStateService;

    public CheckRulesHandler(
        ILogger<CheckRulesHandler> logger,
        IOptions<MonitoringSettings> settings,
        BybitClient bybitClient,
        WeatherClient weatherClient, // Добавили
        TelegramClient telegramClient,
        RuleStateService ruleStateService)
    {
        _logger = logger;
        _settings = settings.Value;
        _bybitClient = bybitClient;
        _weatherClient = weatherClient; // Добавили
        _telegramClient = telegramClient;
        _ruleStateService = ruleStateService;
    }

    public async Task Handle(CheckRules notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинаю проверку {RuleCount} правил.", _settings.Rules.Count);

        foreach (var rule in _settings.Rules)
        {
            // Проверяем, какого типа это правило
            if (rule.Conditions.Crypto is not null)
            {
                await CheckCryptoRule(rule, cancellationToken);
            }
            else if (rule.Conditions.Weather is not null)
            {
                await CheckWeatherRule(rule, cancellationToken);
            }
        }
    }

    private async Task CheckCryptoRule(MonitoringRule rule, CancellationToken cancellationToken)
    {
        var cryptoCondition = rule.Conditions.Crypto!;
        
        var ticker = await _bybitClient.GetTickerAsync(cryptoCondition.Symbol, cancellationToken);
        if (ticker is null) return;

        bool conditionMet = false;
        if (cryptoCondition.Price?.GreaterThan is not null && ticker.LastPrice > cryptoCondition.Price.GreaterThan) conditionMet = true;
        if (cryptoCondition.Price?.LessThan is not null && ticker.LastPrice < cryptoCondition.Price.LessThan) conditionMet = true;

        await ProcessRuleResult(rule, conditionMet, "${Crypto.Price:F2}", ticker.LastPrice.ToString("F2"), cancellationToken);
    }

    private async Task CheckWeatherRule(MonitoringRule rule, CancellationToken cancellationToken)
    {
        var weatherCondition = rule.Conditions.Weather!;

        var weather = await _weatherClient.GetCurrentWeatherAsync(weatherCondition.City, cancellationToken);
        if (weather is null) return;

        bool conditionMet = false;
        var temp = weather.Main.Temperature;

        if (weatherCondition.Temperature?.GreaterThan is not null && temp > weatherCondition.Temperature.GreaterThan) conditionMet = true;
        if (weatherCondition.Temperature?.LessThan is not null && temp < weatherCondition.Temperature.LessThan) conditionMet = true;

        await ProcessRuleResult(rule, conditionMet, "${Weather.Temperature:F1}", temp.ToString("F1"), cancellationToken);
    }

    // Вынесли общую логику отправки и проверки состояния в отдельный метод, чтобы не дублировать код
    private async Task ProcessRuleResult(MonitoringRule rule, bool conditionMet, string placeholder, string value, CancellationToken cancellationToken)
    {
        bool isAlreadyTriggered = _ruleStateService.IsTriggered(rule.RuleName);

        if (conditionMet && !isAlreadyTriggered)
        {
            _logger.LogInformation("Правило '{RuleName}' сработало!", rule.RuleName);
            var message = rule.MessageTemplate.Replace(placeholder, value);
            await _telegramClient.SendMessageAsync(message, cancellationToken);
            _ruleStateService.SetTriggered(rule.RuleName);
        }
        else if (!conditionMet && isAlreadyTriggered)
        {
            _ruleStateService.Reset(rule.RuleName);
        }
    }
}