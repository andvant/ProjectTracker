using FluentValidation;

namespace ProjectTracker.Features.Projects.CreateProject;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
        RuleFor(c => c.ShortName).MaximumLength(10).Matches("^[a-zA-Z]{1}[a-zA-Z0-9]*$").NotEmpty();
    }
}
