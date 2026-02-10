using FluentValidation;

namespace ProjectTracker.Features.Projects.UpdateProject;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
    }
}
