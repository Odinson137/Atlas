using Atlas.Data;
using Atlas.Interfaces;
using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Repositories;

public class TelegramChatRepository(ApplicationDbContext context) : ITelegramChatRepository
{
    public Task<TelegramChat?> GetChatAsync(long chatId) =>
        context.TelegramChats.SingleOrDefaultAsync(c => c.ChatId == chatId);

    public Task<bool> AnyChatAsync(long chatId) => context.TelegramChats.AnyAsync(c => c.ChatId == chatId);

    public async Task AddChatAsync(long chatId)
    {
        await context.TelegramChats.AddAsync(new TelegramChat(chatId));
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TelegramChat>> GetAllLocalChatsAsync() => await context.TelegramChats.ToListAsync();
}