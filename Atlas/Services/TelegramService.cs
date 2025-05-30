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
            var chats = await _telegramChatRepository.GetAllLocalChatsAsync();
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
            new BotCommand { Command = "start", Description = "Register with the bot" }
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
            }
            else
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "You are already registered!",
                    cancellationToken: cancellationToken
                );
            }
        }
        else if (message.Text.StartsWith("/stop"))
        {
            // if (!await _telegramChatRepository.AnyChatAsync(chatId))
            // {
            //     await _telegramChatRepository.AddChatAsync(chatId);
            //     await botClient.SendMessage(
            //         chatId: chatId,
            //         text: "Welcome! You are now registered with the bot.",
            //         cancellationToken: cancellationToken
            //     );
            // }
            // else
            // {
            //     await botClient.SendMessage(
            //         chatId: chatId,
            //         text: "You are already registered!",
            //         cancellationToken: cancellationToken
            //     );
            // }
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