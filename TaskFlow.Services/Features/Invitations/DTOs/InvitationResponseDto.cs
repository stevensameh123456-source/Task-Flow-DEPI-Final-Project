using TaskFlow.Data.Entities;

namespace TaskFlow.Services.Features.Invitations.DTOs;

public record InvitationResponseDto(
    int Id,
    int WorkspaceId,
    string Email,
    WorkspaceRole Role,
    DateTime ExpiresAt,
    bool IsAccepted
);