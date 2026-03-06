using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Domain.Events;

public record MemberAddedEvent(Project Project, User Member) : IDomainEvent;
