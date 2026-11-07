// Common/Abstractions/IEmailSender.cs
namespace TaskFlow.Services.Common.Abstractions;

// التنفيذ في TaskFlow.Web
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}