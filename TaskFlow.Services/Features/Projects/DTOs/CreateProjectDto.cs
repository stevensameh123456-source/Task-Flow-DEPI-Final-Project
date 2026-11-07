// Features/Projects/DTOs/CreateProjectDto.cs
namespace TaskFlow.Services.Features.Projects.DTOs;

public record CreateProjectDto(
    int WorkspaceId, string Name, string? Description,
    DateTime StartDate, DateTime? EndDate, int ProjectManagerId);