namespace ProjectTracker.Domain.Events;

public record NewOwnerEvent(string ProjectKey, string ProjectName, Guid NewOwnerId) : IDomainEvent;
