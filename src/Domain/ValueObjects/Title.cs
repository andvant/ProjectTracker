namespace ProjectTracker.Domain.ValueObjects;

public record Title
{
    private const int MAX_LENGTH = 100;

    public static string ValidationMessage =>
        $"Title must be not empty and have a max length of {MAX_LENGTH}.";

    public string Value { get; }

    public Title(string title)
    {
        if (!IsValid(title))
        {
            throw new TitleNotValidException(title, ValidationMessage);
        }

        Value = title;
    }

    public static bool IsValid(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return false;

        return title.Length <= MAX_LENGTH;
    }

    public static implicit operator Title(string title) => new Title(title);
    public static implicit operator string(Title title) => title.Value;
    public override string ToString() => Value;
}
