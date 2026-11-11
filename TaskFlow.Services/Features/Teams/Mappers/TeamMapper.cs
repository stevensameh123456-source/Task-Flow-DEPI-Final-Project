// Features/Teams/Mappers/TeamMapper.cs
using TaskFlow.Data.Entities;
using TaskFlow.Services.Features.Teams.DTOs;

namespace TaskFlow.Services.Features.Teams.Mappers;

public static class TeamMapper
{
    public static TeamResponseDto ToDto(Team t, int membersCount)
        => new(t.Id, t.Name, t.Description, t.ProjectId, t.LeaderId, membersCount);
}