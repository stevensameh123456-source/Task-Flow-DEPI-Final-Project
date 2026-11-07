using TaskFlow.Services.Common.Abstractions;

namespace TaskFlow.Services.Common;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}