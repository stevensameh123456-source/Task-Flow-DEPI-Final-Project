// Features/Workspaces/Mappers/WorkspaceMapper.cs
using TaskFlow.Data.Entities;
using TaskFlow.Services.Features.Workspaces.DTOs;

namespace TaskFlow.Services.Features.Workspaces.Mappers;

public static class WorkspaceMapper
{
    public static WorkspaceResponseDto ToDto(Workspace w, int membersCount, int projectsCount)
        => new(w.Id, w.Name, w.Description, w.OwnerId, membersCount, projectsCount);

    public static WorkspaceMemberDto ToDto(WorkspaceMember m, string fullName)
        => new(m.UserId, fullName, m.Role, m.JoinedAt);
}