using System.ComponentModel.DataAnnotations;

namespace Atlas.Models;

public class TelegramChat
{
    [Key]
    public long ChatId { get; set; }

    public bool CanSendNotification { get; set; } = true;
    
    public DateTime RegisteredAt { get; init; } = DateTime.Now;

    public TelegramChat(){}

    public TelegramChat(long chatId)
    {
        ChatId = chatId;
    }
}