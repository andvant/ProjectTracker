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

    public static IssueKey Parse(string key)
    {
        try
        {
            var split = key.Split('-');
            return new IssueKey(split[0], int.Parse(split[1]));
        }
        catch
        {
            throw new Exception($"Failed to parse the issue key: {key}");
        }
    }

    public static implicit operator string(IssueKey key) => key.ToString();
    public override string ToString() => $"{ProjectKey}-{Number}";
}
