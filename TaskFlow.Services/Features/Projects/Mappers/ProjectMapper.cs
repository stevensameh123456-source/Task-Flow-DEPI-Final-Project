// Features/Projects/Mappers/ProjectMapper.cs
using TaskFlow.Data.Entities;
using TaskFlow.Services.Features.Projects.DTOs;

namespace TaskFlow.Services.Features.Projects.Mappers;

public static class ProjectMapper
{
    public static ProjectResponseDto ToDto(Project p, int teamsCount, double progressPercent)
        => new(p.Id, p.WorkspaceId, p.Name, p.Description, p.StartDate, p.EndDate,
               p.Status, p.ProjectManagerId, teamsCount, progressPercent);
}