// Features/Workspaces/DTOs/AddWorkspaceMemberDto.cs
using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Workspaces.DTOs;

public record AddWorkspaceMemberDto(int UserId, WorkspaceRole Role);