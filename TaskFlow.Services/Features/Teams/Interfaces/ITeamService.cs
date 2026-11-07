// Features/Teams/Interfaces/ITeamService.cs
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Teams.DTOs;

namespace TaskFlow.Services.Features.Teams.Interfaces;

public interface ITeamService
{
    Task<ServiceResult<TeamResponseDto>> CreateAsync(CreateTeamDto dto);
    Task<ServiceResult<TeamResponseDto>> UpdateAsync(int id, string name, string? description);
    Task<ServiceResult<TeamResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<TeamResponseDto>>> GetByProjectAsync(int projectId);
    Task<ServiceResult> AssignLeaderAsync(int teamId, int leaderId);
    Task<ServiceResult> AddEmployeeAsync(int teamId, int employeeId);
    Task<ServiceResult> RemoveEmployeeAsync(int teamId, int employeeId);
}