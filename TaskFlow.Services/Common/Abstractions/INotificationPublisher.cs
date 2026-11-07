// Common/Abstractions/INotificationPublisher.cs
namespace TaskFlow.Services.Common.Abstractions;

// التوصيل اللحظي فقط (SignalR في Web). منطق الإشعارات في NotificationService.
public interface INotificationPublisher
{
    Task PublishToUserAsync(int userId, string title, string message);
}