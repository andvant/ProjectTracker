namespace ProjectTracker.Domain.Exceptions;

public class TitleNotValidException : DomainException
{
    public string Title { get; }

    public TitleNotValidException(string title, string message)
        : base(message)
    {
        Title = title;
    }
}
