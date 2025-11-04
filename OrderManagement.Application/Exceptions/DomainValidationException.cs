namespace OrderManagement.Application.Exceptions;

public class DomainValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public DomainValidationException(IEnumerable<string> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}
