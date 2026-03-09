namespace ProjectTracker.Domain.Exceptions;

public class NegativeEstimationException : DomainException
{
    public int EstimationMinutes { get; }

    public NegativeEstimationException(int estimationMinutes)
        : base($"Estimation cannot be negative. Value: '{estimationMinutes}'")
    {
        EstimationMinutes = estimationMinutes;
    }
}
