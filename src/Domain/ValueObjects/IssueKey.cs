namespace ProjectTracker.Domain.ValueObjects;

public record IssueKey
{
    public ProjectKey ProjectKey { get; }
    public int Number { get; }

    public IssueKey(ProjectKey projectKey, int number)
    {
        if (number <= 0)
        {
            throw new IssueNumberNotValidException(number);
        }

        ProjectKey = projectKey;
        Number = number;
    }

    public static implicit operator string(IssueKey key) => key.ToString();
    public override string ToString() => $"{ProjectKey}-{Number}";
}
