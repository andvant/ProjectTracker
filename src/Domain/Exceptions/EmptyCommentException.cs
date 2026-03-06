namespace ProjectTracker.Domain.Exceptions;

public class EmptyCommentException : DomainException
{
    public EmptyCommentException() : base("Comment cannot be empty.")
    {
    }
}
