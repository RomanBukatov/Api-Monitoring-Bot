using System.Collections.Concurrent;

namespace ApiMonitoringBot.Services;

/// <summary>
/// Сервис для хранения состояния правил (сработали они или нет).
/// Потокобезопасный Singleton.
/// </summary>
public class RuleStateService
{
    // Словарь для хранения состояний: RuleName -> isTriggered
    private readonly ConcurrentDictionary<string, bool> _triggeredRules = new();

    /// <summary>
    /// Проверяет, было ли правило уже активировано.
    /// </summary>
    public bool IsTriggered(string ruleName)
    {
        return _triggeredRules.GetValueOrDefault(ruleName, false);
    }

    /// <summary>
    /// Помечает правило как активированное.
    /// </summary>
    public void SetTriggered(string ruleName)
    {
        _triggeredRules[ruleName] = true;
    }

    /// <summary>
    /// Сбрасывает состояние правила (когда условие больше не выполняется).
    /// </summary>
    public void Reset(string ruleName)
    {
        _triggeredRules[ruleName] = false;
    }
}