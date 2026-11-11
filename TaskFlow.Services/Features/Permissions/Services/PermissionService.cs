// Features/Permissions/Services/PermissionService.cs
using Microsoft.AspNetCore.Identity;
using TaskFlow.Data.Entities;
using TaskFlow.Services.Common.Exceptions;
using TaskFlow.Services.Features.Permissions.DTOs;
using TaskFlow.Services.Features.Permissions.Interfaces;
using TaskFlow.Services.Features.Permissions.Policies;

namespace TaskFlow.Services.Features.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly UserManager<ApplicationUser> _users;

    public PermissionService(UserManager<ApplicationUser> users) => _users = users;

    public async Task<IReadOnlyList<string>> GetRolesAsync(int userId)
    {
        var user = await _users.FindByIdAsync(userId.ToString());
        if (user is null) return Array.Empty<string>();
        return (await _users.GetRolesAsync(user)).ToList();
    }

    public async Task<IReadOnlyList<JobRoleDto>> GetJobRolesAsync(int userId)
        => (await GetRolesAsync(userId)).Select(r => new JobRoleDto(userId, r)).ToList();

    public async Task<bool> HasRoleAsync(int userId, string role)
    {
        var user = await _users.FindByIdAsync(userId.ToString());
        return user is not null && await _users.IsInRoleAsync(user, role);
    }

    public async Task<bool> HasPermissionAsync(int userId, string permission)
        => RolePermissionMatrix.HasPermission(await GetRolesAsync(userId), permission);

    public async Task<PermissionResultDto> CheckAsync(int userId, string permission)
    {
        var allowed = await HasPermissionAsync(userId, permission);
        return new PermissionResultDto(allowed, allowed ? null : $"Missing permission '{permission}'.");
    }

    public async Task EnsureRoleAsync(int userId, string role)
    {
        if (!await HasRoleAsync(userId, role))
            throw new UnauthorizedOperationException($"This action requires the '{role}' role.");
    }

    public async Task EnsurePermissionAsync(int userId, string permission)
    {
        if (!await HasPermissionAsync(userId, permission))
            throw new UnauthorizedOperationException($"You do not have the permission '{permission}'.");
    }

    public Task<bool> CanCreateTaskAsync(int userId)
        => HasPermissionAsync(userId, AppPermissions.CreateTasks);

    public Task<bool> CanManageProjectAsync(int userId)
        => HasPermissionAsync(userId, AppPermissions.ManageProjects);

    public Task<bool> CanReviewTaskAsync(int userId)
        => HasPermissionAsync(userId, AppPermissions.ReviewTasks);
}