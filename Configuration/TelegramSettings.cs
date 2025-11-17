namespace ApiMonitoringBot.Configuration;

public record TelegramSettings
{
    public const string SectionName = "Telegram";
    public required string BotToken { get; init; }
    public required string ChatId { get; init; }
}