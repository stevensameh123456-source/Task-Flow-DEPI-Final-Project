// Features/Projects/DTOs/ProjectResponseDto.cs
using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Projects.DTOs;

public record ProjectResponseDto(
    int Id, int WorkspaceId, string Name, string? Description,
    DateTime StartDate, DateTime? EndDate, ProjectStatus Status,
    int ProjectManagerId, int TeamsCount, double ProgressPercent);