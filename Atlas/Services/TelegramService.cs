using Atlas.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Atlas.Services;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITelegramChatRepository _telegramChatRepository;

    public TelegramService(IConfiguration configuration, ITelegramChatRepository telegramChatRepository)
    {
        _telegramChatRepository = telegramChatRepository;
        var botToken = configuration["Telegram:BotToken"];
        if (string.IsNullOrEmpty(botToken))
            throw new ArgumentNullException(nameof(botToken));

        _botClient = new TelegramBotClient(botToken);
    }

    public async Task SendMessageAsync(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        try
        {
            var chats = await _telegramChatRepository.GetAllLocalEnableChatsAsync();
            foreach (var chat in chats)
                await _botClient.SendMessage(
                    chatId: new ChatId(chat.ChatId),
                    text: message,
                    parseMode: ParseMode.Markdown
                );
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send Telegram message: {ex.Message}", ex);
        }
    }

    public async Task StartReceivingAsync()
    {
        await _botClient.SetMyCommands([
            new BotCommand { Command = "start", Description = "Регистрацию или включение уведомлений" },
            new BotCommand { Command = "stop", Description = "Отключение уведомлений" }
        ]);

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync
        );
    }

    // Обработка входящих обновлений
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;

        if (message.Text.StartsWith("/start"))
        {
            if (!await _telegramChatRepository.AnyChatAsync(chatId))
            {
                await _telegramChatRepository.AddChatAsync(chatId);
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Welcome! You are now registered with the bot.",
                    cancellationToken: cancellationToken
                );
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Чтобы отменить отправку сообщений в telegram, отправьте команду /stop",
                    cancellationToken: cancellationToken
                );
                return;
            }

            await _telegramChatRepository.EnableNotificationAsync(chatId);

            await botClient.SendMessage(
                chatId: chatId,
                text: "Уведомления через telegram успешно включены)",
                cancellationToken: cancellationToken
            );
        }
        else if (message.Text.StartsWith("/stop"))
        {
            if (!await _telegramChatRepository.AnyChatAsync(chatId))
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Вас нет в системе. Для регистрации отправьте команду /start",
                    cancellationToken: cancellationToken
                );
                return;
            }

            await _telegramChatRepository.CancelNotificationAsync(chatId);

            await botClient.SendMessage(
                chatId: chatId,
                text: "Уведомления через telegram успешно выключены(",
                cancellationToken: cancellationToken
            );
        }
    }

    // Обработка ошибок polling
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Polling error: {exception.Message}");
        return Task.CompletedTask;
    }
}