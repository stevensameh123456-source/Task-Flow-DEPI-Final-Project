// Features/Workspaces/Validators/CreateWorkspaceDtoValidator.cs
using FluentValidation;
using TaskFlow.Services.Features.Workspaces.DTOs;

namespace TaskFlow.Services.Features.Workspaces.Validators;

public class CreateWorkspaceDtoValidator : AbstractValidator<CreateWorkspaceDto>
{
    public CreateWorkspaceDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}