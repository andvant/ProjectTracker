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

public class CreateIssueValidatorTests
{
    private readonly FakeTimeProvider _timeProvider = new(DateTimeOffset.UtcNow);

    [Fact]
    public async Task Validator_passes_if_valid_title()
    {
        var validator = new CreateIssueCommandValidator(_timeProvider);
        var command = new CreateIssueCommand(Guid.NewGuid(), "valid title", "test desc", null, null, null, null, null, null);

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.ShouldBeTrue();
    }

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
