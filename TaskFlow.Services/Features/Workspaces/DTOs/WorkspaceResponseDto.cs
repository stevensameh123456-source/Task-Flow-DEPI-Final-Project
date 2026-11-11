// Features/Workspaces/DTOs/WorkspaceResponseDto.cs
using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Workspaces.DTOs;

public record WorkspaceResponseDto(
    int Id, string Name, string? Description, int OwnerId, int MembersCount, int ProjectsCount);

// لصفحة Members
public record WorkspaceMemberDto(int UserId, string FullName, WorkspaceRole Role, DateTime JoinedAt);