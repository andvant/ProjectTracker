namespace ProjectTracker.Domain.Events;

public record IssueAssignedEvent(Guid IssueId, Guid AssigneeId) : IDomainEvent;
