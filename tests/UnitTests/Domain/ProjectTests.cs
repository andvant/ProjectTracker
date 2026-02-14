using AutoFixture.Xunit3;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace ProjectTracker.UnitTests.Domain;

public class ProjectTests
{
    [Theory]
    [AutoData]
    public void Create_issue_without_assignee(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc");

        var issue = project.CreateIssue(1, "test", "test desc", owner, null, null, null, null, null, null);

        issue.ShouldNotBeNull();
        issue.Project.ShouldBe(project);
        issue.Key.Number.ShouldBe(1);
        issue.Title.ToString().ShouldBe("test");
        issue.Reporter.ShouldBe(owner);
        issue.Type.ShouldBe(IssueType.Task);
        issue.Priority.ShouldBe(IssuePriority.Normal);
        project.Issues.ShouldContain(issue);
    }

    [Theory]
    [AutoData]
    public void Create_issue_with_assignee(User owner, User memberAssignee)
    {
        var project = new Project("TP", "Test Project", owner, "test desc");
        project.Members.Add(memberAssignee);

        var issue = project.CreateIssue(1, "test", "test desc", owner, memberAssignee, null, null, null, null, null);

        issue.Assignee.ShouldBe(memberAssignee);
    }

    [Theory]
    [AutoData]
    public void Throw_if_assignee_is_not_member(User owner, User nonMemberAssignee)
    {
        var project = new Project("TP", "Test Project", owner, "test desc");

        var exception = Should.Throw<AssigneeNotMemberException>(() =>
            project.CreateIssue(1, "test", "test desc", owner, nonMemberAssignee, null, null, null, null, null));

        exception.AssigneeId.ShouldBe(nonMemberAssignee.Id);
    }
}
