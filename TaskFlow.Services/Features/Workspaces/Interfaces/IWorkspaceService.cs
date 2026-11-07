// Features/Workspaces/Interfaces/IWorkspaceService.cs
using TaskFlow.Data.Entities;
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Workspaces.DTOs;

namespace TaskFlow.Services.Features.Workspaces.Interfaces;

public interface IWorkspaceService
{
    Task<ServiceResult<WorkspaceResponseDto>> CreateAsync(CreateWorkspaceDto dto);
    Task<ServiceResult<WorkspaceResponseDto>> UpdateAsync(int id, UpdateWorkspaceDto dto);
    Task<ServiceResult<WorkspaceResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<WorkspaceResponseDto>>> GetMineAsync();
    Task<ServiceResult<IReadOnlyList<WorkspaceMemberDto>>> GetMembersAsync(int workspaceId);
    Task<ServiceResult> AddMemberAsync(int workspaceId, AddWorkspaceMemberDto dto);
    Task<ServiceResult> ChangeMemberRoleAsync(int workspaceId, int userId, WorkspaceRole role);
    Task<ServiceResult> RemoveMemberAsync(int workspaceId, int userId);
}