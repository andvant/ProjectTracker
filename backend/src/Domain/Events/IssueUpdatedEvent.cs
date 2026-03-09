namespace ProjectTracker.Domain.Events;

public record IssueUpdatedEvent(Guid IssueId) : IDomainEvent;
