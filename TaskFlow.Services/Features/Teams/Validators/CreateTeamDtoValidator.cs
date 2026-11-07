// Features/Teams/Validators/CreateTeamDtoValidator.cs
using FluentValidation;
using TaskFlow.Services.Features.Teams.DTOs;

namespace TaskFlow.Services.Features.Teams.Validators;

public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.LeaderId).GreaterThan(0);
    }
}