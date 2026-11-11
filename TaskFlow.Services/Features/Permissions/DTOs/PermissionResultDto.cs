// Features/Permissions/DTOs/PermissionResultDto.cs
namespace TaskFlow.Services.Features.Permissions.DTOs;

public record PermissionResultDto(bool Allowed, string? Reason);