// Common/Abstractions/IClock.cs
namespace TaskFlow.Services.Common.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}