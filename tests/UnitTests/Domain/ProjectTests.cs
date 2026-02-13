using AutoFixture.Xunit3;
using ProjectTracker.Domain.Entities;
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
        var project = new Project("TP", "Test Project", owner);

        var issue = project.CreateIssue(1, "test", owner);

        issue.ShouldNotBeNull();
        issue.Project.ShouldBe(project);
        issue.Number.ShouldBe(1);
        issue.Title.ShouldBe("test");
        issue.Creator.ShouldBe(owner);
        project.Issues.ShouldContain(issue);
    }

    [Theory]
    [AutoData]
    public void Create_issue_with_assignee(User owner, User memberAssignee)
    {
        var project = new Project("TP", "Test Project", owner);
        project.Members.Add(memberAssignee);

        var issue = project.CreateIssue(1, "test", owner, memberAssignee);

        issue.Assignee.ShouldBe(memberAssignee);
    }

    [Theory]
    [AutoData]
    public void Throw_if_assignee_is_not_member(User owner, User nonMemberAssignee)
    {
        var project = new Project("TP", "Test Project", owner);

        var exception = Should.Throw<AssigneeNotMemberException>(() =>
            project.CreateIssue(1, "test", owner, nonMemberAssignee));

        exception.AssigneeId.ShouldBe(nonMemberAssignee.Id);
    }
}
