namespace ProjectTracker.Domain.ValueObjects;

public class IssueKey : ValueObject
{
    public ProjectKey ProjectKey { get; }
    public int Number { get; }

    public IssueKey(ProjectKey projectKey, int number)
    {
        ProjectKey = projectKey;
        Number = number;
    }

    public static implicit operator string(IssueKey key) => key.ToString();
    public override string ToString() => $"{ProjectKey}-{Number}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProjectKey;
        yield return Number;
    }
}
