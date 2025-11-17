using ApiMonitoringBot.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace ApiMonitoringBot.Clients;

public class TelegramClient
{
    private readonly TelegramBotClient _botClient;
    private readonly TelegramSettings _settings;
    private readonly ILogger<TelegramClient> _logger;

    public TelegramClient(IOptions<TelegramSettings> settings, ILogger<TelegramClient> logger)
    {
        _settings = settings.Value;
        _botClient = new TelegramBotClient(_settings.BotToken);
        _logger = logger;
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _botClient.SendMessage(
                chatId: _settings.ChatId,
                text: message,
                cancellationToken: cancellationToken);
            
            _logger.LogInformation("Сообщение успешно отправлено в Telegram.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в Telegram.");
        }
    }
}