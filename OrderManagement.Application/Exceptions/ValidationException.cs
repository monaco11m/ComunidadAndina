namespace OrderManagement.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Validation failed.")
        {
            Errors = errors;
        }
    }
}
