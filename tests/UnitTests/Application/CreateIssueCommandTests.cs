using AutoFixture.Xunit3;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProjectTracker.Application.Exceptions;
using ProjectTracker.Application.Features.Issues.Common;
using ProjectTracker.Application.Features.Issues.CreateIssue;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using Shouldly;
using Xunit;

namespace ProjectTracker.UnitTests.Application;

public class CreateIssueCommandTests
{
    [Theory]
    [AutoData]
    public async Task Create_issue_and_return_dto(User user)
    {
        var project = new Project("TP", "Test Project", user, "test desc");

        project.Members.Add(user);

        List<Project> projects = [project];
        List<User> users = [user];

        var mapper = new IssueDtoMapper();
        var logger = Substitute.For<ILogger<CreateIssueCommandHandler>>();

        var command = new CreateIssueCommand(project.Id, "test issue", user, user.Id, null, null);
        var handler = new CreateIssueCommandHandler(projects, users, mapper, logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Title.ShouldBe("test issue");
        project.Issues.Single().Assignee.ShouldBe(user);
    }

    [Theory]
    [AutoData]
    public async Task Throw_if_assignee_doesnt_exist(User user)
    {
        var missingAssigneeId = Guid.NewGuid();

        var project = new Project("TP", "Test Project", user, "test desc");

        project.Members.Add(user);

        List<Project> projects = [project];
        List<User> users = [user];

        var mapper = new IssueDtoMapper();
        var logger = Substitute.For<ILogger<CreateIssueCommandHandler>>();

        var command = new CreateIssueCommand(project.Id, "test issue", user, missingAssigneeId, null, null);
        var handler = new CreateIssueCommandHandler(projects, users, mapper, logger);

        var exception = await Should.ThrowAsync<AssigneeNotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));

        exception.AssigneeId.ShouldBe(missingAssigneeId);
    }

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(null)]
    public async Task Validator_fails_if_invalid_title(string? title, User user)
    {
        var validator = new CreateIssueCommandValidator();
        var command = new CreateIssueCommand(Guid.NewGuid(), title!, user, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Title));
    }

    [Theory]
    [AutoData]
    public async Task Validator_passes_if_valid_title(User user)
    {
        var validator = new CreateIssueCommandValidator();
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", user, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineAutoData((IssueType)9000)]
    [InlineAutoData((IssueType)(-1))]
    public async Task Validator_fails_if_invalid_type(IssueType type, User user)
    {
        var validator = new CreateIssueCommandValidator();
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", user, null, type, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Type));
    }

    [Theory]
    [InlineAutoData((IssuePriority)9000)]
    [InlineAutoData((IssuePriority)(-1))]
    public async Task Validator_fails_if_invalid_priority(IssuePriority priority, User user)
    {
        var validator = new CreateIssueCommandValidator();
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", user, null, null, priority);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Priority));
    }
}
