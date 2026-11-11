// Features/Teams/DTOs/TeamResponseDto.cs
namespace TaskFlow.Services.Features.Teams.DTOs;

public record TeamResponseDto(
    int Id, string Name, string? Description, int ProjectId, int LeaderId, int MembersCount);