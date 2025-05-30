using Atlas.Interfaces;

namespace Atlas.Services;

public class TelegramNotificationService : INotificationService
{
    private readonly ITelegramService _telegramService;

    public TelegramNotificationService(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public Task Notify(string message) => _telegramService.SendMessageAsync(message);
}