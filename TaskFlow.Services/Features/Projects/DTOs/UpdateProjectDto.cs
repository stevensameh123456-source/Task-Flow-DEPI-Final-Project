// Features/Projects/DTOs/UpdateProjectDto.cs
using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Projects.DTOs;

public record UpdateProjectDto(
    string Name, string? Description, DateTime StartDate, DateTime? EndDate, ProjectStatus Status);