// Features/Teams/DTOs/CreateTeamDto.cs
namespace TaskFlow.Services.Features.Teams.DTOs;

public record CreateTeamDto(string Name, string? Description, int ProjectId, int LeaderId);