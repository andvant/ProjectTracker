using AutoFixture.Xunit3;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace ProjectTracker.UnitTests.Domain;

public class ProjectTests
{
    private readonly DateTimeOffset _currentTime = DateTimeOffset.UtcNow;

    [Theory]
    [AutoData]
    public void Create_valid_project(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        project.Key.ToString().ShouldBe("TP");
        project.Name.ToString().ShouldBe("Test Project");
        project.Description.ShouldBe("test desc");
        project.Owner.ShouldBe(owner);
        project.Members.ShouldContain(m => m.UserId == owner.Id);
        project.Members.Single().MemberSince.ShouldBe(_currentTime);
        project.Issues.ShouldBeEmpty();
    }

    [Theory]
    [InlineAutoData("123")]
    [InlineAutoData("TP 1")]
    [InlineAutoData("too long project key")]
    public void Throw_if_invalid_project_key(string projectKey, User owner)
    {
        Should.Throw<ProjectKeyNotValidException>(() =>
            new Project(projectKey, "Test Project", owner, "test desc", _currentTime));
    }

    [Theory]
    [AutoData]
    public void Create_issue_without_assignee(User reporter)
    {
        var project = new Project("TP", "Test Project", reporter, "test desc", _currentTime);

        var issue = project.CreateIssue(1, "test", "test desc", reporter,
            null, null, null, null, _currentTime.AddDays(1), _currentTime, 60);

        issue.ShouldNotBeNull();
        issue.Assignee.ShouldBeNull();
        issue.AssigneeId.ShouldBeNull();
        issue.Project.ShouldBe(project);
        issue.Key.Number.ShouldBe(1);
        issue.Key.ProjectKey.ShouldBe(project.Key);
        issue.Key.ToString().ShouldBe("TP-1");
        issue.Title.ToString().ShouldBe("test");
        issue.Reporter.ShouldBe(reporter);
        issue.Watchers.ShouldContain(w => w.UserId == reporter.Id);
        issue.Type.ShouldBe(IssueType.Task);
        issue.Priority.ShouldBe(IssuePriority.Normal);
        project.Issues.ShouldContain(issue);
    }

    [Theory]
    [AutoData]
    public void Create_issue_with_assignee(User reporter, User memberAssignee)
    {
        var project = new Project("TP", "Test Project", reporter, "test desc", _currentTime);
        project.AddMember(memberAssignee, _currentTime);

        var issue = project.CreateIssue(1, "test", "test desc", reporter,
            memberAssignee, null, null, null, _currentTime.AddDays(1), _currentTime, 60);

        issue.ShouldNotBeNull();
        issue.Assignee.ShouldBe(memberAssignee);
        issue.Project.ShouldBe(project);
        issue.Watchers.ShouldContain(w => w.UserId == reporter.Id);
        issue.Watchers.ShouldContain(w => w.UserId == memberAssignee.Id);
        project.Issues.ShouldContain(issue);
    }

    [Theory]
    [AutoData]
    public void Throw_if_new_issue_past_due_date(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);
        var pastDate = _currentTime.AddDays(-1);

        var exception = Should.Throw<PastDueDateException>(() =>
            project.CreateIssue(1, "test", "test desc", owner,
            null, null, null, null, pastDate, _currentTime, 60));

        exception.DueDate.ShouldBe(pastDate);
    }

    [Theory]
    [AutoData]
    public void Throw_if_assignee_is_not_member(User owner, User nonMemberAssignee)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        var exception = Should.Throw<AssigneeNotMemberException>(() =>
            project.CreateIssue(1, "test", "test desc", owner,
            nonMemberAssignee, null, null, null, _currentTime.AddDays(1), _currentTime, 60));

        exception.AssigneeId.ShouldBe(nonMemberAssignee.Id);
    }

    [Theory]
    [AutoData]
    public void Remove_member_from_assignees_and_watchers(User owner, User member)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        project.AddMember(member, _currentTime);

        var issue = project.CreateIssue(1, "test", "test desc", member,
            member, null, null, null, _currentTime.AddDays(1), _currentTime, 60);

        project.RemoveMember(member);

        issue.Assignee.ShouldBeNull();
        issue.AssigneeId.ShouldBeNull();
        issue.Watchers.ShouldNotContain(w => w.UserId == member.Id);
    }

    [Theory]
    [AutoData]
    public void Throw_if_removing_owner_from_members(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        var exception = Should.Throw<CantRemoveProjectOwnerException>(() => project.RemoveMember(owner));

        exception.UserId.ShouldBe(owner.Id);
    }

    [Theory]
    [AutoData]
    public void Throw_if_parent_issue_from_another_project(User owner)
    {
        var parentProject = new Project("TP", "Test Project", owner, "test desc", _currentTime);
        var childProject = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        var parentIssue = parentProject.CreateIssue(1, "test", "test desc", owner,
            null, IssueType.Task, null, null, _currentTime.AddDays(1), _currentTime, 60);

        var exception = Should.Throw<ParentIssueWrongProjectException>(() => childProject.CreateIssue(2, "test", "test desc",
            owner, null, null, null, parentIssue, _currentTime.AddDays(1), _currentTime, 60));

        exception.ExpectedProjectId.ShouldBe(childProject.Id);
    }

    [Theory]
    [AutoData]
    public void Throw_if_parent_issue_is_not_epic(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        var parentIssue = project.CreateIssue(1, "test", "test desc", owner,
            null, IssueType.Task, null, null, _currentTime.AddDays(1), _currentTime, 60);

        var exception = Should.Throw<ParentIssueWrongTypeException>(() => project.CreateIssue(2, "test", "test desc", owner,
            null, null, null, parentIssue, _currentTime.AddDays(1), _currentTime, 60));

        exception.ParentIssueId.ShouldBe(parentIssue.Id);
        exception.ExpectedType.ShouldBe(IssueType.Epic);
    }

    [Theory]
    [AutoData]
    public void Throw_if_child_issue_is_epic(User owner)
    {
        var project = new Project("TP", "Test Project", owner, "test desc", _currentTime);

        var parentIssue = project.CreateIssue(1, "test", "test desc", owner,
            null, IssueType.Epic, null, null, _currentTime.AddDays(1), _currentTime, 60);

        Should.Throw<ChildIssueWrongTypeException>(() => project.CreateIssue(2, "test", "test desc", owner,
            null, IssueType.Epic, null, parentIssue, _currentTime.AddDays(1), _currentTime, 60));
    }
}
