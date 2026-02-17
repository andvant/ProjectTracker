using AutoFixture.Xunit3;
using ProjectTracker.Application.Features.Issues.CreateIssue;
using ProjectTracker.Domain.Enums;
using Shouldly;
using Xunit;

namespace ProjectTracker.UnitTests.Application;

public class FakeTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}

public class CreateIssueCommandTests
{
    private readonly FakeTimeProvider _timeProvider = new(DateTimeOffset.UtcNow);

    //[Theory]
    //[AutoData]
    //public async Task Create_issue_and_return_dto(User user)
    //{
    //    var project = new Project("TP", "Test Project", user, "test desc");

    //    project.AddMember(user);

    //    List<Project> projects = [project];
    //    List<User> users = [user];

    //    var logger = Substitute.For<ILogger<CreateIssueCommandHandler>>();

    //    var command = new CreateIssueCommand(project.Id, "test issue", "test desc", user, user.Id, null, null, null, null, null);
    //    var handler = new CreateIssueCommandHandler(projects, users, mapper, _timeProvider, logger);

    //    var result = await handler.Handle(command, TestContext.Current.CancellationToken);

    //    result.ShouldNotBeNull();
    //    result.Title.ShouldBe("test issue");
    //    project.Issues.Single().Assignee.ShouldBe(user);
    //}

    //[Theory]
    //[AutoData]
    //public async Task Throw_if_assignee_doesnt_exist(User user)
    //{
    //    var missingAssigneeId = Guid.NewGuid();

    //    var project = new Project("TP", "Test Project", user, "test desc");

    //    project.Members.Add(user);

    //    List<Project> projects = [project];
    //    List<User> users = [user];

    //    var mapper = new IssueDtoMapper();
    //    var logger = Substitute.For<ILogger<CreateIssueCommandHandler>>();

    //    var command = new CreateIssueCommand(project.Id, "test issue", "test desc", user, missingAssigneeId, null, null, null, null, null);
    //    var handler = new CreateIssueCommandHandler(projects, users, mapper, _timeProvider, logger);

    //    var exception = await Should.ThrowAsync<AssigneeNotFoundException>(() =>
    //        handler.Handle(command, TestContext.Current.CancellationToken));

    //    exception.AssigneeId.ShouldBe(missingAssigneeId);
    //}

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(null)]
    public async Task Validator_fails_if_invalid_title(string? title)
    {
        var validator = new CreateIssueCommandValidator(_timeProvider);
        var command = new CreateIssueCommand(Guid.NewGuid(), title!, "test desc", null, null, null, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Title));
    }

    [Fact]
    public async Task Validator_passes_if_valid_title()
    {
        var validator = new CreateIssueCommandValidator(_timeProvider);
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", "test desc", null, null, null, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineAutoData((IssueType)9000)]
    [InlineAutoData((IssueType)(-1))]
    public async Task Validator_fails_if_invalid_type(IssueType type)
    {
        var validator = new CreateIssueCommandValidator(_timeProvider);
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", "test desc", null, type, null, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Type));
    }

    [Theory]
    [InlineAutoData((IssuePriority)9000)]
    [InlineAutoData((IssuePriority)(-1))]
    public async Task Validator_fails_if_invalid_priority(IssuePriority priority)
    {
        var validator = new CreateIssueCommandValidator(_timeProvider);
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", "test desc", null, null, priority, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateIssueCommand.Priority));
    }
}
