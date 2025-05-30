namespace Atlas.Interfaces;

public interface INotificationService
{
    public Task Notify(string message);
}