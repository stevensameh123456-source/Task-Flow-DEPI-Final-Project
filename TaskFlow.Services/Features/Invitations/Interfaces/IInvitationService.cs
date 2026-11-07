using TaskFlow.Services.Features.Invitations.DTOs;

namespace TaskFlow.Services.Features.Invitations.Interfaces;

public interface IInvitationService
{
    Task<InvitationResponseDto> InviteAsync(
        SendInvitationDto dto);

    Task AcceptAsync(
        AcceptInvitationDto dto);

    Task RevokeAsync(
        int invitationId);

    Task<IReadOnlyList<InvitationResponseDto>> GetPendingAsync(
        int workspaceId);
}