namespace Atlas.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(string message);

    Task StartReceivingAsync();
}