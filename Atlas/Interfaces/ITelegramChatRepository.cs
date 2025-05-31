using Atlas.Models;

namespace Atlas.Interfaces;

public interface ITelegramChatRepository
{
    public Task<TelegramChat?> GetChatAsync(long chatId);
    public Task<bool> AnyChatAsync(long chatId);
    
    public Task EnableNotificationAsync(long chatId);
    
    public Task CancelNotificationAsync(long chatId);
    
    public Task AddChatAsync(long chatId);
    
    public Task<IEnumerable<TelegramChat>> GetAllLocalEnableChatsAsync();
}