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
    private readonly TelegramClient _telegramClient;
    private readonly RuleStateService _ruleStateService; // Добавили "память"

    public CheckRulesHandler(
        ILogger<CheckRulesHandler> logger,
        IOptions<MonitoringSettings> settings,
        BybitClient bybitClient,
        TelegramClient telegramClient,
        RuleStateService ruleStateService) // Добавили "память"
    {
        _logger = logger;
        _settings = settings.Value;
        _bybitClient = bybitClient;
        _telegramClient = telegramClient;
        _ruleStateService = ruleStateService; // Добавили "память"
    }

    public async Task Handle(CheckRules notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинаю проверку {RuleCount} правил.", _settings.Rules.Count);

        foreach (var rule in _settings.Rules)
        {
            await CheckCryptoRule(rule, cancellationToken);
        }
    }

    private async Task CheckCryptoRule(MonitoringRule rule, CancellationToken cancellationToken)
    {
        var cryptoCondition = rule.Conditions.Crypto;
        if (cryptoCondition is null) return;

        var ticker = await _bybitClient.GetTickerAsync(cryptoCondition.Symbol, cancellationToken);
        if (ticker is null)
        {
            _logger.LogWarning("Не удалось получить данные для {Symbol} для правила '{RuleName}'", cryptoCondition.Symbol, rule.RuleName);
            return;
        }

        bool conditionMet = false; // Выполняется ли условие ПРЯМО СЕЙЧАС
        if (cryptoCondition.Price?.GreaterThan is not null && ticker.LastPrice > cryptoCondition.Price.GreaterThan)
        {
            conditionMet = true;
        }
        if (cryptoCondition.Price?.LessThan is not null && ticker.LastPrice < cryptoCondition.Price.LessThan)
        {
            conditionMet = true;
        }

        bool isAlreadyTriggered = _ruleStateService.IsTriggered(rule.RuleName); // Какое состояние было в ПРОШЛЫЙ РАЗ

        // Условие для отправки: условие выполнено СЕЙЧАС, но НЕ БЫЛО выполнено в прошлый раз
        if (conditionMet && !isAlreadyTriggered)
        {
            _logger.LogInformation("Правило '{RuleName}' сработало! Цена {Symbol} = {Price}", rule.RuleName, ticker.Symbol, ticker.LastPrice);

            var message = rule.MessageTemplate.Replace("${Crypto.Price:F2}", ticker.LastPrice.ToString("F2"));
            await _telegramClient.SendMessageAsync(message, cancellationToken);

            _ruleStateService.SetTriggered(rule.RuleName); // Запоминаем, что отправили уведомление
        }
        // Условие для сброса: условие НЕ выполнено СЕЙЧАС, но БЫЛО выполнено в прошлый раз
        else if (!conditionMet && isAlreadyTriggered)
        {
            _logger.LogInformation("Условие для правила '{RuleName}' больше не выполняется. Сбрасываю состояние.", rule.RuleName);
            _ruleStateService.Reset(rule.RuleName); // Сбрасываем, чтобы оно могло сработать снова
        }
    }
}