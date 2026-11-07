// Features/Projects/Validators/CreateProjectDtoValidator.cs
using FluentValidation;
using TaskFlow.Services.Features.Projects.DTOs;

namespace TaskFlow.Services.Features.Projects.Validators;

public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.WorkspaceId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.ProjectManagerId).GreaterThan(0);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");
    }
}