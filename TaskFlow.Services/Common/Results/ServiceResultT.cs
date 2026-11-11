// Common/Results/ServiceResultT.cs
namespace TaskFlow.Services.Common.Results;

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public new static ServiceResult<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}