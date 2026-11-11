// Features/Permissions/Interfaces/IPermissionService.cs
using TaskFlow.Services.Features.Permissions.DTOs;

namespace TaskFlow.Services.Features.Permissions.Interfaces;

public interface IPermissionService
{
    Task<IReadOnlyList<string>> GetRolesAsync(int userId);
    Task<IReadOnlyList<JobRoleDto>> GetJobRolesAsync(int userId);
    Task<bool> HasRoleAsync(int userId, string role);
    Task<bool> HasPermissionAsync(int userId, string permission);
    Task<PermissionResultDto> CheckAsync(int userId, string permission);

    Task EnsureRoleAsync(int userId, string role);
    Task EnsurePermissionAsync(int userId, string permission);

    Task<bool> CanCreateTaskAsync(int userId);     // TeamLeader فقط
    Task<bool> CanManageProjectAsync(int userId);  // ProjectManager
    Task<bool> CanReviewTaskAsync(int userId);     // TeamLeader أو ProjectManager
}