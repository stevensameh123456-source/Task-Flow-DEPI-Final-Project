// Common/Abstractions/IAuditLogger.cs
namespace TaskFlow.Services.Common.Abstractions;

public interface IAuditLogger
{
    Task LogAsync(string action, string entityName, int entityId);
}