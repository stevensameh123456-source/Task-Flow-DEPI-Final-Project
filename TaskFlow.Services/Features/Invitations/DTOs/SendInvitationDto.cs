using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Invitations.DTOs;

public class SendInvitationDto
{
    public int WorkspaceId { get; set; }

    public string Email { get; set; } = string.Empty;

    public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
}