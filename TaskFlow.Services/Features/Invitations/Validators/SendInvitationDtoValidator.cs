using FluentValidation;
using TaskFlow.Data.Entities;
using TaskFlow.Services.Features.Invitations.DTOs;

namespace TaskFlow.Services.Features.Invitations.Validators;

public class SendInvitationDtoValidator
    : AbstractValidator<SendInvitationDto>
{
    public SendInvitationDtoValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .GreaterThan(0)
            .WithMessage("Workspace ID must be greater than zero.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.")
            .MaximumLength(256);

        RuleFor(x => x.Role)
            .Must(role =>
                role == WorkspaceRole.Admin ||
                role == WorkspaceRole.Member ||
                role == WorkspaceRole.Viewer)
            .WithMessage("Invalid invitation role.");
    }
}