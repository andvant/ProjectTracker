using System.Text.RegularExpressions;

namespace ProjectTracker.Domain.ValueObjects;

public record ProjectKey
{
    private const int MAX_LENGTH = 10;
    private static readonly Regex Pattern = new("^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

    public static string ValidationMessage =>
        $"Project key must start with a letter, contain only alphanumeric characters and have a max length of {MAX_LENGTH}.";

    public string Value { get; }

    public ProjectKey(string key)
    {
        if (!IsValid(key))
        {
            throw new ProjectKeyNotValidException(key, ValidationMessage);
        }

        Value = key;
    }

    public static bool IsValid(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        if (key.Length > MAX_LENGTH) return false;

        return Pattern.IsMatch(key);
    }

    public static implicit operator string(ProjectKey key) => key.Value;
    public override string ToString() => Value;
}
