using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Issues.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Issues.CreateIssue;

public record CreateIssueCommand(Guid ProjectId, string Title, User Creator,
    Guid? AssigneeId = null, IssuePriority? Priority = null) : IRequest<IssueDto>;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly List<Project> _projects;
    private readonly IssueDtoMapper _mapper;
    private readonly ILogger<CreateIssueCommandHandler> _logger;

    public CreateIssueCommandHandler(
        List<Project> projects,
        IssueDtoMapper mapper,
        ILogger<CreateIssueCommandHandler> logger)
    {
        _projects = projects;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand command, CancellationToken cancellationToken)
    {
        var nextIssueNumber = _projects.SelectMany(p => p.Issues).Max(i => i.Number) + 1;

        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ApplicationException($"Project with id {command.ProjectId} not found");
        }

        User? assignee = null;

        if (command.AssigneeId != null)
        {
            assignee = project.Members.FirstOrDefault(m => m.Id == command.AssigneeId.Value);

            if (assignee is null)
            {
                throw new ApplicationException($"Assignee with id {command.AssigneeId} is not a member of the project");
            }
        }

        var issue = project.CreateIssue(nextIssueNumber, command.Title, command.Creator, assignee, command.Priority);

        _logger.LogInformation(
            "Created issue with id '{Id}', short name '{ShortName}', title '{Title}'",
            issue.Id, issue.ShortName, issue.Title);

        return _mapper.ToDto(issue);
    }
}

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(c => c.Title).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Priority).IsInEnum();
    }
}
