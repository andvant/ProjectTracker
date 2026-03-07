namespace ProjectTracker.Domain.Events;

public record MemberAddedEvent(string ProjectKey, string ProjectName, Guid MemberId) : IDomainEvent;
