using AutoFixture.Xunit3;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Exceptions;
using ProjectTracker.Application.Features.Issues.CreateIssue;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Infrastructure.Database;
using Shouldly;
using Xunit;

namespace ProjectTracker.IntegrationTests.Application;

public class CreateIssueCommandTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateIssueCommandTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [AutoData]
    public async Task Create_issue_and_return_dto(User user)
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>() as FakeCurrentUser;
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>() as FakeTimeProvider;
        currentUser!.FakeUserId = user.Id;
        timeProvider!.FakeTime = DateTimeOffset.UtcNow;

        var project = new Project("TP1", "Test Project 1", user, "test desc", DateTimeOffset.UtcNow);

        project.AddMember(user, DateTimeOffset.UtcNow);

        context.Users.Add(user);
        context.Projects.Add(project);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new CreateIssueCommand(project.Id, "test issue", "test desc", user.Id,
            null, null, null, null, null);

        // Act
        var result = await mediator.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.Title.ShouldBe("test issue");
        result.Status.ShouldBe(IssueStatus.Open);
        result.AssigneeId.ShouldBe(user.Id);
        result.CreatedOn.ShouldBe(timeProvider.FakeTime);
        result.UpdatedOn.ShouldBe(timeProvider.FakeTime);
        result.CreatedBy.ShouldBe(user.Id);
        result.UpdatedBy.ShouldBe(user.Id);

        var savedProject = await context.Projects
            .Include(p => p.Issues)
            .ThenInclude(i => i.Assignee)
            .SingleAsync(p => p.Id == project.Id, TestContext.Current.CancellationToken);

        var issue = savedProject.Issues.Single();
        issue.AssigneeId.ShouldBe(user.Id);
        issue.ReporterId.ShouldBe(user.Id);
        issue.Key.ProjectKey.ShouldBe(savedProject.Key);
    }

    [Theory]
    [AutoData]
    public async Task Throw_if_assignee_doesnt_exist(User user)
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var project = new Project("TP2", "Test Project 2", user, "test desc", DateTimeOffset.UtcNow);

        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>() as FakeCurrentUser;
        currentUser!.FakeUserId = user.Id;

        project.AddMember(user, DateTimeOffset.UtcNow);

        context.Users.Add(user);
        context.Projects.Add(project);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var missingAssigneeId = Guid.NewGuid();

        var command = new CreateIssueCommand(project.Id, "test issue", "test desc",
            missingAssigneeId, null, null, null, null, null);

        // Act & Assert
        var exception = await Should.ThrowAsync<AssigneeNotFoundException>(() =>
            mediator.Send(command, TestContext.Current.CancellationToken));

        exception.AssigneeId.ShouldBe(missingAssigneeId);
    }
}
