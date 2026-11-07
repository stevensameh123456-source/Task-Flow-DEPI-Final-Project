// Features/Projects/Interfaces/IProjectService.cs
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Projects.DTOs;

namespace TaskFlow.Services.Features.Projects.Interfaces;

public interface IProjectService
{
    Task<ServiceResult<ProjectResponseDto>> CreateAsync(CreateProjectDto dto);
    Task<ServiceResult<ProjectResponseDto>> UpdateAsync(int id, UpdateProjectDto dto);
    Task<ServiceResult<ProjectResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<ProjectResponseDto>>> GetByWorkspaceAsync(int workspaceId);
    Task<double> GetProgressAsync(int projectId);
}