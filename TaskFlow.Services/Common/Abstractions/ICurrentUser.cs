// Common/Abstractions/ICurrentUser.cs
namespace TaskFlow.Services.Common.Abstractions;

public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}