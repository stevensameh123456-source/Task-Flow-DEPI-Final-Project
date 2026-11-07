// Common/Results/ServiceResult.cs
namespace TaskFlow.Services.Common.Results;

public class ServiceResult
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }

    public static ServiceResult Success() => new() { IsSuccess = true };
    public static ServiceResult Failure(string error) => new() { IsSuccess = false, Error = error };
}